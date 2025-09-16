using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
  [SerializeField] private Image iconImage;

  public Sprite hiddenSprite;
  public Sprite iconSprite;

  public bool isSelected;

  public CardsController controller;

  public void OnCardClick()
  {
    controller.SetSelected(this);
  }

  public void SetIconSprite(Sprite sprite)
  {
    iconSprite = sprite;
  }

  public void Show()
  {
    iconImage.sprite = iconSprite;
    isSelected = true;
  }

  public void Hide()
  {
    iconImage.sprite = hiddenSprite;
    isSelected = false;
  }
}
