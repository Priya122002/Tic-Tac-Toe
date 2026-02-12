using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject difficultyPanel;

    public void StartPvP()
    {
        SoundManager.Instance.Play("tap");
        GameManager.Instance.currentMode = GameMode.PlayerVsPlayer;
        SceneManager.LoadScene("Game");
    }

    public void OpenDifficultyPanel()
    {
        SoundManager.Instance.Play("tap");
        mainMenuPanel.SetActive(false);
        difficultyPanel.SetActive(true);
    }

    public void SelectEasy()
    {
        StartPvC(AIDifficulty.Easy);
    }

    public void SelectMedium()
    {
        StartPvC(AIDifficulty.Medium);
    }

    public void SelectHard()
    {
        StartPvC(AIDifficulty.Hard);
    }

    void StartPvC(AIDifficulty difficulty)
    {
        SoundManager.Instance.Play("tap");

        GameManager.Instance.currentMode = GameMode.PlayerVsComputer;
        GameManager.Instance.selectedDifficulty = difficulty;

        SceneManager.LoadScene("Game");
    }

    public void BackToMainMenu()
    {
        SoundManager.Instance.Play("tap");
        difficultyPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
