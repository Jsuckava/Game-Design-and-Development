using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Hierarchy;
using UnityEngine.WSA;

public class MatchaPickHandler : MonoBehaviour
{
    [Header("Game Settings")]
    public Sprite cardBackSprite;
    public Sprite[] cardFaceSprite;
    public float timeLimit = 60f;

    [Header("UI Reference")]
    public Transform gridContainer;
    public GameObject cardButtonPrefab;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultText;

    private MinigameManager minigameManager;
    private List<Button> cardButtons = new List<Button>();
    public List<Sprite> shuffledFaces = new List<Sprite>();

    private int firstCardIndex = -1;
    private int secondCardIndex = -1;
    private bool isInputLocked = false;
    private int pairsFound = 0;
    private int totalPairs = 0;

    private float currentTime;
    private bool isGameActive = false;

    private void Start()
    {
        minigameManager = FindFirstObjectByType<MinigameManager>();
        if (gridContainer == null)
        {
            Debug.LogError("CRITICAL ERROR: 'Grid Container' is empty in Inspector!");
            return;
        }
        if (cardButtonPrefab == null)
        {
            Debug.LogError("CRITICAL ERROR: 'Card Button Prefab' is empty in Inspector!");
            return;
        }
        if (cardFaceSprite == null || cardFaceSprite.Length == 0)
        {
            Debug.LogError("CRITICAL ERROR: 'Card Face Sprites' list is empty!");
            return;
        }
        SetUpGame();
    }

    private void Update()
    {
        if (!isGameActive) return;

        currentTime -= Time.deltaTime;
        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(currentTime).ToString();
        }

        if (currentTime < 0) GameOver(false);
    }

    void SetUpGame()
    {
        isGameActive = true;
        currentTime = timeLimit;
        pairsFound = 0;
        firstCardIndex = -1;
        secondCardIndex = -1;
        isInputLocked = false;
        shuffledFaces.Clear();
        int pairsNeeded = 6;
        for (int i = 0; i < pairsNeeded; i++)
        {
            Sprite face = cardFaceSprite[i % cardFaceSprite.Length];
            shuffledFaces.Add(face);
            shuffledFaces.Add(face);
        }

        totalPairs = pairsNeeded;

        Shuffle(shuffledFaces);

        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }
        cardButtons.Clear();

        for (int i = 0; i < shuffledFaces.Count; i++)
        {
            GameObject btnObj = Instantiate(cardButtonPrefab, gridContainer);
            Button btn = btnObj.GetComponent<Button>();
            Image img = btnObj.GetComponent<Image>();

            img.sprite = cardBackSprite;

            int idx = i;
            btn.onClick.AddListener(() => OnCardClicked(idx));

            cardButtons.Add(btn);
        }



    }

    void OnCardClicked(int index)
    {
        if (isInputLocked || index == firstCardIndex || !cardButtons[index].interactable) return;

        cardButtons[index].GetComponent<Image>().sprite = shuffledFaces[index];

        if (firstCardIndex == -1)
        {
            firstCardIndex = index;
        }
        else
        {
            secondCardIndex = index;
            StartCoroutine(CheckMatch());
        }
    }


    IEnumerator CheckMatch()
    {
        isInputLocked = true;

        Sprite firstSprite = shuffledFaces[firstCardIndex];
        Sprite secondSprite = shuffledFaces[secondCardIndex];

        if (firstSprite == secondSprite)
        {
            yield return new WaitForSeconds(0.5f);

            cardButtons[firstCardIndex].interactable = false;
            cardButtons[secondCardIndex].interactable = false;

            pairsFound++;

            if (pairsFound >= totalPairs)
            {
                GameOver(true);
            }
        }
        else
        {
            yield return new WaitForSeconds(2f);

            cardButtons[firstCardIndex].GetComponent<Image>().sprite = cardBackSprite;
            cardButtons[secondCardIndex].GetComponent<Image>().sprite = cardBackSprite;

        }

        firstCardIndex = -1;
        secondCardIndex = -1;
        isInputLocked = false;
    }

    void GameOver(bool didPlayerWin)
    {
        isGameActive = false;
        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            resultText.text = didPlayerWin ? "You win!" : "Time's Up!";
        }
        StartCoroutine(CloseAfterDelay(didPlayerWin));
    }

    IEnumerator CloseAfterDelay(bool didPlayerWin)
    {
        yield return new WaitForSeconds(2f);
        minigameManager.OnMinigameCompleted(didPlayerWin);
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0;i < list.Count;i++)
        {
            T item = list[i];
            int randIdx = Random.Range(i, list.Count);
            list[i] = list[randIdx];
            list[randIdx] = item;
        }
    }
}
