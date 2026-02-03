using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI; 
    public float scaleSpeed = 2f;
    private bool isGameOver = false;

    public static GameManager Instance;
    void Awake() { Instance = this; }

    public void PlayerLost()
    {
        if (isGameOver) return;
        isGameOver = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        RunawayMinecartEffect maskEffect = FindObjectOfType<RunawayMinecartEffect>();

        // 1. 先让玩家动不了
        if (player != null)
        {
            var pc = player.GetComponent<PlayerController>();
            if(pc != null) pc.enabled = false;
        }

        // 2. 触发光圈吸附（光圈会在吸附完后自动 SetActive(false)）
        if (maskEffect != null && player != null)
        {
            maskEffect.StartGameOverSequence(player.transform.position);
        }

        // 3. 弹出 UI
        StartCoroutine(ShowGameOverAnimation());
    }

    public void PlayerWin()
    {
        if (isGameOver) return;
        isGameOver = true;
        Invoke("RestartGame", 0.1f); 
    }

    IEnumerator ShowGameOverAnimation()
    {
        // 稍微等一下光圈吸附的动作，再弹出 UI
        yield return new WaitForSeconds(0.5f);
        
        gameOverUI.SetActive(true);
        Vector3 targetScale = new Vector3(10, 10, 1); 
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * scaleSpeed;
            gameOverUI.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }
        yield return new WaitForSeconds(1.0f);
        RestartGame();
    }

    public void RestartGame() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
}