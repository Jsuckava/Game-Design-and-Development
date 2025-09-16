using UnityEngine;
using TMPro;

public class LivesUI : MonoBehaviour
{
  [SerializeField] TextMeshProUGUI livesText;

  void OnEnable()
  {
    if (LivesManager.Instance != null)
    {
      LivesManager.Instance.OnLiveChanged.AddListener(UpdateUI);
    }
  }

  void OnDisable()
  {
    if (LivesManager.Instance != null)
    {
      LivesManager.Instance.OnLiveChanged.RemoveListener(UpdateUI);
    }
  }

  void Start()
  {
    if (LivesManager.Instance != null)
    {
      UpdateUI(LivesManager.Instance.Lives);
    }
  }

  private void UpdateUI(int currentLives)
  {
    livesText.text = $"x {currentLives}";
  }
}
