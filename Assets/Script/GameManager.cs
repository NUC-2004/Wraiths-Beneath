using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI 元素")]
    public GameObject gameOverUI;
    public GameObject gameWinUI;
    public GameObject Image; // 死亡大图

    [Header("音效设置")]
    public AudioSource audioSource; 
    public AudioClip deathSound;    
    public AudioClip winSound;      
    public AudioClip gameOverMusic; 

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayerLost()
    {
        if (isGameOver) return;
        isGameOver = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        RunawayMinecartEffect maskEffect = FindObjectOfType<RunawayMinecartEffect>();

        if (player != null)
        {
            var pc = player.GetComponent<PlayerMoveAndFacing2D>();
            if (pc != null) pc.enabled = false;

            var switcher = player.GetComponent<RigSwitcher2D>();
            if (switcher != null) switcher.SetDead();

            if (audioSource != null && deathSound != null)
            {
                audioSource.PlayOneShot(deathSound);
            }
        }

        if (maskEffect != null && player != null)
        {
            maskEffect.StartGameOverSequence(player.transform.position);
        }
    
        // 步骤 A：先等待 1.5 秒
        StartCoroutine(ShowUIDelayed(1.5f)); 
    }

    IEnumerator ShowUIDelayed(float delayTime)
    {
        // 1. 等待第一段延迟 (1.5s)
        yield return new WaitForSeconds(delayTime);

        // --- 此时：清理现场声音并显示图片 ---
        if (MusicManager.Instance != null && MusicManager.Instance.musicSource != null)
            MusicManager.Instance.musicSource.Stop();

        MonsterProximity[] allGhosts = FindObjectsOfType<MonsterProximity>();
        foreach (MonsterProximity ghost in allGhosts) ghost.StopProximitySound();

        AudioSource[] allSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allSources)
            if (source != audioSource) source.Stop();

        if (Image != null) Image.SetActive(true);

        // 2. 步骤 B：图片显示 1 秒
        yield return new WaitForSeconds(1f);

        // 3. --- 此时：图片消失，弹出正式 UI ---
        if (Image != null) Image.SetActive(false); // 图片消失

        if (gameOverUI != null) gameOverUI.SetActive(true);

        // 播放失败背景音乐
        if (gameOverMusic != null && audioSource != null)
        {
            audioSource.clip = gameOverMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    // --- 胜利逻辑 (保持原样) ---
    public void PlayerWin()
    {
        if (isGameOver) return;
        isGameOver = true;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject dest = GameObject.FindGameObjectWithTag("Destination");
        RunawayMinecartEffect maskEffect = FindObjectOfType<RunawayMinecartEffect>();
        if (player != null && dest != null && maskEffect != null)
        {
            var pc = player.GetComponent<PlayerMoveAndFacing2D>();
            if (pc != null) pc.enabled = false;
            maskEffect.StartGameOverSequence(dest.transform.position);
            if (audioSource != null && winSound != null) audioSource.PlayOneShot(winSound);
            StartCoroutine(WinSequence(player));
        }
    }

    IEnumerator WinSequence(GameObject player)
    {
        float duration = 1.0f;
        float elapsed = 0f;
        Vector3 initialScale = player.transform.localScale;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (player != null) player.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, elapsed / duration);
            yield return null;
        }
        if (gameWinUI != null) gameWinUI.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}