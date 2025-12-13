using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; 
using UnityEngine.UI; 

public class PlayerMovement : MonoBehaviour
{
    [Header("Waypoints & Managers")]
    public Transform[] waypoints; 
    public DisasterManager disasterManager;
    
    [Header("Turn UI (Assigned by TurnManager)")]
    public Button rollButton; 
    public TextMeshProUGUI diceResultText; 
    public Button buildHudButton; 

    [Header("Movement Settings")]
    private int currentWaypointIndex = 0;
    public float moveSpeed = 5f; 
    private bool isMoving = false;
    
    private TurnManager turnManager; 
    private PopupController popupController;
    private PunishmentDeck punishmentDeck;
    private BuildTileController buildTileController;
    private BahayKuboTracker bahayKuboTracker;

    void Start()
    {
        turnManager = FindFirstObjectByType<TurnManager>(); 
        disasterManager = FindFirstObjectByType<DisasterManager>();
        popupController = FindFirstObjectByType<PopupController>();
        punishmentDeck = FindFirstObjectByType<PunishmentDeck>();
        buildTileController = FindFirstObjectByType<BuildTileController>();
        bahayKuboTracker = FindFirstObjectByType<BahayKuboTracker>();
        if (buildHudButton == null) 
        {
            GameObject btn = GameObject.Find("BuildButton");
            if (btn != null) buildHudButton = btn.GetComponent<Button>();
        }

        // Always hide at start
        if (buildHudButton != null) 
            buildHudButton.gameObject.SetActive(false);
    }

    public void AssignUI(Button rollBtn, TextMeshProUGUI text, Button buildBtn)
    {
        this.rollButton = rollBtn;
        this.diceResultText = text;
        
        if (buildBtn != null)
        {
            this.buildHudButton = buildBtn; 
        }
        
        if (this.buildHudButton != null)
             this.buildHudButton.gameObject.SetActive(false);
    }
    
    public void AddButtonListener()
    {
        if (rollButton != null)
        {
            rollButton.onClick.RemoveAllListeners(); 
            rollButton.onClick.AddListener(RollAndMove);
        }
    }

    public void RemoveButtonListener()
    {
        if (rollButton != null)
        {
            rollButton.onClick.RemoveListener(RollAndMove);
        }
    }

    public void SetRollButtonInteractable(bool interactable)
    {
        if (rollButton != null)
        {
            rollButton.interactable = interactable;
        }
    }
    
    public void RollAndMove()
    {
        if (isMoving) return; 

        if (buildHudButton != null) 
            buildHudButton.gameObject.SetActive(false);

        int diceRoll = UnityEngine.Random.Range(1, 7);
        
        if (diceResultText != null)
        {
            diceResultText.text = "You rolled a " + diceRoll;
        }

        PlayerStats stats = GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.hasRolledDice = true;
        }
        
        SetRollButtonInteractable(false);
        StartCoroutine(MovePlayer(diceRoll));
    }

    private IEnumerator MovePlayer(int steps)
    {
        isMoving = true;

        for (int i = 0; i < steps; i++)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0; 
            }

            Transform targetWaypoint = waypoints[currentWaypointIndex];
            while (Vector3.Distance(transform.position, targetWaypoint.position) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);
                yield return null; 
            }
            
            transform.position = targetWaypoint.position; 
            yield return new WaitForSeconds(0.1f);
        }

        isMoving = false;
        StartCoroutine(FinalizeTurnAfterEvent());
    }

    private IEnumerator FinalizeTurnAfterEvent()
    {
        yield return null; 
        yield return new WaitForSeconds(0.05f); 

        bool eventHappened = TriggerTileEvent(waypoints[currentWaypointIndex]);

        if (!eventHappened) 
        {
            FinishTurnExecution();
        }
    }
    
    public void FinishTurnExecution()
    {
        if (buildHudButton != null) 
            buildHudButton.gameObject.SetActive(false);

        if (turnManager != null)
        {
            turnManager.EndTurn();
        }
    }
    private bool CheckIfTeamCanBuild(out string missingInfo)
    {
        missingInfo = "";
        if (bahayKuboTracker == null || turnManager == null) return false;

        int teamBamboo = 0;
        int teamHardwood = 0;
        int teamSawali = 0;
        int teamNipa = 0;

        foreach (var p in turnManager.GetAllActivePlayers())
        {
            if (p != null)
            {
                teamBamboo += p.bamboo;
                teamHardwood += p.hardwood;
                teamSawali += p.sawali;
                teamNipa += p.nipaLeaves;
            }
        }
        if (!bahayKuboTracker.haligiBuilt)
        {
            if (teamBamboo >= bahayKuboTracker.haligi_bambooCost) return true;
            missingInfo = $"Need {bahayKuboTracker.haligi_bambooCost - teamBamboo} more Bamboo for Haligi.";
            return false;
        }
        else if (!bahayKuboTracker.sahigBuilt)
        {
            if (teamHardwood >= bahayKuboTracker.sahig_hardwoodCost) return true;
            missingInfo = $"Need {bahayKuboTracker.sahig_hardwoodCost - teamHardwood} more Hardwood for Sahig.";
            return false;
        }
        else if (!bahayKuboTracker.paderBuilt)
        {
            bool hasSawali = teamSawali >= bahayKuboTracker.pader_sawaliCost;
            bool hasBamboo = teamBamboo >= bahayKuboTracker.pader_bambooCost;
            
            if (hasSawali && hasBamboo) return true;
            
            missingInfo = "Need: ";
            if (!hasSawali) missingInfo += $"{bahayKuboTracker.pader_sawaliCost - teamSawali} Sawali ";
            if (!hasBamboo) missingInfo += $"{bahayKuboTracker.pader_bambooCost - teamBamboo} Bamboo ";
            missingInfo += "for Pader.";
            return false;
        }
        else if (!bahayKuboTracker.bubongBuilt)
        {
            bool hasNipa = teamNipa >= bahayKuboTracker.bubong_nipaCost;
            bool hasBamboo = teamBamboo >= bahayKuboTracker.bubong_bambooCost;

            if (hasNipa && hasBamboo) return true;

            missingInfo = "Need: ";
            if (!hasNipa) missingInfo += $"{bahayKuboTracker.bubong_nipaCost - teamNipa} Nipa ";
            if (!hasBamboo) missingInfo += $"{bahayKuboTracker.bubong_bambooCost - teamBamboo} Bamboo ";
            missingInfo += "for Bubong.";
            return false;
        }

        missingInfo = "Bahay Kubo is fully built!";
        return false;
    }

    private bool TriggerTileEvent(Transform tile)
    {
        PlayerStats stats = GetComponent<PlayerStats>();
        if (stats == null) return false;

        if (tile.CompareTag("SafeZoneTile"))
        {
            return false; 
        }
        else if (tile.CompareTag("BuildTile"))
        {
            string missingMsg = "";
            
            bool canBuild = CheckIfTeamCanBuild(out missingMsg);

            if (canBuild)
            {
                if (buildTileController != null && buildHudButton != null)
                {
                    buildHudButton.gameObject.SetActive(true);
                    
                    buildHudButton.onClick.RemoveAllListeners();
                    buildHudButton.onClick.AddListener(() => 
                    {
                        if (popupController != null)
                        {
                            popupController.ShowBuildPopup(stats);
                        }
                        buildHudButton.gameObject.SetActive(false);
                    });
                    return true; 
                }
            }
            else
            {
                if (popupController != null)
                {
                    popupController.ShowSimplePopup("Not Enough Materials", $"You cannot build yet.\n\n{missingMsg}", stats);
                    return true; 
                }
            }
        }
        else if (tile.CompareTag("DisasterTile"))
        {
            if (popupController != null)
            {
                popupController.ShowPunishmentChoice("A disaster strikes! Choose your fate:", stats);
                return true; 
            }
        }
        else if (tile.CompareTag("MiniGameTile"))
        {
            if (popupController != null)
            {
                popupController.ShowMiniGamePopup("Choose a Larong Pinoy!", stats);
                return true; 
            }
        }
        else if (tile.CompareTag("WildTile"))
        {
            if (popupController != null)
            {
                popupController.ShowMaterialChoice("You found some materials! Choose a card:", stats);
                return true;
            }
        }
        
        return false;
    }
}