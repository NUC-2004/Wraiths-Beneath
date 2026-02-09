using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public GameObject gameWinUI;
    public GameObject Image; // 你的死亡插图
    [Header("音效设置")]
    public AudioSource audioSource;
    public AudioClip deathSound;
    public AudioClip winSound;
    
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

        if (player != null)
        {
            var pc = player.GetComponent<PlayerMoveAndFacing2D>();
            if(pc != null) pc.enabled = false;
            var switcher = player.GetComponent<RigSwitcher2D>();
            if(switcher != null) switcher.SetDead();
        }

        if (maskEffect != null && player != null)
        {
            maskEffect.StartGameOverSequence(player.transform.position);
        }
    
        StartCoroutine(ShowUIDelayed(1f));
    }

    public void PlayerWin()
    {
        if (isGameOver) return;
        isGameOver = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        // 💡 只需要这一行：找到你的终点（记得给终点物体设置 Tag 为 Destination）
        GameObject dest = GameObject.FindGameObjectWithTag("Destination");
        RunawayMinecartEffect maskEffect = FindObjectOfType<RunawayMinecartEffect>();

        if (player != null && maskEffect != null && dest != null)
        {
            // 1. 禁用移动
            var pc = player.GetComponent<PlayerMoveAndFacing2D>();
            if (pc != null) pc.enabled = false;

            // 2. 重点：像 Lost 一样触发定位，但目标是终点 dest
            maskEffect.StartGameOverSequence(dest.transform.position);

            // 3. 播放胜利音效
            if (audioSource != null && winSound != null)
                audioSource.PlayOneShot(winSound);

            // 4. 开启缩小协程
            StartCoroutine(WinSequence(player));
        }
    }

    IEnumerator WinSequence(GameObject player)
    {
        // 玩家缩小
        float duration = 1.0f;
        float elapsed = 0f;
        Vector3 initialScale = player.transform.localScale;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (player != null)
                player.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, elapsed / duration);
            yield return null;
        }

        // 弹出 UI
        if (gameWinUI != null) gameWinUI.SetActive(true);
    }

   

    // 这个是你原来的函数，给 PlayerLost 专用，不要改动它
    IEnumerator ShowUIDelayed(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        yield return new WaitForSeconds(delayTime);

        if (Image != null)
        {
            Image.SetActive(true);
            if (audioSource != null && deathSound != null)
                audioSource.PlayOneShot(deathSound); 
            
            yield return new WaitForSeconds(1f);
            Image.SetActive(false); 
        }

        if (gameOverUI != null) gameOverUI.SetActive(true);
    }
}