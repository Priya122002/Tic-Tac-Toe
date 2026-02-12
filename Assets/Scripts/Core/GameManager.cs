using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameMode currentMode;
    public AIDifficulty selectedDifficulty = AIDifficulty.Medium;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
