using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using System;
using System.Collections;

public class PopupController : MonoBehaviour
{
    [Header("Main Panel")]
    public GameObject popupPanel;
    public TextMeshProUGUI titleText;

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
        popupPanel.SetActive(true);
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
        popupPanel.SetActive(true);
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
        popupPanel.SetActive(true);
        
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
        popupPanel.SetActive(true);
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

        popupPanel.SetActive(true);
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
        
        if (storeButton != null) storeButton.gameObject.SetActive(false);
        if (useButton != null) useButton.gameObject.SetActive(false);

        popupPanel.SetActive(false);
        currentPlayerStats = null;
        storedPunishmentEvent = null;

        if (turnManager != null)
        {
            turnManager.EndTurn();
        }
        
        isMinigamePopup = false;
    }
}