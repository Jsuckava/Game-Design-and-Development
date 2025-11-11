using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI; 
using System.Collections.Generic;
using System;
using System.Collections;

public class MultiPlayerSelectionOfPlayerManager : MonoBehaviour
{
    public CharacterManager characterManager;
    public TextMeshProUGUI playerTurnText;
    public TextMeshProUGUI[] confirmedNameTextSlots = new TextMeshProUGUI[4];
    public Button startGameButton; 
    public Button mainSelectButton; 
    
    public PopOutUp popoutManager; 
    public TMP_InputField nameInputField; 
    public TextMeshProUGUI popoutTitleText; 

    public int currentPlayerID = 1;
    private const int MAX_PLAYERS = 4;
    private const string SelectedOptionKeyPrefix = "Player_"; 
    private const string PlayerNameKeyPrefix = "PlayerName_"; 
    private HashSet<int> selectedCharacterIndices = new HashSet<int>(); 
    
    void Start() 
    { 
        selectedCharacterIndices.Clear(); 
        StartNewSelectionRound(); 
    }

    private void StartNewSelectionRound() 
    {
        if (characterManager == null)
        {
            Debug.LogError("FATAL ERROR: Character Manager script reference is missing.");
            return;
        }

        characterManager.gameObject.SetActive(false); 

        if (popoutManager != null)
        {
            popoutManager.ShowPanel(); 
        }
        if (mainSelectButton != null)
        {
            mainSelectButton.interactable = false;
        }

        if (popoutTitleText != null)
        {
            popoutTitleText.text = "Type Your Name Player " + currentPlayerID.ToString();
        }
        if (nameInputField != null)
        {
            nameInputField.text = "";
            nameInputField.Select();
        }
        
        playerTurnText.text = "Player " + currentPlayerID.ToString() + " Enters Name...";

        if (startGameButton != null)
        {
            startGameButton.interactable = false;
        }
    }
    
    public void ConfirmNameOnly()
    {
        if (nameInputField == null)
        {
            Debug.LogError("Input Field is NULL. Assign it in the Inspector.");
            return;
        }
        
        string playerName = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Please enter a name.");
            return;
        }

        PlayerPrefs.SetString(PlayerNameKeyPrefix + currentPlayerID, playerName);
        Debug.Log($"Name saved: Player {currentPlayerID} is now {playerName}.");

        if (popoutManager != null)
        {
            popoutManager.HidePanel();
        }

        // Show character selection UI
        characterManager.gameObject.SetActive(true); 
        characterManager.SetUsedCharacters(selectedCharacterIndices); 
        characterManager.selectedOption = -1;
        characterManager.NextOption(); 

        if (mainSelectButton != null)
        {
            mainSelectButton.interactable = true;
        }
        playerTurnText.text = playerName + " Selects...";
    }
    
    public void ConfirmSelection(int sceneID) 
    {
        string playerName = PlayerPrefs.GetString(PlayerNameKeyPrefix + currentPlayerID, ""); 
        int selectedCharacterID = characterManager.selectedOption;
        
        if (string.IsNullOrEmpty(playerName) || characterManager.characterDb == null || selectedCharacterIndices.Contains(selectedCharacterID)) 
        {
            if (string.IsNullOrEmpty(playerName)) Debug.LogWarning("Name is missing. Please confirm name in pop-out first.");
            if (characterManager.characterDb == null) Debug.LogError("Character Database is missing!");
            if (selectedCharacterIndices.Contains(selectedCharacterID)) Debug.LogWarning("Character already chosen.");
            return; 
        } 
        selectedCharacterIndices.Add(selectedCharacterID);
        
        // 🚨 FIX: Use the array on the CharacterManager
        if (selectedCharacterID >= 0 && selectedCharacterID < characterManager.characterDisplays.Length)
        {
            if (characterManager.characterDisplays[selectedCharacterID] != null)
            {
                characterManager.characterDisplays[selectedCharacterID].SetActive(false); 
            }
        }
        Character chosenCharacter = characterManager.characterDb.GetCharacter(selectedCharacterID);
        int slotIndex = currentPlayerID - 1;

        if (slotIndex >= 0 && slotIndex < MAX_PLAYERS)
        { 
            string confirmationText = "P" + currentPlayerID.ToString() + ": " + playerName + " - " + chosenCharacter.characterName;
            
            confirmedNameTextSlots[slotIndex].gameObject.SetActive(true);
            confirmedNameTextSlots[slotIndex].text = confirmationText;
            
            PlayerPrefs.SetInt(SelectedOptionKeyPrefix + currentPlayerID, selectedCharacterID);
            Debug.Log(playerName + " chose character: " + chosenCharacter.characterName);
        }

        currentPlayerID++; 

        if (currentPlayerID > MAX_PLAYERS)
        {
            playerTurnText.text = "All Builders Ready!";
            characterManager.gameObject.SetActive(false);
            if(mainSelectButton != null) mainSelectButton.gameObject.SetActive(false);
            if (startGameButton != null) startGameButton.interactable = true;
        }
        else
        {
            StartNewSelectionRound(); 
        }
    }
    
    public void LoadGameScene(int sceneID)
    {
        if (startGameButton != null && startGameButton.interactable)
        {
            SceneManager.LoadScene(sceneID);
        }
    }
    
    public void UpdateLiveNameDisplay(string currentInput)
    { 
        if (playerTurnText != null && popoutManager.panelObject.activeInHierarchy)
        {
            playerTurnText.text = "Player " + currentPlayerID.ToString() + " is: " + currentInput;
        }
    }
}