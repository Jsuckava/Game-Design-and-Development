using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class XoxManager : MonoBehaviour
{
    public Button[] tilesButton;
    public TextMeshProUGUI winnerText;

    private TextMeshProUGUI[] tilesText; 
    private string playerSide = "X";
    private int moveCount = 0;
    private string[] boardPanel = new string[9];
    
    private MinigameManager minigameManager;
    private int drawCount = 0; 

    void Start()
    {
        minigameManager = FindFirstObjectByType<MinigameManager>();
        if (minigameManager == null)
        {
            Debug.Log("XoxManager could not find MinigameManager!");
        }

        drawCount = 0; 

        tilesText = new TextMeshProUGUI[tilesButton.Length];
        for (int i = 0; i < tilesButton.Length; i++)
        {
            tilesText[i] = tilesButton[i].GetComponentInChildren<TextMeshProUGUI>();
            if (tilesText[i] == null)
            {
                Debug.LogError($"Button {i} is missing a TextMeshProUGUI child component!", tilesButton[i].gameObject);
            }
        }

        SetupBoard();
    }

    void Update()
    {

    }

    void SetupBoard() {
        playerSide = "X";
        moveCount = 0;
        winnerText.gameObject.SetActive(false);

        for (int i = 0; i < tilesButton.Length; i++)
        {
            boardPanel[i] = "";
            tilesButton[i].interactable = true;
            
            if (tilesText[i] != null) 
            {
                tilesText[i].text = "";
            }

            int index = i;
            tilesButton[i].onClick.RemoveAllListeners();
            tilesButton[i].onClick.AddListener(() => OnTileClicked(index));
        }
    }

    public void OnTileClicked(int index) {
        if (boardPanel[index] != "" || !tilesButton[index].interactable || playerSide == "O") return ;

        if (tilesText[index] == null) return;

        boardPanel[index] = playerSide;
        tilesText[index].text = playerSide; 
        tilesButton[index].interactable = false;
        moveCount++;

        if (CheckForWin())
        {
            GameOver(playerSide);
        }
        else if (moveCount >= 9)
        {
            drawCount++;
            if (drawCount >= 3)
            {
                GameOver("O"); 
            }
            else
            {
                StartCoroutine(RestartDelay()); 
            }
        }
        else
        {
            ChangePlayerSide();
            StartCoroutine(AiMove());
        }
    }

    IEnumerator AiMove()
    {
        yield return new WaitForSeconds(0.5f);

        int bestMove = FindBestMove();
        
        if (!winnerText.gameObject.activeInHierarchy)
        {
            if (tilesText[bestMove] == null)
            {
                Debug.LogError($"AI trying to move to {bestMove}, but it has no TextMeshProUGUI component.");
                yield break; 
            }

            boardPanel[bestMove] = playerSide;
            tilesText[bestMove].text = playerSide; 
            tilesButton[bestMove].interactable = false;
            moveCount++;
            
            if (CheckForWin())
            {
                GameOver(playerSide);
            }
            else if (moveCount >= 9)
            {
                drawCount++;
                if (drawCount >= 3)
                {
                    GameOver("O"); 
                }
                else
                {
                    StartCoroutine(RestartDelay());
                }
            }
            else
            {
                ChangePlayerSide();
            }
        }
        
    }

    int FindBestMove()
    {
        for (int i = 0; i < boardPanel.Length; i++)
        {
            if (boardPanel[i] == "")
            {
                boardPanel[i] = "O";
                if (CheckForWin())
                {
                    boardPanel[i] = "";
                    return i;
                }
                boardPanel[i] = "";
            }
        }
        
        for (int i = 0; i < boardPanel.Length; i++)
        {
            if (boardPanel[i] == "")
            {
                boardPanel[i] = "X";
                if (CheckForWin())
                {
                    boardPanel[i] = "";
                    return i;
                }
                boardPanel[i] = "";
            }
        }
        
        if (boardPanel[4] == "")
        {
            return 4;
        }
        
        List<int> corners = new List<int> { 0, 2, 6, 8 };
        corners = corners.OrderBy(x => Random.Range(0f, 1f)).ToList();
        foreach (int corner in corners)
        {
            if (boardPanel[corner] == "")
            {
                return corner;
            }
        }
        
        List<int> sides = new List<int> { 1, 3, 5, 7 };
        sides = sides.OrderBy(x => Random.Range(0f, 1f)).ToList();
        foreach (int side in sides)
        {
            if (boardPanel[side] == "")
            {
                return side;
            }
        }
        
        for (int i = 0; i < boardPanel.Length; i++)
        {
            if (boardPanel[i] == "") return i;
        }

        return 0;
    }


    void ChangePlayerSide() {
        playerSide = (playerSide == "X") ? "O" : "X";
    }

    bool CheckForWin() {
        if (CheckLine(0, 1, 2) || CheckLine(3, 4, 5) || CheckLine(6, 7, 8) ||
            CheckLine(0, 3, 6) || CheckLine(1, 4, 7) || CheckLine(2, 5, 8) ||
            CheckLine(0, 4, 8) || CheckLine(2, 4, 6))
        {
            return true;
        }
        return false;
    }

    bool CheckLine(int a, int b, int c) {
        return boardPanel[a] != "" && boardPanel[a] == boardPanel[b] && boardPanel[b] == boardPanel[c];
    }

    void GameOver(string result) {
        for (int i = 0; i < tilesButton.Length; i++)
        {
            tilesButton[i].interactable = false;
        }
        winnerText.gameObject.SetActive(true);
        bool playerWon = false;
        if (result == "Draw")
        {
            winnerText.text = "It's a Draw!";
            playerWon = false;
        }
        else if (result == "O")
        {
            winnerText.text = "You Lose!";
            playerWon = false;
        }
        else if (result == "X")
        {
            winnerText.text = "You Win!";
            playerWon = true;
        }
        
        StartCoroutine(CloseMiniGameAfterDelay(playerWon));
    }
    
    IEnumerator CloseMiniGameAfterDelay(bool didPlayerWin)
    {
        yield return new WaitForSeconds(2f);
        
        if (minigameManager != null)
        {
            minigameManager.OnMinigameCompleted(didPlayerWin);
        }
    }
    
    IEnumerator RestartDelay()
    {
        winnerText.gameObject.SetActive(true);
        winnerText.text = $"It's a Draw! Round {drawCount + 1}";

        yield return new WaitForSeconds(2f);
        RestartGame();
    }

    public void RestartGame()
    {
        SetupBoard();
    }
}