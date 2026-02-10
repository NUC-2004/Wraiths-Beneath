using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // 方法1：直接加载指定场景（可以通过Inspector设置场景名称）
    public void LoadSceneByName(string sceneName)
    {
        if (SceneExists(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' not found in Build Settings!");
        }
    }
    
    // 方法2：为不同按钮分别创建方法
    public void LoadGameScene()
    {
        LoadSceneByName("Mechanic2 Scene 1");
    }
    
    public void LoadOptionScene()
    {
        LoadSceneByName("OptionScene");
    }
    
    // 方法3：使用场景索引（确保Build Settings中顺序正确）
    [Header("场景索引设置")]
    public int gameSceneIndex = 1;    // MapTestScene的索引
    public int optionSceneIndex = 2;  // OptionScene的索引
    
    public void LoadGameSceneByIndex()
    {
        if (gameSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(gameSceneIndex);
        }
        else
        {
            Debug.LogError($"Scene index {gameSceneIndex} is out of range!");
        }
    }
    
    public void LoadOptionSceneByIndex()
    {
        if (optionSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(optionSceneIndex);
        }
        else
        {
            Debug.LogError($"Scene index {optionSceneIndex} is out of range!");
        }
    }
    
    // 检查场景是否存在
    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string scene = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (scene == sceneName)
                return true;
        }
        return false;
    }
    
    // 退出游戏
    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}