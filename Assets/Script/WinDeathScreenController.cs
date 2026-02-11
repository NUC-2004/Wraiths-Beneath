using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement; // 必须引用
using System.Collections;

public class WinDeathScreenController : MonoBehaviour
{
    [Header("组件引用")]
    public VideoPlayer videoPlayer;    // 拖入 VideoBackground
    public CanvasGroup uiCanvasGroup; // 拖入带有 Canvas Group 的 UI 父物体

    [Header("设置")]
    public float fadeInDuration = 1.5f; // 淡入持续时间（秒）

    void OnEnable()
    {
        // 初始状态：隐藏 UI，注册视频结束事件
        if (uiCanvasGroup != null)
        {
            uiCanvasGroup.alpha = 0f;
            uiCanvasGroup.interactable = false;     // 未淡入前不可交互
            uiCanvasGroup.blocksRaycasts = false;   // 未淡入前不挡鼠标
        }

        // 绑定视频结束的回调函数
        videoPlayer.loopPointReached += OnVideoFinished;
        
        // 开始播放视频
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // 视频播完，启动淡入协程
        StartCoroutine(FadeInUI());
        
        // 取消事件绑定，防止重复触发
        videoPlayer.loopPointReached -= OnVideoFinished;
    }

    IEnumerator FadeInUI()
    {
        float currentTime = 0f;

        while (currentTime < fadeInDuration)
        {
            currentTime += Time.deltaTime;
            // 平滑计算当前的 Alpha 值
            if (uiCanvasGroup != null)
            {
                uiCanvasGroup.alpha = Mathf.Lerp(0f, 1f, currentTime / fadeInDuration);
            }
            yield return null; // 等待下一帧
        }

        // 确保最终状态完全显示，并开启交互
        if (uiCanvasGroup != null)
        {
            uiCanvasGroup.alpha = 1f;
            uiCanvasGroup.interactable = true;
            uiCanvasGroup.blocksRaycasts = true;
        }
    }
}