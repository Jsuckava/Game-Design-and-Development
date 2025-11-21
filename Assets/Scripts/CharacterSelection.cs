using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video; 
using TMPro;

public class CharacterSelection : MonoBehaviour
{
    public GameData gameData;
    
    [Header("Character Data")]
    public CharacterVideoSet[] characterDatabase; 
    
    [Header("UI Elements")]
    public TextMeshProUGUI statusText; 
    public Button selectButton;
    
    [Header("UI Panels")]
    public GameObject vsPopupPanel;
    public GameObject characterGrid;

    [Header("FINAL MATCHUP VIDEOS (Popup Panel)")]
    public VideoPlayer finalPlayer1VideoPlayer; 
    public VideoPlayer finalPlayer2VideoPlayer; 

    [Header("Highlights")]
    public GameObject[] characterHighlights;

    private int pickingPlayer = 1;
    private int currentlyHighlightedID = -1;
    private TextMeshProUGUI selectButtonText;
    
    void Start()
    {
        gameData.ResetSelections();
        pickingPlayer = 1;
        currentlyHighlightedID = -1;
        
        if (selectButton != null)
        {
            selectButtonText = selectButton.GetComponentInChildren<TextMeshProUGUI>(true);
        }
        
        if (selectButton == null)
        {
             Debug.LogError("FATAL ERROR: 'Select Button' is NULL in the Inspector.");
             return;
        }
       
        if (selectButtonText == null)
        {
             Debug.LogError("FATAL ERROR: 'Select Button' is linked, but the required TextMeshPro component is missing from its children. Please check the button's hierarchy.");
             return;
        }

        if (finalPlayer1VideoPlayer != null) finalPlayer1VideoPlayer.Stop(); 
        if (finalPlayer2VideoPlayer != null) finalPlayer2VideoPlayer.Stop();
        
        vsPopupPanel.SetActive(false);
        characterGrid.SetActive(true);
        
        UpdateUI(); 
    }

    private void StartGame()
    {
        SceneManager.LoadScene("FightingScene");
    }

    public void HighlightCharacter(int characterID)
    {
        if (pickingPlayer == 3) return;
        int arrayIndex = characterID - 1; 

        if (arrayIndex < 0 || arrayIndex >= characterDatabase.Length)
        {
            Debug.LogError($"Attempted to highlight invalid character ID: {characterID} (Index: {arrayIndex}). Database size: {characterDatabase.Length}");
            return;
        }

        currentlyHighlightedID = arrayIndex;
        UpdateUI();
    }

    public void ConfirmSelection()
    {
        if (pickingPlayer == 3)
        {
            Debug.Log("Phase 3 (Ready) detected. Starting Game...");
            StartGame();
            return;
        }
        if (currentlyHighlightedID == -1) return;

        if (pickingPlayer == 1)
        {
            gameData.player1CharacterID = currentlyHighlightedID;
            Debug.Log($"Player 1 Selected Character ID: {gameData.player1CharacterID}");
            pickingPlayer = 2;
            
            currentlyHighlightedID = -1;
            UpdateUI();
            return; 
        }
        else if (pickingPlayer == 2)
        {
            gameData.player2CharacterID = currentlyHighlightedID;
            Debug.Log($"Playyer 2 Selected Character ID: {gameData.player2CharacterID}");
            pickingPlayer = 3;
            
            LoadAndPlayMatchupVideos(); 
            
            currentlyHighlightedID = -1;
            UpdateUI();
            return; 
        }
        
        
    }

    private void LoadAndPlayMatchupVideos()
    {
        Debug.Log("Loading Matchup Videos...");

        if (finalPlayer1VideoPlayer != null) finalPlayer1VideoPlayer.gameObject.SetActive(true);
        if (finalPlayer2VideoPlayer != null) finalPlayer2VideoPlayer.gameObject.SetActive(true);
        
        vsPopupPanel.SetActive(true); 

        if (gameData.player1CharacterID != -1)
        {
            PlayVideoOnTarget(finalPlayer1VideoPlayer, gameData.player1CharacterID);
        }
        
        if (gameData.player2CharacterID != -1)
        {
            PlayVideoOnTarget(finalPlayer2VideoPlayer, gameData.player2CharacterID);
        }
    }

    private void PlayVideoOnTarget(VideoPlayer targetPlayer, int charID)
    {
        if (charID < 0 || characterDatabase == null || charID >= characterDatabase.Length)
        {
            Debug.LogError($"FATAL ERROR: Invalid Data for PlayVideoOnTarget. CharID: {charID}, Database Size: {(characterDatabase != null ? characterDatabase.Length.ToString() : "null")}");
            return;
        }

        if (targetPlayer == null)
        {
             Debug.LogError($"FATAL ERROR: Target Video Player is null.");
             return;
        }

        if (characterDatabase[charID].idleClip == null)
        {
             Debug.LogError($"IdleClip is NULL for character ID {charID}");
             return;
        }

        VideoClip clipToPlay = characterDatabase[charID].idleClip;
        
        targetPlayer.clip = clipToPlay; 
        targetPlayer.Play(); 
    }

    private void UpdateUI()
    {
        if (characterHighlights == null || statusText == null || selectButton == null || selectButtonText == null || characterDatabase == null)
        {
            Debug.LogError("FATAL ERROR: Essential UI elements are not linked. Check Inspector.");
            return;
        }
        
        for (int i = 0; i < characterHighlights.Length; i++)
        {
            if (characterHighlights[i] != null)
            {
                bool isActive = (i == currentlyHighlightedID);
                characterHighlights[i].SetActive(isActive);
            }
        }

        if (pickingPlayer == 1)
        {
            statusText.text = "PLAYER 1: CHOOSE YOUR FIGHTER";
            selectButtonText.text = "SELECT";
            selectButton.interactable = (currentlyHighlightedID != -1);
        }
        else if (pickingPlayer == 2)
        {
            statusText.text = "PLAYER 2: CHOOSE YOUR FIGHTER";
            selectButtonText.text = "SELECT";
            selectButton.interactable = (currentlyHighlightedID != -1);
        }
        else if (pickingPlayer == 3)
        {
            statusText.text = "ARE YOU READY?";
            selectButtonText.text = "START GAME";
            selectButton.interactable = true;
            
            characterGrid.SetActive(false);
            vsPopupPanel.SetActive(true);
        }
    }


}