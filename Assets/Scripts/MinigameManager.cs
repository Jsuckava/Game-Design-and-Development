using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [Header("Minigame Prefabs")]
    public GameObject bugtongMinigamePrefab;
    public GameObject xoxMinigamePrefab;
    public GameObject matchaPickerMinigamePrefab;

    [Header("Popup Container")]
    public Transform popupContainer;

    public AbilityManager abilityManager;
    public string minigamePlaying = null;
    
    private PlayerStats currentPlayerStats;
    private TurnManager turnManager;
    private PopupController popupController;

    void Start()
    {
        // 1. Safety Check on Start
        if (Timer.IsGameOver) 
        {
            gameObject.SetActive(false);
            return;
        }

        turnManager = FindFirstObjectByType<TurnManager>();
        popupController = FindFirstObjectByType<PopupController>();
        abilityManager = FindAnyObjectByType<AbilityManager>();
    }

    public void StartXoxMinigame(PlayerStats player)
    {
        if (Timer.IsGameOver) return;

        popupContainer.gameObject.SetActive(true);
        currentPlayerStats = player;
        Instantiate(xoxMinigamePrefab, popupContainer);
        minigamePlaying = "Xox";
    }

    public void StartBugtongMinigame(PlayerStats player)
    {
        if (Timer.IsGameOver) return;

        popupContainer.gameObject.SetActive(true);
        currentPlayerStats = player;
        Instantiate(bugtongMinigamePrefab, popupContainer);
        minigamePlaying = "Bugtong";
    }

    public void StartMatchaPickerMinigame(PlayerStats player)
    {
        if (Timer.IsGameOver) return;

        popupContainer.gameObject.SetActive(true);
        currentPlayerStats = player;
        Instantiate(matchaPickerMinigamePrefab, popupContainer);
        minigamePlaying = "Matching";
    }

    public void OnMinigameCompleted(bool isWin)
    {
        if (Timer.IsGameOver)
        {
            if (popupContainer.childCount > 0)
            {
                Destroy(popupContainer.GetChild(0).gameObject);
            }
            popupContainer.gameObject.SetActive(false);
            return; 
        }

        if (isWin)
        {
            Debug.Log("Player won");
            if (minigamePlaying == "Bugtong")
            {
                abilityManager.CheckMinigameBonus(currentPlayerStats, "Bugtong");
            }
        }

        if (popupContainer.childCount > 0)
        {
            Destroy(popupContainer.GetChild(0).gameObject);
        }
        
        popupContainer.gameObject.SetActive(false);

        if (popupController != null)
        {
            if (popupController.uiAudioSource != null)
            {
                popupController.uiAudioSource.Stop();
            }

            if (popupController.mainBgMusic != null)
            {
                popupController.mainBgMusic.UnPause();
            }
        }

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
                popupController.ShowPunishmentChoice("Aguy! ", currentPlayerStats);
            }
        }
        else
        {
            turnManager.EndTurn();
        }

        currentPlayerStats = null;
    }
}