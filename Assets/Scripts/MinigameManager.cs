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

    void Start()
    {
		turnManager = FindFirstObjectByType<TurnManager>();
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
        if (isWin)
        {
            Debug.Log("Player won.");
        }
        else
        {
			Debug.Log("You Lose.");
		}

        if (popupContainer.childCount > 0)
        {
            Destroy(popupContainer.GetChild(0).gameObject);
        }
        
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
