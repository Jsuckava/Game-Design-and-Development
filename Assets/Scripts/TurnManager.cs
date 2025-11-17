using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    [Header("Runtime References")]
    public List<PlayerMovement> players = new List<PlayerMovement>();
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
    public int communityMorale = 50;
    private int minMorale = 0;
    private int maxMorale = 100;

    [Header("Highlight Settings")]
    public Color highlightColor = Color.yellow;
    public Color defaultColor = Color.white;

    private int currentPlayerIndex = 0;

    void Start()
    {
        if (mainRollButton == null)
        {
            mainRollButton = GameObject.Find("RollButton").GetComponent<Button>();
        }
        if (mainDiceResultText == null)
        {
            mainDiceResultText = GameObject.Find("DiceResultText").GetComponent<TextMeshProUGUI>();
        }

        if (communityMoraleSlider != null)
        {
            communityMoraleSlider.value = communityMorale;
        }
    }

    public void Initialize(List<PlayerMovement> allPlayers, List<PlayerStats> allPlayerStats, TextMeshProUGUI[] uiSlots)
    {
        players = allPlayers;
        activePlayers = allPlayerStats; 
        playerListSlots = new List<TextMeshProUGUI>(uiSlots);

        currentPlayerIndex = 0;

        foreach (var player in players)
        {
            player.AssignUI(mainRollButton, mainDiceResultText);
        }

        StartTurn(activePlayers[currentPlayerIndex].playerName);
    }

    public void UpdateMoralityVisual(int moralityValue)
    {
        if (moralitySlider != null)
        {
            moralitySlider.value = moralityValue;
        }
    }

    private void StartTurn(string playerName)
    {
        Debug.Log($"Starting turn for {playerName} (Player {currentPlayerIndex + 1})");

        if (turnStatusText != null)
        {
            turnStatusText.text = $"{playerName}'s Turn to Roll!";
        }

        PlayerStats stats = activePlayers[currentPlayerIndex].GetComponent<PlayerStats>();
        if (stats != null)
        {
            UpdateMoralityVisual(stats.morality);
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

        players[currentPlayerIndex].AddButtonListener();
        players[currentPlayerIndex].SetRollButtonInteractable(true);
    }

    public void EndTurn()
    {
        if (currentPlayerIndex < activePlayers.Count)
        {
            players[currentPlayerIndex].SetRollButtonInteractable(false);
            players[currentPlayerIndex].RemoveButtonListener();
        }

        currentPlayerIndex++;
        if (currentPlayerIndex >= activePlayers.Count)
        {
            currentPlayerIndex = 0;
        }

        string nextPlayerName = activePlayers[currentPlayerIndex].playerName;
        StartTurn(nextPlayerName);
    }

    public void ChangeCommunityMorale(int amount)
    {
        communityMorale += amount;
        communityMorale = Mathf.Clamp(communityMorale, minMorale, maxMorale);

        if (communityMoraleSlider != null)
        {
            communityMoraleSlider.value = communityMorale;
        }
        Debug.Log("New Community Morale: " + communityMorale);
    }
    
    public void EliminatePlayer(PlayerStats eliminatedPlayer)
    {
        Debug.Log($"GAME OVER for {eliminatedPlayer.playerName}!");
        
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
        }
        if (currentPlayerIndex >= activePlayers.Count)
        {
            currentPlayerIndex = 0;
        }
        EndTurn(); 
    }
}