using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI 元素")]
    public GameObject gameOverUI;
    public GameObject gameWinUI;
    public GameObject Image;
    public GameObject deathIllustration;
    public Sprite deathIllustrationSprite;

    [Header("音效设置")]
    public AudioSource audioSource;
    public AudioClip deathSound;
    public AudioClip deathSequenceSound;
    public AudioClip winSound;
    public AudioClip gameOverMusic;

    private bool isGameOver;
    private AudioClip lastDeathClip;
    public bool IsGameOver => isGameOver;

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

        EndGameplay(player, true, false);

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

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject dest = GameObject.FindGameObjectWithTag("Destination");
        RunawayMinecartEffect maskEffect = FindObjectOfType<RunawayMinecartEffect>();

        EndGameplay(player, false, true);
        PlayWinSound();

        if (player == null || dest == null || maskEffect == null)
        {
            return;
        }

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

        GameObject deathImage = GetDeathIllustration();
        if (deathImage != null)
        {
            deathImage.transform.SetAsLastSibling();
            deathImage.SetActive(true);
        }

        PlayDeathSound();
        StopGameplayAudio();

        yield return new WaitForSeconds(1f);

        if (deathImage != null)
        {
            deathImage.SetActive(false);
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
            movement.StopMovement();
            movement.enabled = false;
        }

        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.StopMovement();
            controller.enabled = false;
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        RigSwitcher2D switcher = player.GetComponent<RigSwitcher2D>();
        if (switcher != null)
        {
            switcher.SetMoving(false);
        }

        if (!showDeadRig)
        {
            return;
        }

        if (switcher != null)
        {
            switcher.SetDead();
        }
    }

    private void EndGameplay(GameObject player, bool showDeadRig, bool stopAllGameplayAudio)
    {
        DisablePlayer(player, showDeadRig);
        StopMonsters();
        StopProximityAudio();

        if (stopAllGameplayAudio)
        {
            StopGameplayAudio();
        }
    }

    private void StopMonsters()
    {
        MonsterAI[] monsters = FindObjectsOfType<MonsterAI>();
        foreach (MonsterAI monster in monsters)
        {
            try
            {
                monster.StopChasing();
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning($"Failed to stop monster '{monster.name}': {exception.Message}", monster);
            }
        }
    }

    private void PlayDeathSound()
    {
        AudioClip clip = deathSequenceSound != null ? deathSequenceSound : deathSound;
        if (clip == null)
        {
            clip = gameOverMusic;
        }

        if (audioSource != null && clip != null)
        {
            lastDeathClip = clip;
            audioSource.PlayOneShot(clip);
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

        if (lastDeathClip == gameOverMusic && audioSource.isPlaying)
        {
            return;
        }

        audioSource.clip = gameOverMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    private GameObject GetDeathIllustration()
    {
        if (Image != null)
        {
            return Image;
        }

        if (deathIllustration != null)
        {
            Image = deathIllustration;
            return Image;
        }

        Image existingImage = FindExistingDeathIllustration();
        if (existingImage != null)
        {
            Image = existingImage.gameObject;
            return Image;
        }

        return CreateDeathIllustration();
    }

    private Image FindExistingDeathIllustration()
    {
        Image[] images = Resources.FindObjectsOfTypeAll<Image>();
        foreach (Image candidate in images)
        {
            if (!candidate.gameObject.scene.IsValid() || candidate.sprite == null)
            {
                continue;
            }

            if (candidate.sprite.name.Contains("86117"))
            {
                return candidate;
            }
        }

        return null;
    }

    private GameObject CreateDeathIllustration()
    {
        if (deathIllustrationSprite == null)
        {
            return null;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            return null;
        }

        GameObject imageObject = new GameObject("DeathIllustration", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.transform.SetParent(canvas.transform, false);

        RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = imageObject.GetComponent<Image>();
        image.sprite = deathIllustrationSprite;
        image.preserveAspect = false;
        image.raycastTarget = false;

        imageObject.SetActive(false);
        Image = imageObject;
        return imageObject;
    }

    private void StopGameplayAudio()
    {
        if (MusicManager.Instance != null && MusicManager.Instance.musicSource != null)
        {
            MusicManager.Instance.musicSource.Stop();
        }

        StopProximityAudio();

        AudioSource[] allSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allSources)
        {
            if (source != audioSource)
            {
                source.Stop();
            }
        }
    }

    private void StopProximityAudio()
    {
        MonsterProximity[] allGhosts = FindObjectsOfType<MonsterProximity>();
        foreach (MonsterProximity ghost in allGhosts)
        {
            ghost.StopProximitySound();
        }
    }
}
