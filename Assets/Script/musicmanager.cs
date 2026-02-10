using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // 引用列表所需的命名空間

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource musicSource;

    [Header("遊戲場景名單 (在這些場景中不播音樂)")]
    public List<string> gameScenes = new List<string> { "Mechanic Scene" };

    void Awake()
    {
        // 確保單例，防止切換場景時產生多個音樂實體
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 檢查當前載入的場景是否在黑名單中
        bool isGameScene = false;
        foreach (string gScene in gameScenes)
        {
            // 使用 Trim 確保不會因為手抖多打了空格而匹配失敗
            if (scene.name.Trim() == gScene.Trim())
            {
                isGameScene = true;
                break;
            }
        }

        if (isGameScene)
        {
            // 如果進入了遊戲場景，停止音樂
            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.Stop(); 
                Debug.Log("检测到游戏场景 [" + scene.name + "]，背景音乐停止");
            }
        }
        else
        {
            // 如果回到了 Menu, Option 等場景，播放音樂
            if (musicSource != null && !musicSource.isPlaying)
            {
                musicSource.Play();
                Debug.Log("回到非游戏场景 [" + scene.name + "]，背景音乐开启");
            }
        }
    }
}