using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    [Header("Door Unlock")]
    public float unlockDuration = 3f;
    public bool resetProgressWhenNotLooking = true;

    private static GameObject progressRoot;
    private static Image progressFill;

    private RunawayMinecartEffect viewController;
    private float unlockProgress;
    private bool isUnlocked;
    private bool playerInside;
    private bool hasWon;

    private void Start()
    {
        viewController = FindObjectOfType<RunawayMinecartEffect>();
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
        else if (resetProgressWhenNotLooking)
        {
            unlockProgress = 0f;
        }

        UpdateProgressUi(isBeingWatched || unlockProgress > 0f);

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
        UpdateProgressUi(false);
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
        if (progressRoot != null && progressFill != null)
        {
            return;
        }

        progressRoot = new GameObject("DoorUnlockProgressUI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = progressRoot.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = progressRoot.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject background = new GameObject("Background", typeof(RectTransform), typeof(Image));
        background.transform.SetParent(progressRoot.transform, false);

        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = new Vector2(0.5f, 0f);
        backgroundRect.anchorMax = new Vector2(0.5f, 0f);
        backgroundRect.pivot = new Vector2(0.5f, 0.5f);
        backgroundRect.anchoredPosition = new Vector2(0f, 96f);
        backgroundRect.sizeDelta = new Vector2(340f, 18f);

        Image backgroundImage = background.GetComponent<Image>();
        backgroundImage.color = new Color(0f, 0f, 0f, 0.65f);
        backgroundImage.raycastTarget = false;

        GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fill.transform.SetParent(background.transform, false);

        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        progressFill = fill.GetComponent<Image>();
        progressFill.color = new Color(0.22f, 0.95f, 0.78f, 0.95f);
        progressFill.type = Image.Type.Filled;
        progressFill.fillMethod = Image.FillMethod.Horizontal;
        progressFill.fillOrigin = 0;
        progressFill.fillAmount = 0f;
        progressFill.raycastTarget = false;
    }

    private void UpdateProgressUi(bool visible)
    {
        EnsureProgressUi();
        progressFill.fillAmount = unlockDuration > 0f ? Mathf.Clamp01(unlockProgress / unlockDuration) : 1f;
        SetProgressVisible(visible && !isUnlocked);
    }

    private void SetProgressVisible(bool visible)
    {
        if (progressRoot != null && progressRoot.activeSelf != visible)
        {
            progressRoot.SetActive(visible);
        }
    }
}
