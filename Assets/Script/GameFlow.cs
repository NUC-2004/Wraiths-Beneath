using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement; // 必须引入场景管理

public class GameFlowController : MonoBehaviour
{
    [Header("UI面板配置")]
    public GameObject pauseMenuPanel; // 拖入你的暂停菜单物体

    private bool isPaused = false;

    void Awake()
    {
        // 游戏启动时，确保时间流速正常，菜单关闭
        Time.timeScale = 1f;
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }

    void Update()
    {
        // 检测键盘 Esc 键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // --- 核心功能函数 ---

    // 1. 继续游戏
    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // 2. 暂停游戏
    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    // 3. 重新开始当前关卡
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 4. 返回主菜单 (根据你的截图，场景名为 MenuSecne)
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuSecne");
    }

    // 5. 跳转到 Mechanic Scene (响应 ResumeButton)
   public void JumpToMechanicScene()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Mechanic Scene"); 
    }

    // --- 新添加的功能 ---
    // 跳转到 Mechanic3 Scene 2
    public void JumpToMechanic3Scene2()
    {
        Time.timeScale = 1f; // 极其重要：跳转前必须恢复时间
        // 确保引号内的文字与你的场景文件名一模一样（包括空格）
        SceneManager.LoadScene("Mechanic3 Scene 2"); 
    }
}
