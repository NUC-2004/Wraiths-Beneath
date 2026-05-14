using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("场景索引设置")]
    public int gameSceneIndex = 1;
    public int optionSceneIndex = 2;

    public void LoadSceneByName(string sceneName)
    {
        if (!SceneExists(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' not found in Build Settings!");
            return;
        }

        AudioState.ResumeGlobalAudio();
        SceneManager.LoadScene(sceneName);
    }

    public void LoadGameScene()
    {
        LoadSceneByName("1");
    }

    public void LoadOptionScene()
    {
        LoadSceneByName("OptionScene");
    }

    public void LoadGameSceneByIndex()
    {
        LoadSceneByIndex(gameSceneIndex);
    }

    public void LoadOptionSceneByIndex()
    {
        LoadSceneByIndex(optionSceneIndex);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void LoadSceneByIndex(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"Scene index {sceneIndex} is out of range!");
            return;
        }

        AudioState.ResumeGlobalAudio();
        SceneManager.LoadScene(sceneIndex);
    }

    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string scene = Path.GetFileNameWithoutExtension(scenePath);
            if (scene == sceneName)
            {
                return true;
            }
        }

        return false;
    }
}
