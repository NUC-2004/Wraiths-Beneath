using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI 元素")]
    public GameObject gameOverUI;
    public GameObject gameWinUI;
    public GameObject Image;

    [Header("音效设置")]
    public AudioSource audioSource;
    public AudioClip deathSound;
    public AudioClip winSound;
    public AudioClip gameOverMusic;

    private bool isGameOver;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        Destroy(gameObject);
    }

    public void PlayerLost()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        RunawayMinecartEffect maskEffect = FindObjectOfType<RunawayMinecartEffect>();

        DisablePlayer(player, true);

        if (maskEffect != null && player != null)
        {
            maskEffect.StartGameOverSequence(player.transform.position);
        }

        StartCoroutine(ShowGameOverSequence(1f));
    }

    public void PlayerWin()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;
        PlayWinSound();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject dest = GameObject.FindGameObjectWithTag("Destination");
        RunawayMinecartEffect maskEffect = FindObjectOfType<RunawayMinecartEffect>();

        if (player == null || dest == null || maskEffect == null)
        {
            return;
        }

        DisablePlayer(player, false);
        maskEffect.StartGameOverSequence(dest.transform.position);
        StartCoroutine(WinSequence(player));
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator ShowGameOverSequence(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        if (Image != null)
        {
            Image.SetActive(true);
            PlayDeathSound();
        }

        StopGameplayAudio();

        yield return new WaitForSeconds(1f);

        if (Image != null)
        {
            Image.SetActive(false);
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        PlayGameOverMusic();
    }

    private IEnumerator WinSequence(GameObject player)
    {
        float duration = 1.0f;
        float elapsed = 0f;
        Vector3 initialScale = player.transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (player != null)
            {
                player.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, elapsed / duration);
            }

            yield return null;
        }

        if (gameWinUI != null)
        {
            gameWinUI.SetActive(true);
        }
    }

    private void DisablePlayer(GameObject player, bool showDeadRig)
    {
        if (player == null)
        {
            return;
        }

        PlayerMoveAndFacing2D movement = player.GetComponent<PlayerMoveAndFacing2D>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        if (!showDeadRig)
        {
            return;
        }

        RigSwitcher2D switcher = player.GetComponent<RigSwitcher2D>();
        if (switcher != null)
        {
            switcher.SetDead();
        }
    }

    private void PlayDeathSound()
    {
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
    }

    private void PlayWinSound()
    {
        if (audioSource != null && winSound != null)
        {
            audioSource.PlayOneShot(winSound, 0.2f);
        }
    }

    private void PlayGameOverMusic()
    {
        if (gameOverMusic == null || audioSource == null)
        {
            return;
        }

        audioSource.clip = gameOverMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    private void StopGameplayAudio()
    {
        if (MusicManager.Instance != null && MusicManager.Instance.musicSource != null)
        {
            MusicManager.Instance.musicSource.Stop();
        }

        MonsterProximity[] allGhosts = FindObjectsOfType<MonsterProximity>();
        foreach (MonsterProximity ghost in allGhosts)
        {
            ghost.StopProximitySound();
        }

        AudioSource[] allSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allSources)
        {
            if (source != audioSource)
            {
                source.Stop();
            }
        }
    }
}
