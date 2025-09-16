using UnityEngine;
using UnityEngine.Events;

public class LivesManager : MonoBehaviour
{
  public static LivesManager Instance { get; private set; }

  [SerializeField] private int startingLives = 10;

  public UnityEvent<int> OnLiveChanged;
  public UnityEvent OnGameOver;

  public int Lives { get; private set; }

  void Awake()
  {
    if (Instance != null && Instance != this)
    {
      Destroy(gameObject);
      return;
    }
    Instance = this;
    DontDestroyOnLoad(gameObject);
  }

  void Start()
  {
    ResetLives();
  }

  public void ResetLives()
  {
    Lives = Mathf.Max(0, startingLives);
    OnLiveChanged?.Invoke(Lives);
  }

  public void AddLife(int amount = 1)
  {
    Lives += amount;
    OnLiveChanged?.Invoke(Lives);
  }

  public void LoseLife(int amount = 1)
  {
    Lives = Mathf.Max(0, Lives - amount);
    OnLiveChanged?.Invoke(Lives);

    if (Lives <= 0)
    {
      OnGameOver?.Invoke();
    }
  }
}
