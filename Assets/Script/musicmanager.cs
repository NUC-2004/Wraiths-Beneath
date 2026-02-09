using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource musicSource;
    // 💡 确保这里和你的场景文件名完全一致（包含空格）
    public string gameSceneName = "Mechanic Scene"; 

    void Awake()
    {
        // --- 彻底解决重叠问题的单例逻辑 ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // 如果回到菜单发现已经有一个在播了，就毁掉新的，留下旧的
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
        // 打印一下，看看 Unity 实际读取到的名字里有没有多余空格
        Debug.Log("MusicManager 正在检查场景: [" + scene.name + "]");

        // 使用 Trim() 去掉可能存在的首尾空格，防止手动输入错误
        if (scene.name.Trim() == gameSceneName.Trim())
        {
            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.Pause(); // 暂停游戏背景音乐
                Debug.Log("进入游戏场景，背景音乐已暂停");
            }
        }
        else
        {
            if (musicSource != null && !musicSource.isPlaying)
            {
                musicSource.Play(); // 回到菜单，继续播放
                Debug.Log("离开游戏场景，背景音乐恢复");
            }
        }
    }
}