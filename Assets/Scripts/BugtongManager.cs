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
	
	[Header("Ui")]
	public TextMeshProUGUI bugtongText;
	public TextMeshProUGUI timerText;
	public TextMeshProUGUI resultText;
	public TMP_InputField answerInput;
	public Button submitButton;
	public GameObject gamePanel;
	
	[Header("Game Settings")]
	public float timeToAnswer = 30f;
	private float currentTime;
	private bool isGameActive = false;
	
	private List<Bugtong> bugtongList = new List<Bugtong>();
	private Bugtong currentBugtong;
	private MinigameManager minigameManager;
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        minigameManager = FindFirstObjectByType<MinigameManager>();
        if (minigameManager == null)
        {
			Debug.Log("BugtongManager could not find MinigameManager");
		}
		
		FindPanels();
		
		InitializeBugtong();
		StartNewGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameActive) return;
        
        currentTime -= Time.deltaTime;
        timerText.text = Mathf.CeilToInt(currentTime).ToString();
        
        if (currentTime <= 0)
        {
			GameOver(false, "Time's up!'");
		}
    }
    
    void FindPanels()
    {
		Transform foundGamePanel = transform.Find("MinigamePanel");
		if (foundGamePanel != null)
		{
			gamePanel = foundGamePanel.gameObject;
		}
		else
		{
			Debug.Log("BugtongManager could not find a GamePanel");
		}
	}
    
    void OnSubmitClicked()
    {
		string playerAnswer = answerInput.text.Trim().ToLower();
		
		if (currentBugtong.validAnswers.Contains(playerAnswer))
		{
			GameOver(true, "Your answer is correct!");
		}
		else
		{
			GameOver(false, "Your answer is wrong!");
		}
	}
    
    void GameOver(bool didPlayerWin, string message)
    {
		isGameActive = false;
		
		//~ gamePanel.gameObject.SetActive(false);
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
			"Bugtong 1",
			new List<string> { "answer1", "answer2"}
		));
		bugtongList.Add(new Bugtong(
			"Bugtong 2",
			new List<string> { "answer1", "answer2"}
		));
	}
	
	void StartNewGame()
	{
		isGameActive = true;
		currentTime = timeToAnswer;
		
		gamePanel.gameObject.SetActive(true);
		resultText.gameObject.SetActive(false);
		
		answerInput.interactable = true;
		submitButton.interactable = true;
		answerInput.text = "";
		
		currentBugtong = bugtongList[Random.Range(0, bugtongList.Count)];
		bugtongText.text = currentBugtong.bugtongText;
		
		submitButton.onClick.RemoveAllListeners();
		submitButton.onClick.AddListener(OnSubmitClicked);
	}
}
