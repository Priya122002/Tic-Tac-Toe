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

    void Start()
    {
        Debug.Log("Mode: " + GameManager.Instance.currentMode);
        InitializeGame();
    }

    void InitializeGame()
    {
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

        int value = currentPlayer == "X" ? 1 : 2;
        board[cell.cellIndex] = value;
        cell.SetValue(currentPlayer);

        if (CheckWin(value))
        {
            gameState = value == 1 ? GameState.X_Won : GameState.O_Won;
            statusText.text = $"Player {currentPlayer} Wins!";
            return;
        }

        if (CheckDraw())
        {
            gameState = GameState.Draw;
            statusText.text = "It's a Draw!";
            return;
        }

        SwitchPlayer();
    }

    void SwitchPlayer()
    {
        currentPlayer = currentPlayer == "X" ? "O" : "X";
        statusText.text = $"Player {currentPlayer} Turn";
    }

    bool CheckWin(int v)
    {
        int[,] p =
        {
            {0,1,2},{3,4,5},{6,7,8},
            {0,3,6},{1,4,7},{2,5,8},
            {0,4,8},{2,4,6}
        };

        for (int i = 0; i < 8; i++)
            if (board[p[i, 0]] == v &&
                board[p[i, 1]] == v &&
                board[p[i, 2]] == v)
                return true;

        return false;
    }

    bool CheckDraw()
    {
        foreach (int c in board)
            if (c == 0)
                return false;
        return true;
    }
}
