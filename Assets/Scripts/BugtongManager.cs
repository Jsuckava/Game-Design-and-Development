using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BugtongManager : MonoBehaviour
{
    private class Bugtong
    {
        public string bugtongText;
        public List<string> validAnswers;
        
        public Bugtong(string text, List<string> answers)
        {
            this.bugtongText = text;
            this.validAnswers = answers.Select(a => a.Trim().ToLower()).ToList();
        }
    }
    
    [Header("UI")]
    public TextMeshProUGUI bugtongText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultText;
    public TMP_InputField answerInput;
    public Button submitButton;
    
    [Header("Game Settings")]
    public float timeToAnswer = 30f;
    private float currentTime;
    private bool isGameActive = false;
    
    private List<Bugtong> bugtongList = new List<Bugtong>();
    private Bugtong currentBugtong;
    private MinigameManager minigameManager;
    
    void Start()
    {
        minigameManager = FindFirstObjectByType<MinigameManager>();
        if (minigameManager == null)
        {
            Debug.LogError("BugtongManager could not find MinigameManager!");
        }
        if (submitButton == null || answerInput == null || timerText == null)
        {
            Debug.LogError("BUGTONG MANAGER ERROR: Critical UI elements not assigned in Inspector.");
            return;
        }
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(OnSubmitClicked);
        
        InitializeBugtong();
        StartNewGame();
    }

    void Update()
    {
        if (!isGameActive) return;
        
        currentTime -= Time.deltaTime;
        timerText.text = Mathf.CeilToInt(currentTime).ToString();
        
        if (currentTime <= 0)
        {
            GameOver(false, "Time's up! You lose.");
        }
    }
    void OnSubmitClicked()
    {
        string playerAnswer = answerInput.text.Trim().ToLower();
       
        if (currentBugtong == null) return;
        
        if (currentBugtong.validAnswers.Contains(playerAnswer))
        {
            GameOver(true, "Your answer is correct! You win!");
        }
        else
        {
            GameOver(false, "Your answer is wrong! You lose.");
        }
    }
    
    void GameOver(bool didPlayerWin, string message)
    {
        isGameActive = false;  
        resultText.gameObject.SetActive(true);
        resultText.text = message;
        
        answerInput.interactable = false;
        submitButton.interactable = false;
        
        StartCoroutine(CloseMinigameAfterDelay(didPlayerWin));
    }
    
    IEnumerator CloseMinigameAfterDelay(bool didPlayerWin)
    {
        yield return new WaitForSeconds(2f);
        
        if (minigameManager != null)
        {
            minigameManager.OnMinigameCompleted(didPlayerWin);
        }
    }
    
    void InitializeBugtong()
    {
        bugtongList.Add(new Bugtong(
            "Aling hayop ang may sungay na tinutubuan ng dahon?",
            new List<string> { "deer", "usa"}
        ));
        bugtongList.Add(new Bugtong(
            "Anong isda ang umaakyat sa puno?",
            new List<string> { "dalag", "mudfish"}
        ));
        bugtongList.Add(new Bugtong(
            "May ulo walang mukha, may leeg walang katawan.",
            new List<string> { "bote", "bottle"}
        ));
    }
    
    void StartNewGame()
    {
        isGameActive = true;
        currentTime = timeToAnswer;
        resultText.gameObject.SetActive(false);
        
        answerInput.interactable = true;
        submitButton.interactable = true;
        answerInput.text = "";
        currentBugtong = bugtongList[Random.Range(0, bugtongList.Count)];
        bugtongText.text = currentBugtong.bugtongText;
    }
}