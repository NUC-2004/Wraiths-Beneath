using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI设置")]
    public GameObject pauseMenu; // 在Inspector面板中拖入你的暂停菜单Panel

    private bool isPaused = false;

    void Awake()
    {
        // 游戏启动时，强制确保菜单是关闭的，时间是流动的
        // 这样可以解决“必须在编辑器里勾选才能正常使用”的问题
        if (pauseMenu != null)
        {
            ResumeGame(); 
        }
    }

    void Update()
    {
        // 检测按下 Esc 键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // 1. 继续游戏
    public void ResumeGame()
    {
        pauseMenu.SetActive(false); // 隐藏菜单
        Time.timeScale = 1f;        // 恢复正常时间流速
        isPaused = false;
    }

    // 呼出暂停
    public void PauseGame()
    {
        pauseMenu.SetActive(true);  // 显示菜单
        Time.timeScale = 0f;        // 冻结游戏时间
        isPaused = true;
    }

    // 2. 返回主菜单
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // 极其重要：切换场景前必须恢复时间，否则新场景会卡死
        // 这里的名字必须和你 Build Settings 里的场景名完全一致
        SceneManager.LoadScene("MenuSecne");
    }

    // 3. 重新开始
    public void RestartGame()
    {
        Time.timeScale = 1f; // 恢复时间
        // 重新加载当前正在运行的场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
}






