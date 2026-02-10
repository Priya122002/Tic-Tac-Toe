using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void StartPvP()
    {
        StartGame(GameMode.PlayerVsPlayer);
    }

    public void StartPvC()
    {
        StartGame(GameMode.PlayerVsComputer);
    }

    void StartGame(GameMode mode)
    {
        SoundManager.Instance.Play("tap");
        GameManager.Instance.currentMode = mode;
        SceneManager.LoadScene("Game");
    }
}
