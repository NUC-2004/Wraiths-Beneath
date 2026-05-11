using UnityEngine;

public class GameFlowController : PauseManager
{
    [Header("UI面板配置")]
    public GameObject pauseMenuPanel;

    protected override void Awake()
    {
        if (pauseMenu == null)
        {
            pauseMenu = pauseMenuPanel;
        }

        base.Awake();
    }

    public void RestartLevel()
    {
        RestartGame();
    }

    public void BackToMainMenu()
    {
        GoToMainMenu();
    }

    public void JumpToMechanicScene()
    {
        LoadScene("3");
    }

    public void JumpToMechanic3Scene2()
    {
        LoadScene("2");
    }
}
