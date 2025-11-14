using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class PopupController : MonoBehaviour
{
    [Header("Main Panel")]
    public GameObject popupPanel;
    public TextMeshProUGUI titleText;

    [Header("Component Links")]
    public PunishmentDeck punishmentDeck; 

    [Header("Card Choice")]
    public GameObject cardChoiceContainer;
    public GameObject cardBackPrefab_Disaster; 
    public GameObject cardBackPrefab_Wild; 

    [Header("Card Reveal")]
    public GameObject cardRevealPanel;
    public Image revealImage; 
    public TextMeshProUGUI revealTitleText;
    public TextMeshProUGUI revealDescriptionText;
    public Button revealCloseButton;

    [Header("Simple Popup")]
    public TextMeshProUGUI simpleDescriptionText;
    public Button simpleCloseButton;

    private TurnManager turnManager;
    private PlayerStats currentPlayerStats;

    void Start()
    {
        turnManager = FindFirstObjectByType<TurnManager>();
        
        if (punishmentDeck == null)
        {
            Debug.LogError("CRITICAL ERROR: The PunishmentDeck is NOT linked in the PopupController Inspector!", this.gameObject);
        }

        revealCloseButton.onClick.AddListener(ClosePopup); 
        simpleCloseButton.onClick.AddListener(ClosePopup); 
        
        popupPanel.SetActive(false); 
    }

    public void ShowPunishmentChoice(string title, PlayerStats player)
    {
        currentPlayerStats = player;
        titleText.text = title;

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
            cardButton.onClick.AddListener(() => {
                OnCardSelected(punishment);
            });
        }
        popupPanel.SetActive(true);
    }
    
    public void ShowMaterialChoice(string title, PlayerStats player)
    {
        currentPlayerStats = player;
        titleText.text = title;

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
            cardButton.onClick.AddListener(() => {
                OnCardSelected(material);
            });
        }
        popupPanel.SetActive(true);
    }
    
    public void ShowSimplePopup(string title, string description)
    {
        titleText.text = title;
        simpleDescriptionText.text = description;

        cardChoiceContainer.SetActive(false);
        cardRevealPanel.SetActive(false);
        simpleDescriptionText.gameObject.SetActive(true);
        simpleCloseButton.gameObject.SetActive(true);

        popupPanel.SetActive(true);
    }

    private void OnCardSelected(Punishment punishment)
    {
        cardChoiceContainer.SetActive(false);
        simpleDescriptionText.gameObject.SetActive(false);
        simpleCloseButton.gameObject.SetActive(false);
        
        cardRevealPanel.SetActive(true);
        revealImage.sprite = punishment.cardArt; 
        revealTitleText.text = punishment.title;
        revealDescriptionText.text = punishment.description;

        if (currentPlayerStats != null)
        {
            punishment.onPunishmentSelected.Invoke(currentPlayerStats);
        }
    }

    private void ClosePopup()
    {
        popupPanel.SetActive(false);
        currentPlayerStats = null;
        
        if (turnManager != null)
        {
            turnManager.EndTurn();
        }
    }
}   