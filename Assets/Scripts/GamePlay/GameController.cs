using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum AIDifficulty
{
    Easy,
    Medium,
    Hard
}

public class GameController : MonoBehaviour
{
    [Header("Board References")]
    public CellButton[] cells;
    public RectTransform boardRoot;

    [Header("UI")]
    public TextMeshProUGUI statusText;          
    public Image winLineImage;                 
    public GameObject statusPanel;
    [Header("Status Panel Texts")]
    public TextMeshProUGUI statusTitleText;
    public TextMeshProUGUI statusCommentText;


    [Header("Win Line Animation")]
    public float strikeDuration = 1.2f;

    [Header("Versus Panel")]
    public Image rightPlayerImage;
    public Sprite personSprite;
    public Sprite AISprite;
    [Header("Turn Card Sprites")]
    public Image playerXCard;
    public Image playerOCard;

    public Sprite defaultCardSprite;  
    public Sprite activeXSprite;      
    public Sprite activeOSprite;      

    [Header("Turn Text")]
    public TextMeshProUGUI playerXTurnText;
    public TextMeshProUGUI playerOTurnText;

    public Color activeTextColor = Color.white;
    public Color inactiveTextColor = new Color(1f, 1f, 1f, 0.5f);


    [Header("Versus Turn Animation")]
    public RectTransform player1Image;
    public RectTransform player2Image;

    public float pulseScale = 1.2f;
    public float pulseSpeed = 0.5f;

    Coroutine pulseRoutine;
    [Header("Player Name Texts")]
    public TextMeshProUGUI playerXNameText;
    public TextMeshProUGUI playerONameText;

    [Header("Status Panel Animation")]
    public float panelAnimDuration = 0.25f;
    public float panelOvershoot = 1.05f;

    [Header("AI Difficulty")]
    public AIDifficulty aiDifficulty = AIDifficulty.Medium;


    CanvasGroup statusCanvasGroup;

    Coroutine winLineRoutine;
    Coroutine statusPanelRoutine;

    GameState gameState;
    string currentPlayer;
    int[] board = new int[9];

    void Start()
    {
        Debug.Log("Mode: " + GameManager.Instance.currentMode);
        SetupVersusPanel();
        InitializeGame();

    }

    void InitializeGame()
    {
        gameState = GameState.Playing;
        currentPlayer = "X";
        UpdateTurnUI();
        StartPulse(player1Image);

        for (int i = 0; i < board.Length; i++)
            board[i] = 0;

        foreach (var cell in cells)
        {
            cell.SetController(this);
            cell.ResetCell();
        }

        if (winLineImage != null)
        {
            winLineImage.gameObject.SetActive(false);
            winLineImage.fillAmount = 0f;
        }

        if (statusPanel != null)
        {
            statusCanvasGroup = statusPanel.GetComponent<CanvasGroup>();
            if (statusCanvasGroup == null)
                statusCanvasGroup = statusPanel.AddComponent<CanvasGroup>();

            statusCanvasGroup.alpha = 0f;
            statusPanel.transform.localScale = Vector3.one * 0.8f;
            statusPanel.SetActive(false);
        }
    }

    void SetupVersusPanel()
    {
        if (rightPlayerImage == null)
            return;

        if (GameManager.Instance.currentMode == GameMode.PlayerVsPlayer)
        {
            rightPlayerImage.sprite = personSprite;
            playerONameText.text = "Player O";
        }
        else 
        {
            rightPlayerImage.sprite = AISprite;
            playerONameText.text = "AI";
        }
    }



    public void OnCellSelected(CellButton cell)
    {
        if (gameState != GameState.Playing)
            return;

        if (GameManager.Instance.currentMode == GameMode.PlayerVsComputer &&
            currentPlayer == "O")
            return;

        PlayMove(cell, currentPlayer == "X" ? 1 : 2, currentPlayer);

        if (gameState != GameState.Playing)
            return;

        SwitchPlayer();

        if (GameManager.Instance.currentMode == GameMode.PlayerVsComputer &&
            currentPlayer == "O")
        {
            StartCoroutine(AIThinkAndMove());
        }
    }
    IEnumerator AIThinkAndMove()
    {
        float delay = Random.Range(0.7f, 1.2f);
        yield return new WaitForSeconds(delay);
        AIMove();
    }

    void PlayMove(CellButton cell, int value, string symbol)
    {
        StopPulse();
        board[cell.cellIndex] = value;
        cell.SetValue(symbol);

        int winIndex = CheckWin(value);
        if (winIndex != -1)
        {
            gameState = value == 1 ? GameState.X_Won : GameState.O_Won;

            string result =
                value == 1
                ? "Player 1 Wins!"
                : (GameManager.Instance.currentMode == GameMode.PlayerVsComputer
                    ? "AI Wins!"
                    : "Player 2 Wins!");

            ShowWinLine(winIndex);
            StartCoroutine(ShowResultAfterDelay(gameState));


            return;
        }

        if (CheckDraw())
        {
            gameState = GameState.Draw;
            ShowStatusPanel(GameState.Draw);
        }
    }
    IEnumerator ShowResultAfterDelay(GameState state)
    {
        yield return new WaitForSeconds(strikeDuration);
        ShowStatusPanel(state);
    }

    void SwitchPlayer()
    {
        currentPlayer = currentPlayer == "X" ? "O" : "X";
        UpdateTurnUI();

        if (currentPlayer == "X")
            StartPulse(player1Image);
        else
            StartPulse(player2Image);

    }


    void AIMove()
    {
        if (gameState != GameState.Playing)
            return;

        int move = -1;

        switch (aiDifficulty)
        {
            case AIDifficulty.Easy:
                move = GetRandomMove();
                break;

            case AIDifficulty.Medium:
                move = MediumMove();
                break;

            case AIDifficulty.Hard:
                move = HardMove();
                break;
        }

        PlayMove(cells[move], 2, "O");

        if (gameState == GameState.Playing)
        {
            currentPlayer = "X";
            UpdateTurnUI();
            StartPulse(player1Image);
        }
    }

    int GetRandomMove()
    {
        List<int> empty = new List<int>();

        for (int i = 0; i < board.Length; i++)
            if (board[i] == 0)
                empty.Add(i);

        return empty[Random.Range(0, empty.Count)];
    }
    int MediumMove()
    {
        int move = FindBestMove(2);

        if (move == -1)
            move = FindBestMove(1); 

        if (move == -1)
            move = GetRandomMove();

        return move;
    }
    int HardMove()
    {
        int move = FindBestMove(2);

        if (move == -1)
            move = FindBestMove(1);

        if (move == -1 && board[4] == 0)
            move = 4;

        if (move == -1)
        {
            int[] corners = { 0, 2, 6, 8 };
            foreach (int c in corners)
            {
                if (board[c] == 0)
                {
                    move = c;
                    break;
                }
            }
        }

        if (move == -1)
            move = GetRandomMove();

        return move;
    }


    int FindBestMove(int playerValue)
    {
        int[,] p =
        {
            {0,1,2},{3,4,5},{6,7,8},
            {0,3,6},{1,4,7},{2,5,8},
            {0,4,8},{2,4,6}
        };

        for (int i = 0; i < 8; i++)
        {
            int a = p[i, 0], b = p[i, 1], c = p[i, 2];
            int count = 0, empty = -1;

            if (board[a] == playerValue) count++; else if (board[a] == 0) empty = a;
            if (board[b] == playerValue) count++; else if (board[b] == 0) empty = b;
            if (board[c] == playerValue) count++; else if (board[c] == 0) empty = c;

            if (count == 2 && empty != -1)
                return empty;
        }

        return -1;
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
    void StartPulse(RectTransform target)
    {
        StopPulse();

        pulseRoutine = StartCoroutine(PulseImage(target));
    }

    void StopPulse()
    {
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }

        player1Image.localScale = Vector3.one;
        player2Image.localScale = Vector3.one;
    }

    IEnumerator PulseImage(RectTransform target)
    {
        Vector3 baseScale = Vector3.one * 0.8f;
        Vector3 maxScale = Vector3.one;

        while (true)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * pulseSpeed;
                target.localScale = Vector3.Lerp(baseScale, maxScale, t);
                yield return null;
            }

            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * pulseSpeed;
                target.localScale = Vector3.Lerp(maxScale, baseScale, t);
                yield return null;
            }
        }
    }
    void UpdateTurnUI()
    {
        bool isXTurn = currentPlayer == "X";

        playerXCard.sprite = isXTurn ? activeXSprite : defaultCardSprite;
        playerOCard.sprite = isXTurn ? defaultCardSprite : activeOSprite;

        playerXTurnText.color = isXTurn ? activeTextColor : inactiveTextColor;
        playerOTurnText.color = isXTurn ? inactiveTextColor : activeTextColor;
    }


    void ShowWinLine(int index)
    {
        if (winLineImage == null || boardRoot == null)
            return;

        RectTransform lineRT = winLineImage.rectTransform;
        winLineImage.gameObject.SetActive(true);
        winLineImage.fillAmount = 0f;

        lineRT.localPosition = Vector3.zero;
        lineRT.rotation = Quaternion.identity;

        RectTransform firstCell = cells[0].GetComponent<RectTransform>();
        float cellSize = firstCell.rect.width;
        float spacing = boardRoot.GetComponent<GridLayoutGroup>().spacing.x;
        float offset = cellSize + spacing;

        switch (index)
        {
            case 0: lineRT.localPosition = new Vector3(0, offset, 0);
                lineRT.localScale = new Vector3(1, -1, 1);
                break;
            case 1: lineRT.localPosition = Vector3.zero;
                lineRT.localScale = new Vector3(1, -1, 1);
                break;
            case 2: lineRT.localPosition = new Vector3(0, -offset, 0);
                lineRT.localScale = new Vector3(1, -1, 1);
                break;

            case 3:
                lineRT.localPosition = new Vector3(-offset, 0, 0);
                lineRT.rotation = Quaternion.Euler(0, 0, 90);
                lineRT.localScale = new Vector3(-1, 1, 1);
                break;
            case 4:
                lineRT.rotation = Quaternion.Euler(0, 0, 90);
                lineRT.localScale = new Vector3(-1, 1, 1);
                break;
            case 5:
                lineRT.localPosition = new Vector3(offset, 0, 0);
                lineRT.rotation = Quaternion.Euler(0, 0, 90);
                lineRT.localScale = new Vector3(-1, 1, 1);
                break;

            case 6:
                lineRT.rotation = Quaternion.Euler(0, 0, -45);
                lineRT.localScale = new Vector3(1, -1, 1);
                break;
            case 7:
                lineRT.rotation = Quaternion.Euler(0, 0, 45);
                lineRT.localScale = new Vector3(-1, 1, 1);
                break;
        }
        if (winLineRoutine != null)
            StopCoroutine(winLineRoutine);

        winLineRoutine = StartCoroutine(AnimateWinLine());

    }

    IEnumerator AnimateWinLine()
    {
        float t = 0f;
        while (t < strikeDuration)
        {
            t += Time.deltaTime;
            float eased = Mathf.SmoothStep(0f, 1f, t / strikeDuration);
            winLineImage.fillAmount = eased;
            yield return null;
        }
        winLineImage.fillAmount = 1f;
    }

    void ShowStatusPanel(GameState state)
    {
        StopPulse();
        SoundManager.Instance.Play("notification");

        if (statusPanel == null)
            return;

        statusPanel.SetActive(true);

        switch (state)
        {
            case GameState.X_Won:
                statusTitleText.text = "PLAYER X WINS!";
                statusCommentText.text = "Congratulations! Player X is the winner.";
                break;

            case GameState.O_Won:
                if (GameManager.Instance.currentMode == GameMode.PlayerVsComputer)
                {
                    statusTitleText.text = "AI WINS!";
                    statusCommentText.text = "Better luck next time!";
                }
                else
                {
                    statusTitleText.text = "PLAYER O WINS!";
                    statusCommentText.text = "Congratulations! Player O is the winner.";
                }
                break;

            case GameState.Draw:
                statusTitleText.text = "DRAW!";
                statusCommentText.text = "It's a tie! No winner this time.";
                break;
        }

        if (statusPanelRoutine != null)
            StopCoroutine(statusPanelRoutine);

        statusPanelRoutine = StartCoroutine(StatusPanelPopup());
    }

    IEnumerator StatusPanelPopup()
    {
        RectTransform rt = statusPanel.transform as RectTransform;

        float t = 0f;
        Vector3 startScale = Vector3.one * 0.8f;
        Vector3 overshootScale = Vector3.one * panelOvershoot;
        Vector3 endScale = Vector3.one;

        while (t < 1f)
        {
            t += Time.deltaTime / panelAnimDuration;
            float eased = Mathf.SmoothStep(0f, 1f, t);

            rt.localScale = Vector3.Lerp(startScale, overshootScale, eased);
            statusCanvasGroup.alpha = eased;

            yield return null;
        }

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / (panelAnimDuration * 0.6f);
            rt.localScale = Vector3.Lerp(overshootScale, endScale, t);
            yield return null;
        }

        rt.localScale = Vector3.one;
        statusCanvasGroup.alpha = 1f;
    }


    public void RestartGame()
    {
        InitializeGame();
        SoundManager.Instance.Play("tap");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
        SoundManager.Instance.Play("tap");
    }
}
