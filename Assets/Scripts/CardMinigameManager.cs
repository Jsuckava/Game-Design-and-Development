using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class CardMinigameManager : MonoBehaviour
{
    public Button[] cardButtons;
    public List<GameObject> miniGamePrefabs;
    public Transform popupContainer;

    private List<GameObject> shuffledMinigame;

    void Start()
    {
        if (Timer.IsGameOver)
        {
            gameObject.SetActive(false);
            return;
        }

        ShuffleAndAssign();
    }

    void ShuffleAndAssign()
    {
        shuffledMinigame = miniGamePrefabs.OrderBy(x => Random.Range(0f, 1f)).ToList();

        int limit = Mathf.Min(cardButtons.Length, shuffledMinigame.Count);

        for (int i = 0; i < limit; i++)
        {
            cardButtons[i].onClick.RemoveAllListeners();
            
            if (Timer.IsGameOver)
            {
                cardButtons[i].interactable = false;
                continue;
            }

            cardButtons[i].interactable = true;

            GameObject prefabToLoad = shuffledMinigame[i];
            cardButtons[i].onClick.AddListener(() => ShowMiniGamePopup(prefabToLoad));
        }
    }

    void ShowMiniGamePopup(GameObject prefab)
    {
        if (Timer.IsGameOver) return;

        foreach (Button btn in cardButtons)
        {
            btn.interactable = false;
        }
        Instantiate(prefab, popupContainer);
    }
        
    void ResetCards()
    {
        if (Timer.IsGameOver) return;

        foreach (Button btn in cardButtons)
        {
            btn.interactable = true;
        }

        ShuffleAndAssign();
    }
}