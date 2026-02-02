using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; // 用于重启游戏

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI; // 拖入刚才做的 Image
    public float scaleSpeed = 2f;
    private bool isGameOver = false;

    // 单例模式，方便其他脚本调用
    public static GameManager Instance;

    void Awake() { Instance = this; }

    // --- 核心逻辑：玩家输了 ---
    public void PlayerLost()
    {
        if (isGameOver) return;
        isGameOver = true;
        StartCoroutine(ShowGameOverAnimation());
    }

    // --- 核心逻辑：玩家赢了 ---
    public void PlayerWin()
    {
        if (isGameOver) return;
        isGameOver = true;
        // 这里可以执行加载下一关或显示胜利 UI
        Invoke("RestartGame", 0.1f); 
    }

    IEnumerator ShowGameOverAnimation()
    {
        gameOverUI.SetActive(true);
        Vector3 targetScale = new Vector3(10, 10, 1); // 放大到足够遮住屏幕
        
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * scaleSpeed;
            gameOverUI.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }

        // 动画播完，2秒后重启
        yield return new WaitForSeconds(2f);
        RestartGame();
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}