using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;

public class XoxManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupBoard();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Button[] tilesButton;
    public TextMeshProUGUI winnerText;

    private string playerSide = "X";
    private int moveCount = 0;
    private string[] boardPanel = new string[9];

    void SetupBoard() {
        playerSide = "X";
        moveCount = 0;
        winnerText.gameObject.SetActive(false);

        for (int i = 0; i < tilesButton.Length; i++)
        {
            boardPanel[i] = "";
            tilesButton[i].interactable = true;
            tilesButton[i].GetComponentInChildren<TextMeshProUGUI>().text = "";

            int index = i;
            tilesButton[i].onClick.RemoveAllListeners();
            tilesButton[i].onClick.AddListener(() => OnTileClicked(index));
        }
    }

    public void OnTileClicked(int index) {
        if (boardPanel[index] != "" || !tilesButton[index].interactable || playerSide == "O") return ;

        boardPanel[index] = playerSide;
        tilesButton[index].GetComponentInChildren<TextMeshProUGUI>().text = playerSide;
        tilesButton[index].interactable = false;
        moveCount++;

        if (CheckForWin())
        {
            GameOver(playerSide);
        }
        else if (moveCount >= 9)
        {
            GameOver("Draw");
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

        boardPanel[bestMove] = playerSide;
        tilesButton[bestMove].GetComponentInChildren<TextMeshProUGUI>().text = playerSide;
        tilesButton[bestMove].interactable = false;
        moveCount++;

        if (CheckForWin())
        {
            GameOver(playerSide);
        }
        else if (moveCount >= 9)
        {
            GameOver("Draw");
        }
        else
        {
            ChangePlayerSide();
        }

        // OnTileClicked(bestMove);
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
        if (result == "Draw")
        {
            winnerText.text = "It's a Draw!";
            StartCoroutine(RestartDelay());
        }
        else if (result == "O")
        {
            winnerText.text = "You Lose!";
        }
        else if (result == "X")
        {
            winnerText.text = "You Win!";
        }
    }
    
    IEnumerator RestartDelay()
    {
        yield return new WaitForSeconds(2f);
        RestartGame();
    }

    public void RestartGame()
    {
        SetupBoard();
    }
}
