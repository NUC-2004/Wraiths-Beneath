using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement; // 必须引用场景管理命名空间
using System.Collections;

public class DeathScreenController : MonoBehaviour
{
    [Header("组件引用")]
    public VideoPlayer videoPlayer;    // 拖入 WinVideoBackground 或 VideoBackground
    public GameObject uiPanel;         // 拖入包含按钮的父物体（如 WinButtons 或 MenuUI）

    void OnEnable()
    {
        // 1. 场景名称检测：只有当前场景名为 "3" 时才执行逻辑
        if (SceneManager.GetActiveScene().name != "3")
        {
            return; 
        }

        // 2. 初始状态设置：隐藏按钮面板
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }

        // 3. 全局静音逻辑：暂停所有背景音效
        // 提示：确保 Video Player 的 Audio Output Mode 设置为 Direct，否则视频也没声音
        AudioListener.pause = true; 

        // 4. 绑定视频结束事件并播放
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // 5. 视频播放完：显示按钮面板，但不在这里恢复声音
        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
        }
        
        videoPlayer.loopPointReached -= OnVideoFinished;
    }

    // --- 供按钮点击调用的新方法 ---
    public void ManualReturnToMenu(string menuSceneName)
    {
        // 6. 手动点击按钮时：先恢复全局声音，再跳转场景
        AudioListener.pause = false; 
        SceneManager.LoadScene(menuSceneName);
    }
}