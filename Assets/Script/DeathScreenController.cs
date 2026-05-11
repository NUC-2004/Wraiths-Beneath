using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class DeathScreenController : MonoBehaviour
{
    [Header("组件引用")]
    public VideoPlayer videoPlayer;
    public GameObject uiPanel;

    void OnEnable()
    {
        if (SceneManager.GetActiveScene().name != "3")
        {
            return;
        }

        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }

        AudioListener.pause = true;

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

    public void ManualReturnToMenu(string menuSceneName)
    {
        AudioListener.pause = false;
        SceneManager.LoadScene(menuSceneName);
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}
