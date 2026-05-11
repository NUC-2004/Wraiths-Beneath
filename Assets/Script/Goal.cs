using UnityEngine;

public class Goal : MonoBehaviour
{
    [Header("Door Unlock")]
    public float unlockDuration = 3f;

    [Header("Progress Display")]
    public Vector2 progressOffset = new Vector2(0f, 0.45f);
    public Vector2 progressBarSize = new Vector2(1.2f, 0.08f);
    public Color progressFillColor = new Color(0.95f, 0.12f, 0.08f, 0.95f);
    public int progressSortingOrder = 1000;

    [Header("Door Visual")]
    public SpriteRenderer doorRenderer;
    public Color unlockedDoorTint = new Color(0.65f, 1f, 0.65f, 1f);
    [Range(0f, 1f)] public float unlockedTintStrength = 0.35f;

    private static Sprite progressSprite;

    private RunawayMinecartEffect viewController;
    private GameObject progressRoot;
    private Transform progressFillTransform;
    private Color originalDoorColor = Color.white;
    private float unlockProgress;
    private bool isUnlocked;
    private bool playerInside;
    private bool hasWon;

    private void Start()
    {
        viewController = FindObjectOfType<RunawayMinecartEffect>();
        ResolveDoorRenderer();
        EnsureProgressUi();
        SetProgressVisible(false);
    }

    private void Update()
    {
        if (isUnlocked || (GameManager.Instance != null && GameManager.Instance.IsGameOver))
        {
            SetProgressVisible(false);
            return;
        }

        bool isBeingWatched = IsBeingWatched();
        if (isBeingWatched)
        {
            unlockProgress = Mathf.Min(unlockProgress + Time.deltaTime, unlockDuration);
        }
        else
        {
            unlockProgress = 0f;
        }

        UpdateProgressDisplay(isBeingWatched);

        if (unlockProgress >= unlockDuration)
        {
            UnlockDoor();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInside = true;
        TryWin();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    private bool IsBeingWatched()
    {
        if (viewController == null)
        {
            viewController = FindObjectOfType<RunawayMinecartEffect>();
        }

        return viewController != null && viewController.IsWorldPositionInsideView(transform.position);
    }

    private void UnlockDoor()
    {
        isUnlocked = true;
        unlockProgress = unlockDuration;
        ApplyUnlockedDoorTint();
        UpdateProgressDisplay(false);
        TryWin();
    }

    private void TryWin()
    {
        if (!isUnlocked || !playerInside || hasWon || GameManager.Instance == null)
        {
            return;
        }

        hasWon = true;
        GameManager.Instance.PlayerWin();
    }

    private void EnsureProgressUi()
    {
        if (progressRoot != null && progressFillTransform != null)
        {
            return;
        }

        progressRoot = new GameObject($"{name}_DoorUnlockProgress");
        progressRoot.transform.position = GetProgressWorldPosition();

        GameObject background = new GameObject("Background");
        background.transform.SetParent(progressRoot.transform, false);
        background.transform.localScale = new Vector3(progressBarSize.x, progressBarSize.y, 1f);

        SpriteRenderer backgroundRenderer = background.AddComponent<SpriteRenderer>();
        backgroundRenderer.sprite = GetProgressSprite();
        backgroundRenderer.color = new Color(0f, 0f, 0f, 0.65f);
        backgroundRenderer.sortingOrder = progressSortingOrder;

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(background.transform, false);
        progressFillTransform = fill.transform;

        SpriteRenderer fillRenderer = fill.AddComponent<SpriteRenderer>();
        fillRenderer.sprite = GetProgressSprite();
        fillRenderer.color = progressFillColor;
        fillRenderer.sortingOrder = progressSortingOrder + 1;

        SetProgressVisible(false);
    }

    private void UpdateProgressDisplay(bool visible)
    {
        EnsureProgressUi();

        float fillAmount = unlockDuration > 0f ? Mathf.Clamp01(unlockProgress / unlockDuration) : 1f;

        progressRoot.transform.position = GetProgressWorldPosition();
        progressFillTransform.localScale = new Vector3(fillAmount, 1f, 1f);
        progressFillTransform.localPosition = new Vector3(-0.5f + fillAmount * 0.5f, 0f, 0f);

        SetProgressVisible(visible && !isUnlocked);
    }

    private Vector3 GetProgressWorldPosition()
    {
        return transform.position + new Vector3(progressOffset.x, progressOffset.y, 0f);
    }

    private void ResolveDoorRenderer()
    {
        if (doorRenderer == null)
        {
            doorRenderer = GetComponent<SpriteRenderer>();
        }

        if (doorRenderer == null)
        {
            doorRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (doorRenderer != null)
        {
            originalDoorColor = doorRenderer.color;
        }
    }

    private void ApplyUnlockedDoorTint()
    {
        if (doorRenderer == null)
        {
            ResolveDoorRenderer();
        }

        if (doorRenderer != null)
        {
            doorRenderer.color = Color.Lerp(originalDoorColor, unlockedDoorTint, unlockedTintStrength);
        }
    }

    private static Sprite GetProgressSprite()
    {
        if (progressSprite != null)
        {
            return progressSprite;
        }

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        progressSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        return progressSprite;
    }

    private void SetProgressVisible(bool visible)
    {
        if (progressRoot != null && progressRoot.activeSelf != visible)
        {
            progressRoot.SetActive(visible);
        }
    }

    private void OnDestroy()
    {
        if (progressRoot != null)
        {
            Destroy(progressRoot);
        }
    }
}
