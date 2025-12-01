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
        
        popupContainer.gameObject.SetActive(false);

        if (popupController != null && currentPlayerStats != null)
        {
            if (isWin)
            {
                Debug.Log("Player won.");
                popupController.ShowRewardChoice("Minigame Win! Choose Your Reward:", currentPlayerStats);
            }
            else
            {
                Debug.Log("You Lose.");
                popupController.ShowPunishmentChoice("Aguy! ",currentPlayerStats);
            }
        }
        else
        {
             turnManager.EndTurn();
        }

        currentPlayerStats = null;
    }
}