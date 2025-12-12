using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class BuildCard : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI titleText;
    public Image cardImage;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI descriptionText;
    public Button buildButton;

    public void Initialize(string title, string cost, string description, Sprite image, Action onClick)
    {
        if (titleText != null) titleText.text = title;
        if (costText != null) costText.text = cost;
        if (descriptionText != null) descriptionText.text = description;
        if (cardImage != null) cardImage.sprite = image;

        if (buildButton != null)
        {
            buildButton.onClick.RemoveAllListeners();
            buildButton.onClick.AddListener(() => onClick());
        }
    }
}