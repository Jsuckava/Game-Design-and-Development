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
    private Punishment storedRewardItem; 
    private float storedRewardValue;

    private string itemUsedName;
    private Sprite itemUsedIcon;
    private int itemEnergyValue = 7; 

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
        
        if (useItemButton != null) useItemButton.onClick.AddListener(ShowItemUsePanel);
        
        if (consumeItemButton != null) consumeItemButton.onClick.RemoveAllListeners();
        if (consumeItemButton != null) consumeItemButton.onClick.AddListener(() => OnUseItemClicked(false, currentPlayerStats));
        
        if (shareItemButton != null) shareItemButton.onClick.RemoveAllListeners();
        if (shareItemButton != null) shareItemButton.onClick.AddListener(ShowPlayerSelectionForShare);
        
        if (itemUseChoicePanel != null) itemUseChoicePanel.SetActive(false);

        popupPanel.SetActive(false);

        if (inventoryCloseBtn != null) inventoryCloseBtn.onClick.AddListener(CloseInventory);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (itemDetailPanel != null) itemDetailPanel.SetActive(false);
        
        SetHealButtonsVisible(false);
    }
    
    private void SetHealButtonsVisible(bool visible)
    {
        if (healButtonsContainer != null) healButtonsContainer.SetActive(visible);

        if (!visible)
        {
            foreach(Button btn in playerHealButtons)
            {
                if (btn != null) btn.gameObject.SetActive(false);
            }
        }
    }

    public void ShowInventory(PlayerStats player)
    {
        if (inventoryPanel == null) return;

        currentPlayerStats = player; 

        inventoryPanel.SetActive(true);
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
        if (itemDetailPanel == null) 
        {
            Debug.LogError("ItemDetailPanel is NOT linked in the Inspector!");
            return; 
        }

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
                
                if (buttonText != null)
                    buttonText.text = "Use Item";
            }
        }
        itemUsedName = name;
        itemUsedIcon = icon;
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
        
        SetHealButtonsVisible(false);
    }

    public void ShowItemUsePanel()
    {
        if (itemUsedName == null) return;
        itemEnergyValue = 7; 

        CloseInventory();
        
        string title = $"Use {itemUsedName}?";
        string descriptionChoice = $"Choose to Consume for yourself (+{itemEnergyValue} Energy), or Share with a teammate (+10 Global Morale).";
        
        CleanUpAllCardUI();
        
        if (popupPanel != null) popupPanel.SetActive(true);
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

        if (consumeItemButton != null)
        {
            consumeItemButton.GetComponentInChildren<TextMeshProUGUI>().text = "Consume It";
        }
        if (shareItemButton != null)
        {
            shareItemButton.GetComponentInChildren<TextMeshProUGUI>().text = "Share It";
        }
    }

    public void ShowPlayerSelectionForShare()
    {
        if (turnManager == null || playerHealButtons == null) 
        {
            Debug.LogError("Heal button setup missing in Inspector! Cannot show share option.");
            return;
        }
        
        CleanUpAllCardUI();
        if (itemUseChoicePanel != null) itemUseChoicePanel.SetActive(false);
        
        if (simpleCloseButton != null) simpleCloseButton.gameObject.SetActive(false);
        
        if (popupPanel != null) popupPanel.SetActive(false); 

        SetHealButtonsVisible(true);
        
        List<PlayerStats> allPlayers = turnManager.GetAllActivePlayers(); 
        
        for (int i = 0; i < playerHealButtons.Count; i++)
        {
            Button healButton = playerHealButtons[i];
            PlayerStats targetPlayer = (i < allPlayers.Count) ? allPlayers[i] : null;

            if (healButton == null) continue;

            if (targetPlayer != null && targetPlayer != currentPlayerStats)
            {
                healButton.gameObject.SetActive(true);
                healButton.onClick.RemoveAllListeners();
                
                healButton.onClick.AddListener(() => OnUseItemClicked(true, targetPlayer));
            }
            else
            {
                healButton.gameObject.SetActive(false); 
            }
        }
    }

    private void OnUseItemClicked(bool shareForMorale, PlayerStats targetPlayer)
    {
        if (currentPlayerStats == null || itemUsedName == null) return;

        if (shareForMorale || targetPlayer == currentPlayerStats)
            currentPlayerStats.ChangeCropCount(itemUsedName, -1);

        if (shareForMorale)
        {
            if (targetPlayer != null && disasterManager != null)
            {
                disasterManager.AddGlobalMorale(10);
                targetPlayer.ChangeEnergy(itemEnergyValue); 
                simpleDescriptionText.text = $"{itemUsedName} Shared with {targetPlayer.playerName}! +10 Global Morale and +{itemEnergyValue} Energy for {targetPlayer.playerName} applied! \n(Closing in 2 seconds...)";
            }
        }
        else
        {
            currentPlayerStats.ChangeEnergy(itemEnergyValue);
            simpleDescriptionText.text = $"{itemUsedName} Consumed! +{itemEnergyValue} Energy applied! \n(Closing in 2 seconds...)";
        }
        
        if (itemUseChoicePanel != null) itemUseChoicePanel.SetActive(false); 
        
        SetHealButtonsVisible(false); 
        
        if (popupPanel != null) popupPanel.SetActive(true);
        
        if (titleText != null) titleText.gameObject.SetActive(false); 

        if (simpleDescriptionText != null) simpleDescriptionText.gameObject.SetActive(true);
        if (simpleCloseButton != null) simpleCloseButton.gameObject.SetActive(false);
        
        itemUsedName = null;
        itemUsedIcon = null;
        
        StartCoroutine(ClosePopupDelayed(2f));
    }

    public void CloseInventory()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(false); 
        if (itemDetailPanel != null) itemDetailPanel.SetActive(false); 
    }

    public void ShowPunishmentChoice(string title, PlayerStats player)
    {
        CleanUpAllCardUI(); 

        currentPlayerStats = player;
        if (titleText != null) 
        {
            titleText.gameObject.SetActive(true);
            titleText.text = title;
        }
        
        isMinigamePopup = false;

        if (itemUseChoicePanel != null) itemUseChoicePanel.SetActive(false);
        
        cardChoiceContainer.SetActive(true);
        cardRevealPanel.SetActive(false);
        simpleDescriptionText.gameObject.SetActive(false);
        simpleCloseButton.gameObject.SetActive(false);

        foreach (Transform child in cardChoiceContainer.transform)
            Destroy(child.gameObject);

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
        CleanUpAllCardUI(); 

        currentPlayerStats = player;
        if (titleText != null) 
        {
            titleText.gameObject.SetActive(true);
            titleText.text = title;
        }

        isMinigamePopup = false;

        if (itemUseChoicePanel != null) itemUseChoicePanel.SetActive(false);

        cardChoiceContainer.SetActive(true);
        cardRevealPanel.SetActive(false);
    
        simpleDescriptionText.gameObject.SetActive(false);
        simpleCloseButton.gameObject.SetActive(false);

        foreach (Transform child in cardChoiceContainer.transform)
            Destroy(child.gameObject);

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
        CleanUpAllCardUI(); 

        currentPlayerStats = player;
        
        if (titleText != null) 
        {
            titleText.gameObject.SetActive(true);
            titleText.text = title;
        }

        isMinigamePopup = true;

        if (itemUseChoicePanel != null) itemUseChoicePanel.SetActive(false);

        cardChoiceContainer.SetActive(true);
        cardRevealPanel.SetActive(false);
        simpleDescriptionText.gameObject.SetActive(false);
        simpleCloseButton.gameObject.SetActive(false);

        foreach (Transform child in cardChoiceContainer.transform)
            Destroy(child.gameObject);

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
        CleanUpAllCardUI(); 

        currentPlayerStats = player;
        
        if (titleText != null) 
        {
            titleText.gameObject.SetActive(true);
            titleText.text = title;
        }

        isMinigamePopup = false; 

        if (itemUseChoicePanel != null) itemUseChoicePanel.SetActive(false);

        cardChoiceContainer.SetActive(true);
        cardRevealPanel.SetActive(false);
    
        simpleDescriptionText.gameObject.SetActive(false);
        simpleCloseButton.gameObject.SetActive(false);

        foreach (Transform child in cardChoiceContainer.transform)
            Destroy(child.gameObject);

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
        CleanUpAllCardUI();
        
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
                    break;
                case "NipaLeaves":
                    currentPlayerStats.nipaLeaves += storedRewardItem.quantity;
                    isStored = true;
                    break;
                case "Hardwood":
                    currentPlayerStats.hardwood += storedRewardItem.quantity;
                    isStored = true;
                    break;
                case "Sawali":
                    currentPlayerStats.sawali += storedRewardItem.quantity;
                    isStored = true;
                    break;
                
                case "Talong": case "Singkamas": case "Mani": case "Kundol":
                case "Patola": case "Upo": case "Kalabasa": case "Labanos":
                case "Mustasa": case "Luya": case "Ampalaya": case "Sitaw":
                case "Bataw": case "Patani": case "Sigarilyas": case "Sibuyas":
                case "Bawang": case "Carrots": case "Patatas": case "Kamatis":
                case "Blueberry": case "Mangga": case "Mansanas": case "Pina":
                case "Orange": case "Mangosten": case "Lemon": case "Longan":
                case "Loquat":
                    currentPlayerStats.ChangeCropCount(storedRewardItem.itemType, storedRewardItem.quantity);
                    isStored = true;
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
        if (storedPunishmentEvent != null && currentPlayerStats != null)
        {
            storedPunishmentEvent.Invoke(currentPlayerStats);
        }
        
        CleanUpAllCardUI();
        if (itemUseChoicePanel != null) itemUseChoicePanel.SetActive(false);
        SetHealButtonsVisible(false);

        popupPanel.SetActive(false);
        
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
    }
}