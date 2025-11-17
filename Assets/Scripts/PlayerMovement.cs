using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; 
using UnityEngine.UI; 

public class PlayerMovement : MonoBehaviour
{
    public Transform[] waypoints; 
    public DisasterManager disasterManager;
    
    public Button rollButton; 
    public TextMeshProUGUI diceResultText; 

    private int currentWaypointIndex = 0;
    public float moveSpeed = 5f; 
    private bool isMoving = false;
    private TurnManager turnManager; 
    private PopupController popupController;
    private PunishmentDeck punishmentDeck;
    private BuildTileController buildTileController;

    void Start()
    {
        turnManager = FindFirstObjectByType<TurnManager>(); 
        disasterManager = FindFirstObjectByType<DisasterManager>();
        popupController = FindFirstObjectByType<PopupController>();
        punishmentDeck = FindFirstObjectByType<PunishmentDeck>();
        buildTileController = FindFirstObjectByType<BuildTileController>();
    }

    public void AssignUI(Button button, TextMeshProUGUI text)
    {
        this.rollButton = button;
        this.diceResultText = text;
    }
    
    public void AddButtonListener()
    {
        if (rollButton != null)
        {
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

        int diceRoll = Random.Range(1, 7);
        
        if (diceResultText != null)
        {
            diceResultText.text = "You rolled a " + diceRoll;
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
        
        bool eventHappened = TriggerTileEvent(waypoints[currentWaypointIndex]);

        if (!eventHappened)
        {
            if (turnManager != null)
            {
                turnManager.EndTurn();
            }
        }
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
            if (buildTileController != null)
            {
                buildTileController.StartBuilding(this.gameObject); 
                return true; 
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