using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;
using System; 

public enum GameState
{
    Player1Turn,
    Player1Attack,
    Player2React,
    Player2Turn,
    Player2Attack,
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
   
    public GameObject player1_UI_Controls;
    public GameObject player2_UI_Controls;

    public GameData gameData;
    public CharacterVideoSet[] characterDatabase;

    void Start()
    {
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
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        
        // FIX: Corrected capitalization from player2_UI_CONTROLS to player2_UI_Controls
        player1_UI_Controls.SetActive(false);
        player2_UI_Controls.SetActive(false); 

        switch (currentState)
        {
            case GameState.Player1Turn:
                player1_UI_Controls.SetActive(true); 
                player1.PlayIdle();
                player2.PlayIdle();
                break;

            case GameState.Player1Attack:
                player1.PlayAttack1(); 
                break;

            case GameState.Player2React:
                player2.PlayHit();
                player2Health -= 20; 
                if (player2Health <= 0) ChangeState(GameState.Win); 
                break;

            case GameState.Player2Turn:
                player2_UI_Controls.SetActive(true); 
                player1.PlayIdle();
                player2.PlayIdle();
                break;
            
            case GameState.Player2Attack:
                player2.PlayAttack1(); 
                break;

            case GameState.Player1React:
                player1.PlayHit();
                player1Health -= 20; 
                if (player1Health <= 0) ChangeState(GameState.Lose);
                break;

            case GameState.Win:
                Debug.Log("Player 1 Wins!");
                break;

            case GameState.Lose:
                Debug.Log("Player 2 Wins!");
                break;
        }
    }

    public void OnPlayer1AttackButton1Pressed()
    {
        if (currentState == GameState.Player1Turn)
        {
            ChangeState(GameState.Player1Attack);
        }
    }
    
    // FIX: Restored custom playback calls for Kick and Super before state change
    public void OnPlayer1KickPressed()
    {
        if (currentState == GameState.Player1Turn)
        {
            player1.PlayKick();
            ChangeState(GameState.Player1Attack);
        }
    }
    
    public void OnPlayer1SuperPressed()
    {
        if (currentState == GameState.Player1Turn)
        {
            player1.PlaySuper();
            ChangeState(GameState.Player1Attack);
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
            player2.PlayKick();
            ChangeState(GameState.Player2Attack);
        }
    }
    
    public void OnPlayer2SuperPressed()
    {
        if (currentState == GameState.Player2Turn)
        {
            player2.PlaySuper();
            ChangeState(GameState.Player2Attack);
        }
    }
    
    // RESTORED: Turn advancement logic linked to VideoPlayer completion
    private void OnPlayer1VideoFinished()
    {
        if (currentState == GameState.Player1Attack)
        {
            ChangeState(GameState.Player2React);
        }
        else if (currentState == GameState.Player1React)
        {
            ChangeState(GameState.Player1Turn); 
        }
    }

    private void OnPlayer2VideoFinished()
    {
        if (currentState == GameState.Player2Attack)
        {
            ChangeState(GameState.Player1React);
        }
        else if (currentState == GameState.Player2React)
        {
            ChangeState(GameState.Player2Turn); 
        }
    }
}