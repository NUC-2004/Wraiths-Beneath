using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource musicSource;

    [Header("游戏场景名单 (在这些场景中不播音乐)")]
    public List<string> gameScenes = new List<string> { "Mechanic Scene" };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioState.ResumeGlobalAudio();

        if (musicSource == null)
        {
            return;
        }

        if (IsGameScene(scene.name))
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }

            return;
        }

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    private bool IsGameScene(string sceneName)
    {
        foreach (string gameScene in gameScenes)
        {
            if (sceneName.Trim() == gameScene.Trim())
            {
                return true;
            }
        }

        return false;
    }
}
