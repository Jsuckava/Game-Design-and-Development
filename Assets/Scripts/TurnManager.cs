using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Linq; 

public class TurnManager : MonoBehaviour
{
    [Header("Runtime References")]
    private List<PlayerMovement> players = new List<PlayerMovement>();
    private List<PlayerStats> activePlayers; 
    private List<TextMeshProUGUI> playerListSlots;

    [Header("UI References")]
    public TextMeshProUGUI turnStatusText;
    public Button mainRollButton; 
    public TextMeshProUGUI mainDiceResultText; 

    [Header("Morality UI")]
    public Slider moralitySlider;

    [Header("Community Morale")]
    public Slider communityMoraleSlider;
    public int communityMorale = 0;

    public AbilityManager abilityManager;
    private const int MIN_MORALE = 0;
    private const int MAX_MORALE = 100;

    [Header("Highlight Settings")]
    public Color highlightColor = Color.yellow;
    public Color defaultColor = Color.white;

    private int currentPlayerIndex = 0;

    void Start()
    {
        abilityManager = FindFirstObjectByType<AbilityManager>();
        if (mainRollButton == null)
        {
            mainRollButton = FindFirstObjectByType<Button>();
        }
        if (mainDiceResultText == null)
        {
            mainDiceResultText = GameObject.Find("DiceResultText")?.GetComponent<TextMeshProUGUI>();
        }

        if (communityMoraleSlider != null)
        {
            communityMoraleSlider.minValue = MIN_MORALE;
            communityMoraleSlider.maxValue = MAX_MORALE;
            communityMoraleSlider.value = communityMorale;
        }
    }

    public void Initialize(List<PlayerMovement> allPlayers, List<PlayerStats> allPlayerStats, TextMeshProUGUI[] uiSlots)
    {
        players = allPlayers;
        activePlayers = allPlayerStats; 
        playerListSlots = new List<TextMeshProUGUI>(uiSlots);

        currentPlayerIndex = 0;
        
        if (players.Count == 0 || activePlayers.Count == 0)
        {
            Debug.LogError("TurnManager initialized with no players!");
            return;
        }

        foreach (var player in players)
        {
            if (player != null)
            {
                player.AssignUI(mainRollButton, mainDiceResultText);
            }
        }

        StartTurn(activePlayers[currentPlayerIndex]);
    }

    public void UpdateMoralityVisual(int moralityValue)
    {
        if (moralitySlider != null)
        {
            moralitySlider.value = moralityValue; 
        }
    }

    private void StartTurn(PlayerStats currentPlayer)
    {
        if (activePlayers.Count == 0) 
        {
            Debug.Log("Game Over: No active players left to start a turn.");
            return;
        }

        Debug.Log($"Starting turn for {currentPlayer.playerName} (Player {currentPlayerIndex + 1})");

        if (turnStatusText != null)
        {
            turnStatusText.text = $"{currentPlayer.playerName}'s Turn to Roll!";
        }

        UpdateMoralityVisual(currentPlayer.morality);

        if (abilityManager != null)
        {
            abilityManager.CheckStartOfTurnPassive(currentPlayer);
        }

        for (int i = 0; i < playerListSlots.Count; i++)
        {
            if (playerListSlots[i] == null) continue;

            if (i == currentPlayerIndex) 
            {
                playerListSlots[i].color = highlightColor;
            }
            else
            {
                playerListSlots[i].color = defaultColor;
            }
        }

        PlayerMovement currentMovement = players[currentPlayerIndex];
        currentMovement.AddButtonListener();
        currentMovement.SetRollButtonInteractable(true);
    }

    public void EndTurn()
    {
        if (currentPlayerIndex >= 0 && currentPlayerIndex < players.Count)
        {
            players[currentPlayerIndex].SetRollButtonInteractable(false);
            players[currentPlayerIndex].RemoveButtonListener();
        }

        currentPlayerIndex = (currentPlayerIndex + 1) % activePlayers.Count;
        
        StartTurn(activePlayers[currentPlayerIndex]);
    }

    public void ResumeTurn()
    {
        if (currentPlayerIndex >= 0 && currentPlayerIndex < players.Count)
        {
            players[currentPlayerIndex].SetRollButtonInteractable(true);
        }
    }
    public void ChangeCommunityMorale(int amount)
    {
        communityMorale += amount;
        communityMorale = Mathf.Clamp(communityMorale, MIN_MORALE, MAX_MORALE);

        if (communityMoraleSlider != null)
        {
            communityMoraleSlider.value = communityMorale;
        }
        Debug.Log("New Community Morale: " + communityMorale);
    }
    
    public void EliminatePlayer(PlayerStats eliminatedPlayer)
    {
        Debug.Log($"GAME OVER for {eliminatedPlayer.playerName}!");
        
        int eliminatedIndex = activePlayers.IndexOf(eliminatedPlayer);

        PlayerMovement eliminatedMovement = eliminatedPlayer.GetComponent<PlayerMovement>();
        if (eliminatedMovement != null)
        {
            players.Remove(eliminatedMovement);
        }
        
        activePlayers.Remove(eliminatedPlayer);
        eliminatedPlayer.gameObject.SetActive(false);

        if (activePlayers.Count <= 1)
        {
            Debug.Log("GAME END: Only one player remaining.");
            return;
        }
        
        if (eliminatedIndex < currentPlayerIndex)
        {
            currentPlayerIndex--;
        }
        if (currentPlayerIndex >= activePlayers.Count)
        {
            currentPlayerIndex = 0;
        }
        StartTurn(activePlayers[currentPlayerIndex]);
    }

    public List<PlayerStats> GetAllActivePlayers()
    {
        return activePlayers;
    }

    public PlayerStats GetCurrentActivePlayer()
    {
        if (activePlayers != null && activePlayers.Count > 0)
        {
            return activePlayers[currentPlayerIndex];
        }
        return null;
    }
}