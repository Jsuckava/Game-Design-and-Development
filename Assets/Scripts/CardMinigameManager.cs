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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void ShuffleAndAssign()
    {
        shuffledMinigame = miniGamePrefabs.OrderBy(x => Random.Range(0f, 1f)).ToList();

        if (cardButtons.Length < miniGamePrefabs.Count)
        {
            Debug.LogError("Not enough card buttons for the number of minigame scenes.");
            return;
        }

        for (int i = 0; i < cardButtons.Length; i++)
        {
            cardButtons[i].onClick.RemoveAllListeners();
            cardButtons[i].interactable = true;

            GameObject prefabToLoad = shuffledMinigame[i];
            cardButtons[i].onClick.AddListener(() => ShowMiniGamePopup(prefabToLoad));
        }
    }
    void ShowMiniGamePopup(GameObject prefab)
    {
        foreach (Button btn in cardButtons)
        {
            btn.interactable = false;
        }
        Instantiate(prefab, popupContainer);
    }
        
        void ResetCards()
    {
        foreach (Button btn in cardButtons)
        {
            btn.interactable = true;
        }

        ShuffleAndAssign();
    }
}