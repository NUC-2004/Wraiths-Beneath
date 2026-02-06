using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    // 1. 继续游戏
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f; // 恢复时间
        isPaused = false;
    }

    void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; // 停止时间
        isPaused = true;
    }

    // 2. 返回主菜单
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // 重要：切场景前必须恢复时间，否则新场景也是静止的
        SceneManager.LoadScene("MenuSecne"); // 确保名字和你 Hierarchy 里的场景名一致
    }

    // 3. 重新开始
    public void RestartGame()
    {
        Time.timeScale = 1f; // 恢复时间
        // 加载当前活跃的场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
