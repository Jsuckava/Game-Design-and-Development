using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Timer : MonoBehaviour
{
    public float gameDuration = 5.0f;
    private float timeRemaining;
    private bool isTimerRunning = false;
    
    // public Scene mainMenuScene;

    public TextMeshProUGUI timerText;
    // // public float blinkTime = 5.0f;
    public bool isRed = false;
    public GameObject endGamePanel;
    public TextMeshProUGUI gameOverText;

    public BahayKuboTracker bahayKuboTracker;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeRemaining = gameDuration * 60f; // Convert minutes to seconds
        isTimerRunning = true;

        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }

        if (bahayKuboTracker == null)
        {
            bahayKuboTracker = FindFirstObjectByType<BahayKuboTracker>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerRunning)
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

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(0.5f);
    }

    void UpdateTimerDisplay()
    {
        float minutes = Mathf.FloorToInt(timeRemaining / 60f);
        float seconds = Mathf.FloorToInt(timeRemaining % 60f);
        if (timerText != null)
        {
            timerText.text = "" + string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (timeRemaining <= 30f && !isRed)
        {
            timerText.color = Color.red;
            Cooldown();
            isRed = true;
        }
        else
        {
            timerText.color = Color.white;
            Cooldown();
            isRed = false;
        }
    }

    void CheckIfGameWon()
    {
        if (bahayKuboTracker != null)
        {
            if (bahayKuboTracker.haligiBuilt && 
                bahayKuboTracker.sahigBuilt && 
                bahayKuboTracker.paderBuilt && 
                bahayKuboTracker.bubongBuilt)
            {
                isTimerRunning = false;
                EndGame(true);
            }
        }
    }

    void EndGame(bool hasWon)
    {
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
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

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }
}
