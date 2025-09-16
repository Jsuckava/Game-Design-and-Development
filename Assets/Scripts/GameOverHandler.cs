using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverHandler : MonoBehaviour
{
  [SerializeField] private string gameOverScene = "MainMenuProto";

  public void HandleGameOver()
  {
    if (LivesManager.Instance == null)
    {
      return;
    }

    LivesManager.Instance.LoseLife(1);

    if (LivesManager.Instance.Lives <= 0)
    {
      SceneManager.LoadScene(gameOverScene);
    }
    else
    {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
  }
}
