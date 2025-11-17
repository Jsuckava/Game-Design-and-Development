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

    void Start()
    {
        // popupContainer = FindFirstObjectByType<PopupController>();
    }

    public void StartXoxMinigame(PlayerStats player)
    {
        currentPlayerStats = player;
        Instantiate(xoxMinigamePrefab, popupContainer);
    }

    public void StartBugtongMinigame(PlayerStats player)
    {
        currentPlayerStats = player;
        Instantiate(bugtongMinigamePrefab, popupContainer);
    }

    public void StartMatchaPickerMinigame(PlayerStats player)
    {
        currentPlayerStats = player;
        Instantiate(matchaPickerMinigamePrefab, popupContainer);
    }

    public void OnMinigameCompleted(bool isWin)
    {
        if (isWin)
        {
            Debug.Log("Player won.");
        }

        if (popupContainer.childCount > 0)
        {
            Destroy(popupContainer.GetChild(0).gameObject);
        }
    }
}
