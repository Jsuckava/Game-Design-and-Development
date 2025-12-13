using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Timer : MonoBehaviour
{
    public static bool IsGameOver = false; 

    public float gameDuration = 5.0f;
    private float timeRemaining;
    private bool isTimerRunning = false;
    public TextMeshProUGUI timerText;
    public bool isRed = false;

    [Header("UI Panels")]
    public GameObject endGamePanel;
    public TextMeshProUGUI gameOverText;

    [Header("Audio Settings")]
    public AudioSource bgmSource;      
    public AudioClip gameWinSound;
    public AudioClip gameLoseSound;    

    public BahayKuboTracker bahayKuboTracker;

    void Start()
    {
        IsGameOver = false; 
        timeRemaining = gameDuration * 60f; 
        isTimerRunning = true;

        if (endGamePanel != null) endGamePanel.SetActive(false);
        if (bahayKuboTracker == null) bahayKuboTracker = FindFirstObjectByType<BahayKuboTracker>();
    }

    void Update()
    {
        if (isTimerRunning && !IsGameOver)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();
                CheckIfGameWon();
            }
            else
            {
                timeRemaining = 0;
                isTimerRunning = false;
                UpdateTimerDisplay();
                EndGame(false);
            }
        }
    }

    void UpdateTimerDisplay()
    {
        float minutes = Mathf.FloorToInt(timeRemaining / 60f);
        float seconds = Mathf.FloorToInt(timeRemaining % 60f);
        if (timerText != null) timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timeRemaining <= 30f && !isRed)
        {
            if(timerText != null) timerText.color = Color.red;
            isRed = true;
        }
        else if (timeRemaining > 30f)
        {
            if(timerText != null) timerText.color = Color.white;
            isRed = false;
        }
    }

    void CheckIfGameWon()
    {
        if (bahayKuboTracker != null && !IsGameOver)
        {
            if (bahayKuboTracker.haligiBuilt && bahayKuboTracker.sahigBuilt && 
                bahayKuboTracker.paderBuilt && bahayKuboTracker.bubongBuilt)
            {
                isTimerRunning = false;
                EndGame(true);
            }
        }
    }

    void EndGame(bool hasWon)
    {
        if (IsGameOver) return; 
        IsGameOver = true;      

        if (bgmSource != null)
        {
            bgmSource.Stop();
        }

        if (hasWon && gameWinSound != null)
        {
            AudioSource.PlayClipAtPoint(gameWinSound, Camera.main.transform.position, 1.0f);
        }
        else if (!hasWon && gameLoseSound != null)
        {
            AudioSource.PlayClipAtPoint(gameLoseSound, Camera.main.transform.position, 1.0f);
        }

        var popupController = FindFirstObjectByType<PopupController>();
        if (popupController != null)
        {
            if (popupController.buildPopupPanel != null) popupController.buildPopupPanel.SetActive(false);
            if (popupController.popupPanel != null) popupController.popupPanel.SetActive(false);
            if (popupController.progressPopupPanel != null) popupController.progressPopupPanel.SetActive(false);
            if (popupController.inventoryPanel != null) popupController.inventoryPanel.SetActive(false);
            
            popupController.StopAllCoroutines();
            popupController.gameObject.SetActive(false); 
        }

        var minigameManager = FindFirstObjectByType<MinigameManager>();
        if (minigameManager != null)
        {
            minigameManager.StopAllCoroutines();
            minigameManager.gameObject.SetActive(false);
        }

        var bugtongManager = FindFirstObjectByType<BugtongManager>();
        if (bugtongManager != null)
        {
            bugtongManager.StopAllCoroutines();
            bugtongManager.gameObject.SetActive(false);
        }

        var playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement != null && playerMovement.buildHudButton != null)
        {
            playerMovement.buildHudButton.gameObject.SetActive(false);
        }

        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
            
            endGamePanel.transform.SetAsLastSibling(); 

            Canvas panelCanvas = endGamePanel.GetComponent<Canvas>();
            if (panelCanvas == null) panelCanvas = endGamePanel.AddComponent<Canvas>();
            panelCanvas.overrideSorting = true;
            panelCanvas.sortingOrder = 999; 
            
            if (endGamePanel.GetComponent<GraphicRaycaster>() == null)
                endGamePanel.AddComponent<GraphicRaycaster>();

            if (gameOverText != null)
            {
                if (hasWon)
                {
                    gameOverText.text = "Congrats mga ya, nakabuo na kayo ng bahay kubo!";
                    gameOverText.fontSize = 40;
                }
                else
                {
                    gameOverText.text = "Tama na kayo mga ya, wala nang oras!";
                    gameOverText.fontSize = 55;
                }
            }
        }
    }

    public void RestartGame() { SceneManager.LoadScene(1); }
    public void QuitGame() { SceneManager.LoadScene(0); }
}