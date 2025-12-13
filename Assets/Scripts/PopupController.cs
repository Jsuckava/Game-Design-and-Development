using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using System.Collections;

public class PopupController : MonoBehaviour
{
    [Header("Main Panel")]
    public GameObject popupPanel;
    public TextMeshProUGUI titleText;

    [Header("Specific Panels")]
    public GameObject buildPopupPanel;
    public GameObject progressPopupPanel;

    [Header("HUD Buttons")]
    public Button progressHudButton; 

    [Header("Audio Settings")]
    public AudioSource uiAudioSource;    
    public AudioSource mainBgMusic;      
    public AudioClip cardFlipSound;

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
    public Button useItemButton;

    [Header("Item Use Choice Panel")]
    public GameObject itemUseChoicePanel;
    public Image itemUseImage;
    public TextMeshProUGUI itemUseDescriptionText;
    public Button consumeItemButton;
    public Button shareItemButton;

    [Header("Player Stat UI Links")]
    public GameObject healButtonsContainer;
    public List<Button> playerHealButtons;

    [Header("Component Links")]
    public PunishmentDeck punishmentDeck;
    public DisasterManager disasterManager;
    public BahayKuboTracker bahayKuboTracker; 

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

    [Header("Build & Progress Popup")]
    public TextMeshProUGUI buildTitleText;
    public Button buildCloseButton;
    public Button progressCloseButton;

    public AbilityManager abilityManager;

    private TurnManager turnManager;
    private PlayerStats currentPlayerStats;

    private Action<PlayerStats> storedPunishmentEvent;
    private bool isMinigamePopup = false;
    private bool isViewOnly = false; 

    private Punishment storedRewardItem;
    private float storedRewardValue;

    private string itemUsedName;
    private Sprite itemUsedIcon;
    private int itemEnergyValue = 7;
    private float animDuration = 0.2f;

    void Start()
    {
        turnManager = FindFirstObjectByType<TurnManager>();
        disasterManager = FindFirstObjectByType<DisasterManager>();
        
        if (bahayKuboTracker == null) bahayKuboTracker = FindFirstObjectByType<BahayKuboTracker>();
        abilityManager = FindFirstObjectByType<AbilityManager>();

        if (punishmentDeck == null)
            Debug.LogError("PunishmentDeck is NOT linked in the PopupController Inspector!", this.gameObject);

        revealCloseButton.onClick.AddListener(ClosePopup);
        simpleCloseButton.onClick.AddListener(ClosePopup);
        
        if (buildCloseButton != null) buildCloseButton.onClick.AddListener(ClosePopup);
        if (progressCloseButton != null) progressCloseButton.onClick.AddListener(ClosePopup);

        if (storeButton != null) storeButton.gameObject.SetActive(false);
        if (useButton != null) useButton.gameObject.SetActive(false);

        if (useItemButton != null) useItemButton.onClick.AddListener(ShowItemUsePanel);

        if (consumeItemButton != null) consumeItemButton.onClick.RemoveAllListeners();
        if (consumeItemButton != null) consumeItemButton.onClick.AddListener(() => OnUseItemClicked(false, currentPlayerStats));

        if (shareItemButton != null) shareItemButton.onClick.RemoveAllListeners();
        if (shareItemButton != null) shareItemButton.onClick.AddListener(ShowPlayerSelectionForShare);

        if (progressHudButton != null)
        {
            progressHudButton.onClick.RemoveAllListeners();
            progressHudButton.onClick.AddListener(OnProgressButtonClicked);
        }

        if (itemUseChoicePanel != null) itemUseChoicePanel.SetActive(false);
        if (popupPanel != null) popupPanel.SetActive(false);
        
        if (buildPopupPanel != null) buildPopupPanel.SetActive(false);
        if (progressPopupPanel != null) progressPopupPanel.SetActive(false);

        if (inventoryCloseBtn != null) inventoryCloseBtn.onClick.AddListener(CloseInventory);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (itemDetailPanel != null) itemDetailPanel.SetActive(false);

        SetHealButtonsVisible(false);
    }

    private void OnProgressButtonClicked()
    {
        if (turnManager != null)
        {
            PlayerStats activePlayer = turnManager.GetCurrentActivePlayer();
            if (activePlayer != null)
            {
                ShowBuildPopup(activePlayer, true);
            }
        }
    }

    private void SetHealButtonsVisible(bool visible)
    {
        if (healButtonsContainer != null) healButtonsContainer.SetActive(visible);

        if (!visible)
        {
            foreach (Button btn in playerHealButtons)
            {
                if (btn != null) btn.gameObject.SetActive(false);
            }
        }
    }

    private void OpenPanelWithAnimation(GameObject panel)
    {
        if (panel == null) return;
        StopAllCoroutines();
        StartCoroutine(AnimPanelRoutine(panel, true));
    }

    private void ClosePanelWithAnimation(GameObject panel, Action onComplete = null)
    {
        if (panel == null) return;
        StopAllCoroutines();
        StartCoroutine(AnimPanelRoutine(panel, false, onComplete));
    }

    private IEnumerator AnimPanelRoutine(GameObject panel, bool isOpen, Action onComplete = null)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = panel.AddComponent<CanvasGroup>();

        panel.SetActive(true);

        Vector3 startScale = isOpen ? Vector3.zero * 0.7f : Vector3.one;
        Vector3 endScale = isOpen ? Vector3.one : Vector3.zero * 0.7f;
        float startAlpha = isOpen ? 0f : 1f;
        float endAlpha = isOpen ? 1f : 0f;
        float timer = 0f;

        while (timer < animDuration)
        {
            timer += Time.deltaTime;
            float t = timer / animDuration;
            t = t * t * (3f - 2f * t);
            panel.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        panel.transform.localScale = endScale;
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

        currentPlayerStats = player;
        OpenPanelWithAnimation(inventoryPanel);

        if (itemDetailPanel != null) itemDetailPanel.SetActive(false);
        if (inventoryTitleText != null) inventoryTitleText.text = $"{player.playerName}'s Inventory";

        foreach (Transform child in inventoryGridContent)
        {
            Destroy(child.gameObject);
        }

        if (player.bamboo > 0) CreateInventorySlot("Bamboo", player.bamboo, punishmentDeck.bambooArt, "Used for crafting Bahay Kub parts.");
        if (player.nipaLeaves > 0) CreateInventorySlot("Nipa Leaves", player.nipaLeaves, punishmentDeck.nipaArt, "Used for crafting Bahay Kub parts.");
        if (player.hardwood > 0) CreateInventorySlot("Hardwood", player.hardwood, punishmentDeck.hardwoodArt, "Used for crafting Bahay Kub parts.");
        if (player.sawali > 0) CreateInventorySlot("Sawali", player.sawali, punishmentDeck.sawaliArt, "Used for crafting Bahay Kub parts.");

        foreach (var item in player.cropInventory)
        {
            Sprite cropIcon = GetCropSprite(item.Key);
            CreateInventorySlot(item.Key, item.Value, cropIcon, $"A healthy crop item: {item.Key}");
        }
    }

    private Sprite GetCropSprite(string itemType)
    {
        return itemType switch
        {
            "Talong" => punishmentDeck.talongArt,
            "Singkamas" => punishmentDeck.singkamasArt,
            "Mani" => punishmentDeck.maniArt,
            "Kundol" => punishmentDeck.kundolArt,
            "Patola" => punishmentDeck.patolaArt,
            "Upo" => punishmentDeck.upoArt,
            "Kalabasa" => punishmentDeck.kalabasaArt,
            "Labanos" => punishmentDeck.labanosArt,
            "Mustasa" => punishmentDeck.MustasaArt,
            "Luya" => punishmentDeck.luyaArt,
            "Ampalaya" => punishmentDeck.ampalayaArt,
            "Sitaw" => punishmentDeck.sitawArt,
            "Bataw" => punishmentDeck.batawArt,
            "Patani" => punishmentDeck.pataniArt,
            "Sigarilyas" => punishmentDeck.sigarilyasArt,
            "Sibuyas" => punishmentDeck.sibuyasArt,
            "Bawang" => punishmentDeck.bawangArt,
            "Carrots" => punishmentDeck.carrotsArt,
            "Patatas" => punishmentDeck.patatasArt,
            "Kamatis" => punishmentDeck.kamatisArt,
            "Blueberry" => punishmentDeck.blueberryArt,
            "Mangga" => punishmentDeck.manggaArt,
            "Mansanas" => punishmentDeck.mansanasArt,
            "Pina" => punishmentDeck.pinaArt,
            "Orange" => punishmentDeck.orangeArt,
            "Mangosten" => punishmentDeck.mangostenArt,
            "Lemon" => punishmentDeck.lemonArt,
            "Longan" => punishmentDeck.longanArt,
            "Loquat" => punishmentDeck.loquatArt,
            _ => null,
        };
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
        if (itemDetailPanel == null) return;

        itemDetailPanel.SetActive(true);

        if (detailNameText != null) detailNameText.text = name;
        if (detailImage != null) detailImage.sprite = icon;
        if (detailDescText != null) detailDescText.text = desc;

        bool isUsableCrop = currentPlayerStats != null && currentPlayerStats.cropInventory.ContainsKey(name);

        if (useItemButton != null)
        {
            useItemButton.gameObject.SetActive(isUsableCrop);
            if (isUsableCrop)
            {
                TextMeshProUGUI buttonText = useItemButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null) buttonText.text = "Use Item";
            }
        }
        itemUsedName = name;
        itemUsedIcon = icon;
    }

    public void CloseInventory()
    {
        if (inventoryPanel != null && inventoryPanel.activeSelf)
            ClosePanelWithAnimation(inventoryPanel);
        if (itemDetailPanel != null) itemDetailPanel.SetActive(false);
    }

    private void CleanUpAllCardUI()
    {
        if (cardChoiceContainer != null) cardChoiceContainer.SetActive(false);
        if (cardRevealPanel != null) cardRevealPanel.SetActive(false);
        if (storeButton != null) storeButton.gameObject.SetActive(false);
        if (useButton != null) useButton.gameObject.SetActive(false);
        if (useItemButton != null) useItemButton.gameObject.SetActive(false);
        if (titleText != null) titleText.gameObject.SetActive(false);
        if (simpleDescriptionText != null) simpleDescriptionText.gameObject.SetActive(false);
        
        if (buildPopupPanel != null) buildPopupPanel.SetActive(false);
        if (progressPopupPanel != null) progressPopupPanel.SetActive(false);

        SetHealButtonsVisible(false);
    }

    public void ShowBuildPopup(PlayerStats player, bool viewOnly = false)
    {
        CleanUpAllCardUI();
        
        isViewOnly = viewOnly;
        
        currentPlayerStats = player;
        
        if (buildTitleText != null) 
        {
            if (viewOnly)
                buildTitleText.text = $"Construction Progress ({player.playerName})";
            else
                buildTitleText.text = $"Choose a part to build, {player.playerName}:";
        }

        if (viewOnly)
        {
            if (progressPopupPanel != null) OpenPanelWithAnimation(progressPopupPanel);
        }
        else
        {
            if (buildPopupPanel != null) OpenPanelWithAnimation(buildPopupPanel);
        }
    }

    public void BuildSpecificPart(string partName)
    {
        ClosePopup(); 

        if (bahayKuboTracker != null && currentPlayerStats != null)
        {
            string result = bahayKuboTracker.AttemptBuild(currentPlayerStats);

            string message = "";
            switch (result)
            {
                case "Haligi": message = "Haligi built successfully!"; break;
                case "Sahig": message = "Sahig built successfully!"; break;
                case "Pader": message = "Pader built successfully!"; break;
                case "Bubong": message = "Bubong built successfully! The Bahay Kubo is complete!"; break;
                case "Complete": message = "The Bahay Kubo is already complete!"; break;
                
                case "NotEnoughBamboo": message = "Not enough Bamboo to build the Haligi!"; break;
                case "NotEnoughHardwood": message = "Not enough Hardwood to build the Sahig!"; break;
                case "NotEnoughSawali": message = "Not enough Sawali to build the Pader!"; break;
                case "NotEnoughBambooForPader": message = "You have Sawali, but need more Bamboo for the Pader!"; break;
                case "NotEnoughNipa": message = "Not enough Nipa Leaves to build the Bubong!"; break;
                case "NotEnoughBambooForBubong": message = "You have Nipa, but need more Bamboo for the Bubong!"; break;
                
                default: message = "Build attempt failed. Check your materials."; break;
            }

            ShowSimplePopup("Build Result", message, currentPlayerStats);
        }
        else
        {
            Debug.LogError("BahayKuboTracker or PlayerStats is missing!");
        }
    }

    public void ShowItemUsePanel()
    {
        if (itemUsedName == null) return;

        itemEnergyValue = (int)punishmentDeck.GetCropEnergyValue(itemUsedName);

        CloseInventory();

        string title = $"Use {itemUsedName}?";
        string descriptionChoice = $"Choose to Consume for yourself (+{itemEnergyValue} Energy), or Share with a teammate (+10 Global Morale).";

        CleanUpAllCardUI();

        OpenPanelWithAnimation(popupPanel);
        if (titleText != null)
        {
            titleText.gameObject.SetActive(true);
            titleText.text = title;
        }

        if (itemUseChoicePanel != null) itemUseChoicePanel.SetActive(true);

        if (itemUseImage != null) itemUseImage.sprite = itemUsedIcon;

        if (itemUseDescriptionText != null)
        {
            itemUseDescriptionText.text = $"This crop item is used to restore energy or boost team morale.\n\n{descriptionChoice}";
            itemUseDescriptionText.gameObject.SetActive(true);
        }

        if (simpleCloseButton != null) simpleCloseButton.gameObject.SetActive(false);

        if (consumeItemButton != null) consumeItemButton.GetComponentInChildren<TextMeshProUGUI>().text = "Consume It";
        if (shareItemButton != null) shareItemButton.GetComponentInChildren<TextMeshProUGUI>().text = "Share It";
    }

    public void ShowPlayerSelectionForShare()
    {
        if (turnManager == null || playerHealButtons == null) return;

        StopAllCoroutines();

        if (popupPanel != null) popupPanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (itemDetailPanel != null) itemDetailPanel.SetActive(false);

        CleanUpAllCardUI();
        if (itemUseChoicePanel != null) itemUseChoicePanel.SetActive(false);
        if (simpleCloseButton != null) simpleCloseButton.gameObject.SetActive(false);

        SetHealButtonsVisible(true);

        List<PlayerStats> allPlayers = turnManager.GetAllActivePlayers();

        if (playerHealButtons.Count < 4)
        {
            Debug.LogError("You only have " + playerHealButtons.Count + " Heal Buttons! Duplicate 'Heal 3' in Hierarchy, rename to 'Heal 4', and add to PopupController Inspector.");
        }

        for (int i = 0; i < playerHealButtons.Count; i++)
        {
            Button healButton = playerHealButtons[i];

            if (i >= allPlayers.Count)
            {
                if (healButton != null) healButton.gameObject.SetActive(false);
                continue;
            }

            PlayerStats targetPlayer = allPlayers[i];

            if (healButton == null) continue;

            if (targetPlayer != null && targetPlayer != currentPlayerStats)
            {
                SetupHealButton(healButton, targetPlayer);
            }
            else
            {
                healButton.gameObject.SetActive(false);
            }
        }
    }

    private void SetupHealButton(Button btn, PlayerStats playerToHeal)
    {
        btn.gameObject.SetActive(true);
        btn.onClick.RemoveAllListeners();

        btn.onClick.AddListener(() =>
        {
            OnUseItemClicked(true, playerToHeal);
        });
    }

    private void OnUseItemClicked(bool shareForMorale, PlayerStats targetPlayer)
    {
        if (currentPlayerStats == null || itemUsedName == null) return;

        int valueToApply = (int)punishmentDeck.GetCropEnergyValue(itemUsedName);
        if (valueToApply == 0) valueToApply = 7;

        if (shareForMorale || targetPlayer == currentPlayerStats)
            currentPlayerStats.ChangeCropCount(itemUsedName, -1);

        if (shareForMorale)
        {
            if (targetPlayer != null)
            {
                targetPlayer.ChangeEnergy(valueToApply);
                targetPlayer.UpdateStatDisplay();

                if (disasterManager != null)
                {
                    disasterManager.AddGlobalMorale(10);
                }

                simpleDescriptionText.text = $"{itemUsedName} Shared with {targetPlayer.playerName}! +10 Global Morale and +{valueToApply} Energy applied! \n(Closing in 2 seconds...)";
            }
        }
        else
        {
            currentPlayerStats.ChangeEnergy(valueToApply);
            simpleDescriptionText.text = $"{itemUsedName} Consumed! +{valueToApply} Energy applied! \n(Closing in 2 seconds...)";
        }

        if (itemUseChoicePanel != null) itemUseChoicePanel.SetActive(false);
        SetHealButtonsVisible(false);
        
        CloseInventory();
        
        OpenPanelWithAnimation(popupPanel);

        if (titleText != null) titleText.gameObject.SetActive(false);
        if (simpleDescriptionText != null) simpleDescriptionText.gameObject.SetActive(true);
        if (simpleCloseButton != null) simpleCloseButton.gameObject.SetActive(false);

        itemUsedName = null;
        itemUsedIcon = null;

        StartCoroutine(ClosePopupDelayed(2f));
    }

    public void ShowPunishmentChoice(string title, PlayerStats player)
    {
        PrepareCardPopup(title, player);
        isMinigamePopup = false;

        List<Punishment> punishments = punishmentDeck.GetRandomDisasterCards(3);
        GenerateCards(punishments, cardBackPrefab_Disaster, OnPunishmentCardSelected);

        OpenPanelWithAnimation(popupPanel);
    }

    public void ShowMaterialChoice(string title, PlayerStats player)
    {
        PrepareCardPopup(title, player);
        isMinigamePopup = false;

        List<Punishment> materials = punishmentDeck.GetRandomMaterialCards(3);
        GenerateCards(materials, cardBackPrefab_Wild, OnPunishmentCardSelected);

        OpenPanelWithAnimation(popupPanel);
    }

    public void ShowMiniGamePopup(string title, PlayerStats player)
    {
        PrepareCardPopup(title, player);
        isMinigamePopup = true;

        List<Punishment> miniGames = punishmentDeck.GetRandomMiniGameCards(3);
        GenerateCards(miniGames, cardBackPrefab_MiniGame, OnPunishmentCardSelected);

        OpenPanelWithAnimation(popupPanel);
    }

    public void ShowRewardChoice(string title, PlayerStats player)
    {
        PrepareCardPopup(title, player);
        isMinigamePopup = false;

        List<Punishment> rewards = punishmentDeck.GetRandomCropCards(3);
        GenerateCards(rewards, cardBackPrefab_Wild, OnRewardCardSelected);

        OpenPanelWithAnimation(popupPanel);
    }

    private void PrepareCardPopup(string title, PlayerStats player)
    {
        CleanUpAllCardUI();
        currentPlayerStats = player;
        if (titleText != null)
        {
            titleText.gameObject.SetActive(true);
            titleText.text = title;
        }
        if (itemUseChoicePanel != null) itemUseChoicePanel.SetActive(false);
        cardChoiceContainer.SetActive(true);
        cardRevealPanel.SetActive(false);
        simpleDescriptionText.gameObject.SetActive(false);
        simpleCloseButton.gameObject.SetActive(false);

        foreach (Transform child in cardChoiceContainer.transform) Destroy(child.gameObject);
    }

    private void GenerateCards(List<Punishment> cards, GameObject prefab, Action<Punishment> onClickAction)
    {
        foreach (var card in cards)
        {
            GameObject cardBackGO = Instantiate(prefab, cardChoiceContainer.transform);
            Button cardButton = cardBackGO.GetComponent<Button>();
            cardButton.onClick.AddListener(() => onClickAction(card));
        }
    }

   public void ShowSimplePopup(string title, string description, PlayerStats player = null)
    {
        CleanUpAllCardUI();

        if (player != null)
        {
            currentPlayerStats = player;
        }

        if (titleText != null)
        {
            titleText.gameObject.SetActive(true);
            titleText.text = title;
        }

        simpleDescriptionText.text = description;
        isMinigamePopup = false;

        if (itemUseChoicePanel != null) itemUseChoicePanel.SetActive(false);

        cardChoiceContainer.SetActive(false);
        cardRevealPanel.SetActive(false);
        simpleDescriptionText.gameObject.SetActive(true);
        simpleCloseButton.gameObject.SetActive(true);

        OpenPanelWithAnimation(popupPanel);
    }

    private void OnPunishmentCardSelected(Punishment punishment)
    {
        if (mainBgMusic != null)
        {
            mainBgMusic.Pause();
        }

        if (uiAudioSource != null)
        {
            if (cardFlipSound != null)
                uiAudioSource.PlayOneShot(cardFlipSound);

            if (punishment.effectSound != null)
            {
                uiAudioSource.clip = punishment.effectSound;
                uiAudioSource.Play();
            }
        }

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
        if (mainBgMusic != null)
        {
            mainBgMusic.Pause();
        }

        if (uiAudioSource != null)
        {
            if (cardFlipSound != null)
                uiAudioSource.PlayOneShot(cardFlipSound);

            if (reward.effectSound != null)
            {
                uiAudioSource.clip = reward.effectSound;
                uiAudioSource.Play();
            }
        }

        cardChoiceContainer.SetActive(false);
        simpleDescriptionText.gameObject.SetActive(false);
        simpleCloseButton.gameObject.SetActive(false);

        cardRevealPanel.SetActive(true);
        revealImage.sprite = reward.cardArt;
        revealTitleText.text = reward.title;
        revealDescriptionText.text = reward.description + "\n\nChoose: Store for Global Morale or Use now for Energy.";

        storedRewardItem = reward;
        storedRewardValue = reward.value > 0 ? reward.value : 7f;

        revealCloseButton.gameObject.SetActive(false);

        if (storeButton != null)
        {
            storeButton.gameObject.SetActive(true);
            storeButton.onClick.RemoveAllListeners();
            storeButton.onClick.AddListener(OnStoreClicked);
        }
        if (useButton != null)
        {
            useButton.gameObject.SetActive(true);
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(OnUseClicked);
        }

        storedPunishmentEvent = null;
        isMinigamePopup = false;
    }

    private void OnStoreClicked()
    {
        if (currentPlayerStats != null && storedRewardItem != null)
        {
            bool isStored = false;
            switch (storedRewardItem.itemType)
            {
                case "Bamboo":
                    currentPlayerStats.bamboo += storedRewardItem.quantity;
                    isStored = true;
                    if (disasterManager != null) disasterManager.AddGlobalMorale(10);
                    break;
                case "NipaLeaves":
                    currentPlayerStats.nipaLeaves += storedRewardItem.quantity;
                    isStored = true;
                    if (disasterManager != null) disasterManager.AddGlobalMorale(10);
                    break;
                case "Hardwood":
                    currentPlayerStats.hardwood += storedRewardItem.quantity;
                    isStored = true;
                    if (disasterManager != null) disasterManager.AddGlobalMorale(10);
                    break;
                case "Sawali":
                    currentPlayerStats.sawali += storedRewardItem.quantity;
                    isStored = true;
                    if (disasterManager != null) disasterManager.AddGlobalMorale(10);
                    break;

                case "Talong":
                case "Singkamas":
                case "Mani":
                case "Kundol":
                case "Patola":
                case "Upo":
                case "Kalabasa":
                case "Labanos":
                case "Mustasa":
                case "Luya":
                case "Ampalaya":
                case "Sitaw":
                case "Bataw":
                case "Patani":
                case "Sigarilyas":
                case "Sibuyas":
                case "Bawang":
                case "Carrots":
                case "Patatas":
                case "Kamatis":
                case "Blueberry":
                case "Mangga":
                case "Mansanas":
                case "Pina":
                case "Orange":
                case "Mangosten":
                case "Lemon":
                case "Longan":
                case "Loquat":
                    currentPlayerStats.ChangeCropCount(storedRewardItem.itemType, storedRewardItem.quantity);
                    isStored = true;
                    if (disasterManager != null) disasterManager.AddGlobalMorale(10);
                    break;

                default:
                    if (disasterManager != null)
                    {
                        disasterManager.AddGlobalMorale(10);
                    }
                    break;
            }

            string storedItemName = storedRewardItem.title;
            string message;

            if (isStored)
            {
                message = $" {storedItemName}  (+{storedRewardItem.quantity}) stored in inventory! Global Morale boosted! \n(Closing in 2 seconds...)";
                bool isVegetable = CheckIfVegetable(storedRewardItem.itemType);
                if (isVegetable && abilityManager != null)
                {
                    abilityManager.CheckCardObtainedBonus(currentPlayerStats, "Vegetable");
                }
            }
            else
            {
                message = $" {storedItemName}  stored! Global Morale boosted by 10! \n(Closing in 2 seconds...)";
            }

            revealDescriptionText.text = message;
        }
        else
        {
            revealDescriptionText.text = "ERROR: Player stats or reward item missing. Closing in 2 seconds.";
        }

        if (storeButton != null) storeButton.gameObject.SetActive(false);
        if (useButton != null) useButton.gameObject.SetActive(false);

        storedRewardItem = null;
        storedPunishmentEvent = null;
        StartCoroutine(ClosePopupDelayed(2f));
    }

    private bool CheckIfVegetable(string itemType)
    {
        return itemType == "Talong" || itemType == "Singkamas" || itemType == "Sitaw" || 
                    itemType == "Kalabasa" || itemType == "Mustasa" || itemType == "Ampalaya";    
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

        storedRewardItem = null;
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
        if (!isMinigamePopup)
        {
            if (uiAudioSource != null) uiAudioSource.Stop();
            if (mainBgMusic != null) mainBgMusic.UnPause();
        }
        
        if (storedPunishmentEvent != null && currentPlayerStats != null)
        {
            storedPunishmentEvent.Invoke(currentPlayerStats);
        }

        CleanUpAllCardUI();
        if (itemUseChoicePanel != null) itemUseChoicePanel.SetActive(false);
        SetHealButtonsVisible(false);

        if (isViewOnly)
        {
            isViewOnly = false;
            currentPlayerStats = null;
            if (buildPopupPanel != null && buildPopupPanel.activeSelf) ClosePanelWithAnimation(buildPopupPanel);
            if (progressPopupPanel != null && progressPopupPanel.activeSelf) ClosePanelWithAnimation(progressPopupPanel);
            return;
        }

        GameObject panelToClose = popupPanel;
        if (buildPopupPanel != null && buildPopupPanel.activeSelf)
        {
            panelToClose = buildPopupPanel;
        }

        ClosePanelWithAnimation(panelToClose, () =>
        {
            if (currentPlayerStats == null) return;

            PlayerMovement currentMovement = currentPlayerStats.GetComponent<PlayerMovement>();

            if (!isMinigamePopup)
            {
                if (currentMovement != null)
                {
                    currentMovement.FinishTurnExecution();
                }
            }
            else
            {
                turnManager.ResumeTurn();
            }

            currentPlayerStats = null;
            storedPunishmentEvent = null;
            isMinigamePopup = false;
        });
    }
}