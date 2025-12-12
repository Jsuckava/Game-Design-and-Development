using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq; 

public class GameSceneManager : MonoBehaviour
{
    [Header("Asset References")]
    public CharacterDb characterDb;
    public GameObject playerPawnPrefab;

    [Header("Scene References")]
    public Transform[] spawnPoints; 
    public DisasterManager disasterManager;
    public Transform waypointsParent; 
    public TurnManager turnManager; 
    
    [Header("UI References")]
    public TextMeshProUGUI[] playerListSlots = new TextMeshProUGUI[4];

    [Header("Runtime Player List")]
    public List<PlayerStats> activePlayers = new List<PlayerStats>();
    private List<PlayerMovement> playerMovements = new List<PlayerMovement>();   
    private Transform[] allWaypoints;
    private const string SelectedOptionKeyPrefix = "Player_"; 
    private const string PlayerNameKeyPrefix = "PlayerName_"; 

    void Start()
    {
        if (turnManager == null) turnManager = FindFirstObjectByType<TurnManager>();

        if (waypointsParent != null)
        {
            allWaypoints = new Transform[waypointsParent.childCount];
            for (int i = 0; i < waypointsParent.childCount; i++)
            {
                allWaypoints[i] = waypointsParent.GetChild(i);
            }
        }
        else
        {
            Debug.LogError("Waypoints Parent is not assigned in GameSceneManager!");
            return;
        }

        SpawnAllPlayers(); 

        if (disasterManager != null) disasterManager.allActivePlayers = activePlayers;
        
        if (turnManager != null)
        {
            turnManager.Initialize(playerMovements, activePlayers, playerListSlots);
        }
    }

    private void SpawnAllPlayers()
    {
        Debug.Log("Spawning 4 players...");

        for (int i = 1; i <= 4; i++)
        {
            if (spawnPoints == null || i > spawnPoints.Length)
            {
                Debug.LogError($"CRITICAL: Missing Spawn Point for Player {i}. Check 'Spawn Points' array in GameSceneManager Inspector.");
                continue;
            }

            int selectedCharacterID = PlayerPrefs.GetInt(SelectedOptionKeyPrefix + i, 0);
            string playerName = PlayerPrefs.GetString(PlayerNameKeyPrefix + i, "Player " + i);
            
            Character characterData = characterDb.GetCharacter(selectedCharacterID); 
            
            GameObject playerObject = Instantiate(playerPawnPrefab, spawnPoints[i-1].position, Quaternion.identity); 
            playerObject.name = $"P{i} - {playerName} ({characterData?.characterName ?? "Default"})"; 
            
            PlayerStats stats = playerObject.GetComponent<PlayerStats>();
            if (stats == null) stats = playerObject.AddComponent<PlayerStats>();

            if (stats != null)
            {
                if (i <= playerListSlots.Length)
                {
                    stats.myStatDisplay = playerListSlots[i - 1]; 
                }
                
                try
                {
                    stats.Initialize(playerName, characterData.characterName, selectedCharacterID, characterData.Energy, characterData.specialAbilityPower, characterData.characterSprite, i);
                }
                catch (System.NullReferenceException)
                {
                    Debug.LogWarning($"Player {i} failed to load character data. Using default stats.");
                    stats.Initialize(playerName, "Default Character", selectedCharacterID, 30f, AbilityType.None, null, i);
                }
                
                activePlayers.Add(stats);
            }
            
            PlayerMovement movement = playerObject.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.waypoints = allWaypoints;
                playerMovements.Add(movement); 
            }
        }
        Debug.Log($"FINAL ACTIVE PLAYER COUNT: {activePlayers.Count}"); 
    }
}