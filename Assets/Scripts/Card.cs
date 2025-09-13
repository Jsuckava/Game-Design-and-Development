// using PrimeTween;
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
    // Tween.Rotation(transform, endValue: Quaternion.Euler(0, 90, 0), duration: 1);
    iconImage.sprite = iconSprite;
    isSelected = true;
  }

  public void Hide()
  {
    iconImage.sprite = hiddenSprite;
    isSelected = false;
  }
}
