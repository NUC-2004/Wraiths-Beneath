using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class WinDeathScreenController : MonoBehaviour
{
    [Header("组件引用")]
    public VideoPlayer videoPlayer;
    public CanvasGroup uiCanvasGroup;

    [Header("设置")]
    public float fadeInDuration = 1.5f;

    void OnEnable()
    {
        SetCanvasVisible(false);

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.Play();
        }
    }

    void OnDisable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        StartCoroutine(FadeInUI());
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }

    private IEnumerator FadeInUI()
    {
        float currentTime = 0f;

        while (currentTime < fadeInDuration)
        {
            currentTime += Time.deltaTime;
            if (uiCanvasGroup != null)
            {
                uiCanvasGroup.alpha = Mathf.Lerp(0f, 1f, currentTime / fadeInDuration);
            }

            yield return null;
        }

        SetCanvasVisible(true);
    }

    private void SetCanvasVisible(bool visible)
    {
        if (uiCanvasGroup == null)
        {
            return;
        }

        uiCanvasGroup.alpha = visible ? 1f : 0f;
        uiCanvasGroup.interactable = visible;
        uiCanvasGroup.blocksRaycasts = visible;
    }
}
