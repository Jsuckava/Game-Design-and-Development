using UnityEngine;

public class UI_ActionManager : MonoBehaviour
{
    private GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.Log("GameManager not found");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HandleAttack()
    {
        if (gameManager == null) return;

        if (gameManager.currentState == GameState.Player1Turn)
        {
            gameManager.OnPlayer1AttackButton1Pressed();
        }
        else if (gameManager.currentState == GameState.Player2Turn)
        {
            gameManager.OnPlayer2AttackButton1Pressed();
        }
    }

    public void HandleKick()
    {
        if (gameManager == null) return;

        if (gameManager.currentState == GameState.Player1Turn)
        {
            gameManager.OnPlayer1KickPressed();
        }
        else if (gameManager.currentState == GameState.Player2Turn)
        {
            gameManager.OnPlayer2KickPressed();
        }

    }
     public void HandleSuper()
    {
        if (gameManager == null) return;

        if (gameManager.currentState == GameState.Player1Turn)
        {
            gameManager.OnPlayer1SuperPressed();
        }
        else if (gameManager.currentState == GameState.Player2Turn)
        {
            gameManager.OnPlayer2SuperPressed();
        }
        
    }

    // public void HandleDefend()
    // {
    //     if (gameManager == null) return;

    //     if (gameManager.currentState == GameState.Player1Turn)
    //     {
    //         Debug.Log("Player 1 Defend");
    //         gameManager.ChangeState(GameState.Player2Turn);
    //     }
    //     else if (gameManager.currentState == GameState.Player2Turn)
    //     {
    //         Debug.Log("Player 2 Defend");
    //         gameManager.ChangeState(GameState.Player1Turn);
    //     }
    // }
}
