using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [Header("Minigame Prefabs")]
    public GameObject bugtongMinigamePrefab;
    public GameObject xoxMinigamePrefab;
    public GameObject matchaPickerMinigamePrefab;

    [Header("Popup Container")]
    public Transform popupContainer;
    
    private PlayerStats currentPlayerStats;
    private TurnManager turnManager;
    private PopupController popupController;

    void Start()
    {
        turnManager = FindFirstObjectByType<TurnManager>();
        popupController = FindFirstObjectByType<PopupController>();
    }

    public void StartXoxMinigame(PlayerStats player)
    {
        popupContainer.gameObject.SetActive(true);
        currentPlayerStats = player;
        Instantiate(xoxMinigamePrefab, popupContainer);
    }

    public void StartBugtongMinigame(PlayerStats player)
    {
        popupContainer.gameObject.SetActive(true);
        currentPlayerStats = player;
        Instantiate(bugtongMinigamePrefab, popupContainer);
    }

    public void StartMatchaPickerMinigame(PlayerStats player)
    {
        popupContainer.gameObject.SetActive(true);
        currentPlayerStats = player;
        Instantiate(matchaPickerMinigamePrefab, popupContainer);
    }

    public void OnMinigameCompleted(bool isWin)
    {
        if (popupContainer.childCount > 0)
        {
            Destroy(popupContainer.GetChild(0).gameObject);
        }
        
        // 1. WIN CONDITION: Transfer control to the PopupController and exit.
        if (isWin)
        {
            Debug.Log("Player won.");
            if (popupController != null && currentPlayerStats != null)
            {
                popupContainer.gameObject.SetActive(false);
                // The PopupController will call EndTurn() when the reward popup is closed.
                popupController.ShowRewardChoice("Minigame Win! Choose Your Reward:", currentPlayerStats);
                return; // CRITICAL: Stop the rest of the method from running.
            }
        }
        
        // 2. LOSS CONDITION: Just clean up and ensure turn advances via the PopupController's standard path.
        
        Debug.Log("You Lose.");
        popupContainer.gameObject.SetActive(false); 
        
        if (turnManager != null)
        {
            turnManager.EndTurn();
        }
        else
        {
            Debug.Log("MinigameManager could not find TurnManager to end the turn");
        }
        currentPlayerStats = null;
    }
}