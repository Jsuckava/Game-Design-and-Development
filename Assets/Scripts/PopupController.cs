using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using System;
using System.Collections;
using Unity.VisualScripting;

public class PopupController : MonoBehaviour
{
    [Header("Main Panel")]
    public GameObject popupPanel;
    public TextMeshProUGUI titleText;

    [Header("Inventory System")]
    public GameObject inventoryPanel;
    public Transform inventoryGridContent;
    public GameObject inventorySlotPrefab;
    public Button inventoryCloseBtn;

    public TextMeshProUGUI inventoryTitleText;

    [Header("Inventory Details")]
    public GameObject itemDetailPanel;
    public TextMeshProUGUI detailNameText;
    public TextMeshProUGUI detailDescText;
    public Image detailImage;

    [Header("Component Links")]
    public PunishmentDeck punishmentDeck;
    public DisasterManager disasterManager;

    [Header("Card Choice")]
    public GameObject cardChoiceContainer;
    public GameObject cardBackPrefab_Disaster;
    public GameObject cardBackPrefab_Wild;
    public GameObject cardBackPrefab_MiniGame;

    [Header("Card Reveal")]
    public GameObject cardRevealPanel;
    public Image revealImage;
    public TextMeshProUGUI revealTitleText;
    public TextMeshProUGUI revealDescriptionText;
    public Button revealCloseButton;

    [Header("Reward Choice Buttons")]
    public Button storeButton;
    public Button useButton;

    [Header("Simple Popup")]
    public TextMeshProUGUI simpleDescriptionText;
    public Button simpleCloseButton;

    private TurnManager turnManager;
    private PlayerStats currentPlayerStats;
    
    private Action<PlayerStats> storedPunishmentEvent;
    private bool isMinigamePopup = false;
    private float storedRewardValue;

    private float animDuration = 0.2f;

    void Start()
    {
        turnManager = FindFirstObjectByType<TurnManager>();
        disasterManager = FindFirstObjectByType<DisasterManager>();

        if (punishmentDeck == null)
        {
            Debug.LogError("CRITICAL ERROR: The PunishmentDeck is NOT linked in the PopupController Inspector!", this.gameObject);
        }

        revealCloseButton.onClick.AddListener(ClosePopup);
        simpleCloseButton.onClick.AddListener(ClosePopup);

        if (storeButton != null) storeButton.gameObject.SetActive(false);
        if (useButton != null) useButton.gameObject.SetActive(false);

        popupPanel.SetActive(false);

        if (inventoryCloseBtn != null) inventoryCloseBtn.onClick.AddListener(CloseInventory);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (itemDetailPanel != null) itemDetailPanel.SetActive(false);
    }

    private void OpenPanelWithAnimation(GameObject panel)
    {
        StopAllCoroutines();
        StartCoroutine(AnimePanelRoutine(panel, true));
    }

    private void ClosePanelWithAnimation(GameObject panel, Action onComplete = null)
    {
        StartCoroutine(AnimePanelRoutine(panel, false, onComplete));
    }

    private IEnumerator AnimePanelRoutine(GameObject panel, bool isOpen, Action onComplete = null)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = panel.AddComponent<CanvasGroup>();

        panel.SetActive(true);
        
        Vector3 starScale = isOpen ? Vector3.zero * 0.7f : Vector3.one;
        Vector3 endScale = isOpen ? Vector3.one : Vector3.zero * 0.7f;
        float startAlpha = isOpen ? 0f : 1f;
        float endAlpha = isOpen ? 1f : 0f;
        float timer = 0f;

        while (timer < animDuration)
        {
            timer += Time.deltaTime;
            float t = timer / animDuration;
            t = t * t * (3f - 2f * t); // Smoothstep
            panel.transform.localScale = Vector3.Lerp(starScale, endScale, t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        panel.transform.localScale = isOpen ? Vector3.one : Vector3.zero * 0.7f;
        canvasGroup.alpha = endAlpha;

        if (!isOpen)
        {
            panel.SetActive(false);
        }

        if (onComplete != null)
        {
            onComplete.Invoke();
        }

    }

    public void ShowInventory(PlayerStats player)
    {
        if (inventoryPanel == null) return;

        // inventoryPanel.SetActive(true);
        OpenPanelWithAnimation(inventoryPanel);
        if (itemDetailPanel != null) itemDetailPanel.SetActive(false);
        if (inventoryTitleText != null) inventoryTitleText.text = $"{player.playerName}'s Inventory";
        foreach (Transform child in inventoryGridContent)
        {
            Destroy(child.gameObject);
        }

        if (player.bamboo > 0)
        {
            CreateInventorySlot("Bamboo", player.bamboo, punishmentDeck.bambooArt, "Used for crafting Bahay Kub parts.");
        }
        if (player.nipaLeaves > 0)
        {
            CreateInventorySlot("Nipa Leaves", player.nipaLeaves, punishmentDeck.nipaArt, "Used for crafting Bahay Kub parts.");
        }
        if (player.hardwood > 0)
        {
            CreateInventorySlot("Hardwood", player.hardwood, punishmentDeck.hardwoodArt, "Used for crafting Bahay Kub parts.");
        }
        if (player.sawali > 0)
        {
            CreateInventorySlot("Sawali", player.sawali, punishmentDeck.sawaliArt, "Used for crafting Bahay Kub parts.");
        }
    }

    private void CreateInventorySlot(string name, int count, Sprite icon, string desc)
    {
        GameObject newSlot = Instantiate(inventorySlotPrefab, inventoryGridContent);
        InventorySlot slotScript = newSlot.GetComponent<InventorySlot>();

        if (slotScript != null)
        {
            slotScript.Initialize(name, count, icon, desc, OnItemClicked);
        }
    }

    private void OnItemClicked(string name, string desc, Sprite icon)
    {
        if (itemDetailPanel != null)
        {
            itemDetailPanel.SetActive(true);
            if (detailNameText != null) detailNameText.text = name;
            if (detailImage != null) detailImage.sprite = icon;
            if (detailDescText != null) detailDescText.text = desc;
            
        }
    }

    public void CloseInventory()
    {
        if (inventoryPanel != null) ClosePanelWithAnimation(inventoryPanel);/* inventoryPanel.SetActive(false) */ 
    }

    public void ShowPunishmentChoice(string title, PlayerStats player)
    {
        currentPlayerStats = player;
        titleText.text = title;
        isMinigamePopup = false;

        cardChoiceContainer.SetActive(true);
        cardRevealPanel.SetActive(false);
        simpleDescriptionText.gameObject.SetActive(false);
        simpleCloseButton.gameObject.SetActive(false);

        foreach (Transform child in cardChoiceContainer.transform)
        {
            Destroy(child.gameObject);
        }

        List<Punishment> punishments = punishmentDeck.GetRandomDisasterCards(3);

        foreach (var punishment in punishments)
        {
            GameObject cardBackGO = Instantiate(cardBackPrefab_Disaster, cardChoiceContainer.transform);
            Button cardButton = cardBackGO.GetComponent<Button>();
            cardButton.onClick.AddListener(() =>
            {
                OnPunishmentCardSelected(punishment);
            });
        }
        // popupPanel.SetActive(true);
        OpenPanelWithAnimation(popupPanel);
    }

    public void ShowMaterialChoice(string title, PlayerStats player)
    {
        currentPlayerStats = player;
        titleText.text = title;
        isMinigamePopup = false;

        cardChoiceContainer.SetActive(true);
        cardRevealPanel.SetActive(false);
    
        simpleDescriptionText.gameObject.SetActive(false);
        simpleCloseButton.gameObject.SetActive(false);

        foreach (Transform child in cardChoiceContainer.transform)
        {
            Destroy(child.gameObject);
        }

        List<Punishment> materials = punishmentDeck.GetRandomMaterialCards(3);

        foreach (var material in materials)
        {
            GameObject cardBackGO = Instantiate(cardBackPrefab_Wild, cardChoiceContainer.transform);
            Button cardButton = cardBackGO.GetComponent<Button>();
            cardButton.onClick.AddListener(() =>
            {
                OnPunishmentCardSelected(material);
            });
        }
        // popupPanel.SetActive(true);
        OpenPanelWithAnimation(popupPanel);
    }

    public void ShowMiniGamePopup(string title, PlayerStats player)
    {
        currentPlayerStats = player;
        titleText.text = title;
        isMinigamePopup = true;

        cardChoiceContainer.SetActive(true);
        cardRevealPanel.SetActive(false);
        simpleDescriptionText.gameObject.SetActive(false);
        simpleCloseButton.gameObject.SetActive(false);

        foreach (Transform child in cardChoiceContainer.transform)
        {
            Destroy(child.gameObject);
        }

        List<Punishment> miniGames = punishmentDeck.GetRandomMiniGameCards(3);

        foreach (var minigameCard in miniGames)
        {
            GameObject cardBackGO = Instantiate(cardBackPrefab_MiniGame, cardChoiceContainer.transform);
            Button cardButton = cardBackGO.GetComponent<Button>();
            cardButton.onClick.AddListener(() =>
            {
                OnPunishmentCardSelected(minigameCard);
            });
        }
        // popupPanel.SetActive(true);
        OpenPanelWithAnimation(popupPanel);
        
    }

    public void ShowRewardChoice(string title, PlayerStats player)
    {
        currentPlayerStats = player;
        titleText.text = title;
        isMinigamePopup = true;

        cardChoiceContainer.SetActive(true);
        cardRevealPanel.SetActive(false);
    
        simpleDescriptionText.gameObject.SetActive(false);
        simpleCloseButton.gameObject.SetActive(false);

        foreach (Transform child in cardChoiceContainer.transform)
        {
            Destroy(child.gameObject);
        }

        List<Punishment> rewards = punishmentDeck.GetRandomCropCards(3);

        foreach (var reward in rewards)
        {
            GameObject cardBackGO = Instantiate(cardBackPrefab_Wild, cardChoiceContainer.transform);
            Button cardButton = cardBackGO.GetComponent<Button>();
            cardButton.onClick.AddListener(() =>
            {
                OnRewardCardSelected(reward);
            });
        }
        // popupPanel.SetActive(true);
        OpenPanelWithAnimation(popupPanel);
    }

    public void ShowSimplePopup(string title, string description)
    {
        titleText.text = title;
        simpleDescriptionText.text = description;
        isMinigamePopup = false;

        cardChoiceContainer.SetActive(false);
        cardRevealPanel.SetActive(false);
        simpleDescriptionText.gameObject.SetActive(true);
        simpleCloseButton.gameObject.SetActive(true);

        if (storeButton != null) storeButton.gameObject.SetActive(false);
        if (useButton != null) useButton.gameObject.SetActive(false);

        // popupPanel.SetActive(true);
        OpenPanelWithAnimation(popupPanel);
    }

    private void OnPunishmentCardSelected(Punishment punishment)
    {
        cardChoiceContainer.SetActive(false);
        simpleDescriptionText.gameObject.SetActive(false);
        simpleCloseButton.gameObject.SetActive(false);

        cardRevealPanel.SetActive(true);
        revealImage.sprite = punishment.cardArt;
        revealTitleText.text = punishment.title;
        revealDescriptionText.text = punishment.description;
        
        if (storeButton != null) storeButton.gameObject.SetActive(false);
        if (useButton != null) useButton.gameObject.SetActive(false);
        revealCloseButton.gameObject.SetActive(true);

        storedPunishmentEvent = punishment.onPunishmentSelected;
        isMinigamePopup = punishment.isMinigame;
    }

    private void OnRewardCardSelected(Punishment reward)
    {
        cardChoiceContainer.SetActive(false);
        simpleDescriptionText.gameObject.SetActive(false);
        simpleCloseButton.gameObject.SetActive(false);

        cardRevealPanel.SetActive(true);
        revealImage.sprite = reward.cardArt;
        revealTitleText.text = reward.title;
        revealDescriptionText.text = reward.description + "\n\nChoose: Store for Global Morale or Use now for Energy."; 
        
        storedRewardValue = 7f; 
        
        revealCloseButton.gameObject.SetActive(false); 
        if (storeButton != null) storeButton.gameObject.SetActive(true);
        if (useButton != null) useButton.gameObject.SetActive(true);

        if (storeButton != null)
        {
            storeButton.onClick.RemoveAllListeners();
            storeButton.onClick.AddListener(OnStoreClicked);
        }
        if (useButton != null)
        {
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(OnUseClicked);
        }
        
        storedPunishmentEvent = null;
        isMinigamePopup = false;
    }

    private void OnStoreClicked()
    {
        if (disasterManager != null)
        {
            disasterManager.AddGlobalMorale(10); 
            revealDescriptionText.text = "Reward stored! Global Morale boosted! \n(Closing in 2 seconds...)";
        }
        else
        {
            revealDescriptionText.text = "ERROR: Morale Tracker missing. Closing in 2 seconds.";
        }
        
        if (storeButton != null) storeButton.gameObject.SetActive(false);
        if (useButton != null) useButton.gameObject.SetActive(false);
        
        storedPunishmentEvent = null;
        StartCoroutine(ClosePopupDelayed(2f));
    }

    private void OnUseClicked()
    {
        if (currentPlayerStats != null)
        {
            currentPlayerStats.ChangeEnergy(storedRewardValue);
            revealDescriptionText.text = $"Energy Gained! +{storedRewardValue} Energy applied! \n(Closing in 2 seconds...)";
        }
        else
        {
            revealDescriptionText.text = "ERROR: Player stats missing. Closing in 2 seconds.";
        }
        
        if (storeButton != null) storeButton.gameObject.SetActive(false);
        if (useButton != null) useButton.gameObject.SetActive(false);
        
        storedPunishmentEvent = null;
        StartCoroutine(ClosePopupDelayed(2f));
    }

    private void DisableButtons()
    {
        if (storeButton != null) storeButton.gameObject.SetActive(false);
        if (useButton != null) useButton.gameObject.SetActive(false);
    }
    
    IEnumerator ClosePopupDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClosePopup();
    }


    private void ClosePopup()
    {
        if (storedPunishmentEvent != null && currentPlayerStats != null){
            storedPunishmentEvent.Invoke(currentPlayerStats);
        }
        
        // if (storeButton != null) storeButton.gameObject.SetActive(false);
        // if (useButton != null) useButton.gameObject.SetActive(false);
        DisableButtons();
        // popupPanel.SetActive(false);
        // currentPlayerStats = null;
        // storedPunishmentEvent = null;

        ClosePanelWithAnimation(popupPanel, () =>
        {
            currentPlayerStats = null;
            storedPunishmentEvent = null;

            if (turnManager != null && !isMinigamePopup)
            {
                turnManager.EndTurn();
            }

            isMinigamePopup = false;
        });
        // if (turnManager != null && !isMinigamePopup)
        // {
        //     turnManager.EndTurn();
        // }
        
        // isMinigamePopup = false;
    }
}