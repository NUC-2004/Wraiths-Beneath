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

            // --- 原本在这里播放的 deathSound 移走了，为了和图片同步 ---
        }

        if (maskEffect != null && player != null)
        {
            maskEffect.StartGameOverSequence(player.transform.position);
        }
    
        // 延迟显示图片和播放音效
        StartCoroutine(ShowUIDelayed(1f)); 
    }

    IEnumerator ShowUIDelayed(float delayTime)
    {
        // 1. 等待第一段延迟
        yield return new WaitForSeconds(delayTime);

        // --- 此时：图片展示，声音同步响起 ---
        if (Image != null) 
        {
            Image.SetActive(true);
            
            // 在图片显示的瞬间，播放死亡音效
            if (audioSource != null && deathSound != null)
            {
                audioSource.PlayOneShot(deathSound);
            }
        }

        // 清理现场其他背景音乐
        if (MusicManager.Instance != null && MusicManager.Instance.musicSource != null)
            MusicManager.Instance.musicSource.Stop();

        MonsterProximity[] allGhosts = FindObjectsOfType<MonsterProximity>();
        foreach (MonsterProximity ghost in allGhosts) ghost.StopProximitySound();

        AudioSource[] allSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allSources)
            if (source != audioSource) source.Stop();

        // 2. 图片显示 1 秒
        yield return new WaitForSeconds(1f);

        // 3. --- 此时：图片消失，弹出正式 UI ---
        if (Image != null) Image.SetActive(false); 

        if (gameOverUI != null) gameOverUI.SetActive(true);

        // 播放失败后的背景循环音乐
        if (gameOverMusic != null && audioSource != null)
        {
            audioSource.clip = gameOverMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void PlayerWin()
    {
        if (isGameOver) return;
        isGameOver = true;

        // 胜利音量调小到 0.2
        if (audioSource != null && winSound != null) 
        {
            audioSource.PlayOneShot(winSound, 0.2f); 
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject dest = GameObject.FindGameObjectWithTag("Destination");
        RunawayMinecartEffect maskEffect = FindObjectOfType<RunawayMinecartEffect>();

        if (player != null && dest != null && maskEffect != null)
        {
            var pc = player.GetComponent<PlayerMoveAndFacing2D>();
            if (pc != null) pc.enabled = false;
            maskEffect.StartGameOverSequence(dest.transform.position);
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