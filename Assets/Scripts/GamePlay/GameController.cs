using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    [Header("Scene References")]
    public CellButton[] cells;
    public TextMeshProUGUI statusText;

    GameState gameState;
    string currentPlayer;
    int[] board = new int[9];
    public RectTransform winLine;

    void Start()
    {
        Debug.Log("Mode: " + GameManager.Instance.currentMode);
        InitializeGame();
    }

    void InitializeGame()
    {
        winLine.gameObject.SetActive(false);
        gameState = GameState.Playing;
        currentPlayer = "X";
        statusText.text = "Player X Turn";

        for (int i = 0; i < board.Length; i++)
            board[i] = 0;

        foreach (var cell in cells)
        {
            cell.SetController(this);
            cell.ResetCell();
        }
    }

    public void OnCellSelected(CellButton cell)
    {
        if (gameState != GameState.Playing)
            return;

        // Prevent clicking during computer turn
        if (GameManager.Instance.currentMode == GameMode.PlayerVsComputer &&
            currentPlayer == "O")
            return;

        PlayMove(cell, currentPlayer == "X" ? 1 : 2, currentPlayer);

        if (gameState != GameState.Playing)
            return;

        SwitchPlayer();

        // 🔹 Computer move
        if (GameManager.Instance.currentMode == GameMode.PlayerVsComputer &&
            currentPlayer == "O")
        {
            Invoke(nameof(ComputerMove), 0.5f);
        }
    }

    void PlayMove(CellButton cell, int value, string symbol)
    {
        board[cell.cellIndex] = value;
        cell.SetValue(symbol);

        if (CheckWin(value))
        {
            gameState = value == 1 ? GameState.X_Won : GameState.O_Won;
            statusText.text = symbol == "X"
                ? "Player Wins!"
                : "Computer Wins!";
            return;
        }

        if (CheckDraw())
        {
            gameState = GameState.Draw;
            statusText.text = "It's a Draw!";
        }
    }
    int FindBestMove(int playerValue)
    {
        int[,] patterns =
        {
        {0,1,2},{3,4,5},{6,7,8},
        {0,3,6},{1,4,7},{2,5,8},
        {0,4,8},{2,4,6}
    };

        for (int i = 0; i < 8; i++)
        {
            int a = patterns[i, 0];
            int b = patterns[i, 1];
            int c = patterns[i, 2];

            int count = 0;
            int empty = -1;

            if (board[a] == playerValue) count++; else if (board[a] == 0) empty = a;
            if (board[b] == playerValue) count++; else if (board[b] == 0) empty = b;
            if (board[c] == playerValue) count++; else if (board[c] == 0) empty = c;

            if (count == 2 && empty != -1)
                return empty;
        }

        return -1;
    }

    void ComputerMove()
    {
        if (gameState != GameState.Playing)
            return;

        int move = FindBestMove(2);
        if (move == -1)
        {
            move = FindBestMove(1);
        }
        if (move == -1)
        {
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == 0)
                {
                    move = i;
                    break;
                }
            }
        }

        PlayMove(cells[move], 2, "O");

        if (gameState == GameState.Playing)
        {
            currentPlayer = "X";
            statusText.text = "Player X Turn";
        }
    }

    void SwitchPlayer()
    {
        currentPlayer = currentPlayer == "X" ? "O" : "X";
        statusText.text = currentPlayer == "X"
            ? "Player X Turn"
            : (GameManager.Instance.currentMode == GameMode.PlayerVsComputer
                ? "Computer Turn"
                : "Player O Turn");
    }

    int CheckWin(int v)
    {
        int[,] p =
        {
        {0,1,2},{3,4,5},{6,7,8},
        {0,3,6},{1,4,7},{2,5,8},
        {0,4,8},{2,4,6}
    };

        for (int i = 0; i < 8; i++)
        {
            if (board[p[i, 0]] == v &&
                board[p[i, 1]] == v &&
                board[p[i, 2]] == v)
                return i; 
        }

        return -1;
    }
    bool CheckDraw()
    {
        foreach (int c in board)
            if (c == 0)
                return false;
        return true;
    }
    public void RestartGame()
    {
        InitializeGame();
    }

}
