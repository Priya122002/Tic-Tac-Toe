using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Source")]
    public AudioSource uiSource;

    [Header("UI Sounds")]
    public AudioClip[] clips;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void Play(string clipName)
    {
        if (uiSource == null || clips == null) return;

        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] != null && clips[i].name == clipName)
            {
                uiSource.PlayOneShot(clips[i]);
                return;
            }
        }

    }
}