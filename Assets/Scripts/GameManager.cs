using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;
using System;
using UnityEngine.UI;
using NUnit.Framework.Interfaces;

public enum GameState
{
    Player1Turn,
    Player1Attack,
    Player1Kick,
    Player1Super,

    Player2React,
    Player2Turn,
    Player2Attack,
    Player2Kick,
    Player2Super,
    Player1React,
    Win,
    Lose
}

public class GameManager : MonoBehaviour
{
    public GameState currentState;
    public CharacterVideo player1;
    public CharacterVideo player2;

    public int player1Health = 100;
    public int player2Health = 100;
    private int skillDmg = 0;
   
    public GameObject player1_UI_Controls;
    public GameObject player2_UI_Controls;

    public GameData gameData;
    public CharacterVideoSet[] characterDatabase;

    [Header("Movement Settings")]
    public RectTransform player1Display;
    public RectTransform player2Display;
    public float moveDist = 400f;
    public float moveDur = 0.2f;

    [Header("Winner Panel")]
    public GameObject winPanel;
    public VideoPlayer winnerVideoPlayer;
    public GameObject uiControlsP1;
    public GameObject uiControlsP2;

    [Header("Super Buttons")]
    public Button p1SuperBtn;
    public Button p2SuperBtn;

    [Header("Ui Bars")]
    public Slider p1HealthSlider;
    public Slider p2HealthSlider;
    public Slider p1SuperSlider;
    public Slider p2SuperSlider;

    private int p1Super = 0;
    private int p2Super = 0;



    private Vector2 p1StartPos;
    private Vector2 p2StartPos;

    void Start()
    {
        if (p1HealthSlider) { p1HealthSlider.maxValue = 100; p1HealthSlider.value = 100; }
        if (p2HealthSlider) { p2HealthSlider.maxValue = 100; p2HealthSlider.value = 100; }
        if (p1SuperSlider) { p1SuperSlider.maxValue = 100; p1SuperSlider.value = 0; }
        if (p2SuperSlider) { p2SuperSlider.maxValue = 100; p2SuperSlider.value = 0; }

        UpdateSuperButtons(1);
        UpdateSuperButtons(2);

        if (player1Display != null) p1StartPos = player1Display.anchoredPosition;
        if (player2Display != null) p2StartPos = player2Display.anchoredPosition;
        int player1ID = gameData.player1CharacterID;
        int player2ID = gameData.player2CharacterID;

        if (player1ID != -1 && player2ID != -1 && 
            player1ID < characterDatabase.Length && player2ID < characterDatabase.Length)
        {
            player1.SetupCharacter(characterDatabase[player1ID]);
            player2.SetupCharacter(characterDatabase[player2ID]);
        }
        else
        {
            Debug.LogError("Character ID from GameData is invalid! Loading defaults.");
            player1.SetupCharacter(characterDatabase[0]);
            player2.SetupCharacter(characterDatabase[1]);
        }
        
        // RESTORED: Subscribe to the video finish event for automatic turn advancement
        player1.OnVideoFinished += OnPlayer1VideoFinished; 
        player2.OnVideoFinished += OnPlayer2VideoFinished; 

        ChangeState(GameState.Player1Turn);
        ResetPositions();
    }

    void UpdateSuperButtons(int playerId)
    {
        Button btn = (playerId == 1) ? p1SuperBtn : p2SuperBtn;
        int currentSuper = (playerId == 1) ? p1Super : p2Super;

        if (btn != null)
        {
            bool isReady = (currentSuper >= 100);
            btn.interactable = isReady;

            Color btnColor = btn.image.color;
            btnColor.a = isReady ? 1.0f : 5.0f;
            btn.image.color = btnColor;
        }
    }

    private void ResetPositions()
    {
        if (player1Display != null) StartCoroutine(DashMove(player1Display, p1StartPos));
        if (player1Display != null) player1Display.localScale = new Vector2(-1f,1f);

        if (player2Display != null) StartCoroutine(DashMove(player2Display, p2StartPos));
        if (player2Display != null) player2Display.localScale = new Vector2(1f, 1f);

    }

    IEnumerator DashMove(RectTransform target, Vector2 dest)
    {
        if (target == null) yield break;

        Vector2 start = target.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < moveDur)
        {
            target.anchoredPosition = Vector2.Lerp(start, dest, elapsed / moveDur);
            elapsed += moveDur;
            yield return null;
        }
        target.anchoredPosition = dest;
    }

    void AddSuper(int playerId, int amount)
    {
        if (playerId == 1)
        {
            p1Super = Mathf.Clamp(p1Super + amount, 0, 100);
            if (p1SuperSlider) p1SuperSlider.value = p1Super;
            
        }
        else
        {
            p2Super = Mathf.Clamp(p2Super + amount, 0, 100);
            if (p2SuperSlider) p2SuperSlider.value = p2Super;
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        
        // FIX: Corrected capitalization from player2_UI_CONTROLS to player2_UI_Controls
        player1_UI_Controls.SetActive(false);
        player2_UI_Controls.SetActive(false);
        //skillDmg = 0;

        switch (currentState)
        {
            case GameState.Player1Turn:
                ResetPositions();
                player1_UI_Controls.SetActive(true);
                skillDmg = 0;
                player1.PlayIdle();
                player2.PlayIdle();
                break;

            case GameState.Player1Attack:
                player1.PlayAttack1();
                player2.PlayHit();
                skillDmg = 15;
                AddSuper(1, 20);
                break;

            case GameState.Player1Kick:
                player1.PlayKick();
                player2.PlayHit();
                skillDmg = 20;
                AddSuper(1, 25);

                break;

            case GameState.Player1Super:
                StartCoroutine(DashMove(player1Display, p1StartPos + new Vector2(moveDist, 0)));
                player1Display.localScale = new Vector2(-1.2f, 1.2f);
                player2Display.localScale = new Vector2(1.31f, 1.21f);
                player1.PlaySuper();
                player2.PlayHit();
                skillDmg = 30;
                AddSuper(1, -100);

                break;

            case GameState.Player2React:
                ResetPositions();
                //player2.PlayHit();
                player2Health -= skillDmg;
                if (p2HealthSlider) p2HealthSlider.value = player2Health; // --- NEW: Update UI

                if (player2Health <= 0) ChangeState(GameState.Win);
                else ChangeState(GameState.Player2Turn);
                break;

            case GameState.Player2Turn:
                ResetPositions();
                player2_UI_Controls.SetActive(true); 
                player1.PlayIdle();
                player2.PlayIdle();
                skillDmg = 0;
                break;
            
            case GameState.Player2Attack:
                player2.PlayAttack1();
                player1.PlayHit();
                skillDmg = 15;
                AddSuper(2, 20);
                break;


            case GameState.Player2Kick:
                player2.PlayKick();
                player1.PlayHit();
                skillDmg = 20;
                AddSuper(2, 25);

                break;


            case GameState.Player2Super:
                player2Display.localScale = new Vector2(2f, 2f);
                player1Display.localScale = new Vector2(-2f, 2f);

                StartCoroutine(DashMove(player2Display, p2StartPos + new Vector2(-moveDist, 0)));
                player2.PlaySuper();
                player1.PlayHit();
                skillDmg = 30;
                break;

            case GameState.Player1React:
                player1.PlayHit();
                player1Health -= skillDmg;
                if (p1HealthSlider) p1HealthSlider.value = player1Health; // --- NEW: Update UI

                if (player1Health <= 0) ChangeState(GameState.Lose);
                else ChangeState(GameState.Player1Turn);
                break;

            case GameState.Win:
                Debug.Log("Player 1 Wins!");
                ShowWinPanel(1);
                break;

            case GameState.Lose:
                Debug.Log("Player 2 Wins!");
                ShowWinPanel(2);
                break;
        }
    }

    void ShowWinPanel(int winnerId)
    {
        player1_UI_Controls.SetActive(false);
        player1_UI_Controls.SetActive(false);

        if (p1HealthSlider) p1HealthSlider.gameObject.SetActive(false);
        if (p2HealthSlider) p2HealthSlider.gameObject.SetActive(false);
        if (p1SuperSlider) p1SuperSlider.gameObject.SetActive(false);
        if (p2SuperSlider) p2SuperSlider.gameObject.SetActive(false);

        if (winPanel != null) winPanel.SetActive(true);

        CharacterVideoSet winnerData = null;
        int charID = -1;

        if (winnerId == 1)
        {
            charID = gameData.player1CharacterID;
        }
        else
        {
            charID = gameData.player2CharacterID;
        }
        if (charID >= 0 && charID < characterDatabase.Length)
        {
            winnerData = characterDatabase[charID];
        }
        else
        {
            // Fallback if IDs are messed up
            winnerData = characterDatabase[0];
        }

        // 4. Play the Winner's Video (Using Idle Clip for celebration)
        if (winnerVideoPlayer != null && winnerData != null)
        {
            winnerVideoPlayer.clip = winnerData.idleClip; // Or use a specific victoryClip if you added one
            winnerVideoPlayer.Play();
        }

    }

    public void OnPlayer1AttackButton1Pressed()
    {
        //if (p1Super >= 100)
        //{
        //}
        if (currentState == GameState.Player1Turn)
        {
            //player1.PlayAttack1();
            ChangeState(GameState.Player1Attack);
        }
    }
    
    // FIX: Restored custom playback calls for Kick and Super before state change
    public void OnPlayer1KickPressed()
    {
        if (currentState == GameState.Player1Turn)
        {
            //player1.PlayKick();
            ChangeState(GameState.Player1Kick);
        }
    }
    
    public void OnPlayer1SuperPressed()
    {
        if (currentState == GameState.Player1Turn && p1Super >= 100)
        {
            //player1.PlaySuper();
            ChangeState(GameState.Player1Super);
        }
    }
    
    public void OnPlayer2AttackButton1Pressed()
    {
        if (currentState == GameState.Player2Turn)
        {
            ChangeState(GameState.Player2Attack);
        }
    }
    
    public void OnPlayer2KickPressed()
    {
        if (currentState == GameState.Player2Turn)
        {
            //player2.PlayKick();
            ChangeState(GameState.Player2Kick);
        }
    }
    
    public void OnPlayer2SuperPressed()
    {
        if (currentState == GameState.Player2Turn && p2Super >= 100)
        {
            //player2.PlaySuper();
            ChangeState(GameState.Player2Super);
        }
    }
    
    // RESTORED: Turn advancement logic linked to VideoPlayer completion
    private void OnPlayer1VideoFinished()
    {
        if (currentState == GameState.Player1Attack ||
            currentState == GameState.Player1Kick ||
            currentState == GameState.Player1Super)
        {
            ChangeState(GameState.Player2React);
        }
        //if (currentState == GameState.Player1Attack)
        //{
        //    ChangeState(GameState.Player2React);
        //}
        //else if (currentState == GameState.Player1React)
        //{
        //    ChangeState(GameState.Player1Turn); 
        //}
    }

    private void OnPlayer2VideoFinished()
    {
        if (currentState == GameState.Player2Attack ||
            currentState == GameState.Player2Kick ||
            currentState == GameState.Player2Super)
        {
            ChangeState(GameState.Player1React);
        }
        //if (currentState == GameState.Player2Attack)
        //{
        //    ChangeState(GameState.Player1React);
        //}
        //else if (currentState == GameState.Player2React)
        //{
        //    ChangeState(GameState.Player2Turn); 
        //}
    }
}