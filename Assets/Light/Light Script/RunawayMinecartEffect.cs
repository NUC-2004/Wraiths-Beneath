using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RunawayMinecartEffect : MonoBehaviour
{
    [Header("动量系统")]
    public float momentumGain = 0.1f;
    public float momentumDecay = 0.002f;
    public float maxMomentum = 2.5f;

    [Header("失控加速")]
    public float runawaySpeedThreshold = 1.2f;
    public float runawayAcceleration = 0.15f;
    public float runawayTurnResistance = 0.95f;

    [Header("物理效果")]
    public float wallBounce = 0.85f;
    public float speedDamping = 0.998f;

    [Header("遮罩参数")]
    public Material maskMaterial;
    public float revealRadius = 0.18f;
    public float edgeSoftness = 0.05f;

    private readonly Vector2 startPosition = new Vector2(0.5f, 0.7f);
    private readonly float normalTurnResistance = 0.85f;

    private Vector2 position = new Vector2(0.5f, 0.5f);
    private Vector2 velocity;
    private Vector2 currentDirection;
    private float currentMomentum = 1.0f;
    private bool isRunaway;
    private bool isResetting;

    void Start()
    {
        ResolveMaskMaterial();
        position = startPosition;
        UpdateMask();
        StartCoroutine(CheckRunawayState());
    }

    void Update()
    {
        if (isResetting)
        {
            return;
        }

        HandleMinecartControls();
        ApplyRunawayPhysics();
        UpdatePosition();
        UpdateMask();
    }

    public void ApplyExternalForce(Vector2 force)
    {
        velocity += force;
        currentMomentum = velocity.magnitude;

        if (velocity.magnitude > runawaySpeedThreshold)
        {
            isRunaway = true;
        }
    }

    public void ResetToPlayer(Vector3 playerWorldPos)
    {
        if (isResetting)
        {
            return;
        }

        isResetting = true;
        Vector2 targetPos = Camera.main.WorldToViewportPoint(playerWorldPos);
        StartCoroutine(MoveToPlayerRoutine(targetPos));
    }

    public void StartGameOverSequence(Vector3 playerWorldPos)
    {
        if (isResetting)
        {
            return;
        }

        isResetting = true;
        Vector2 targetPos = Camera.main.WorldToViewportPoint(playerWorldPos);
        StartCoroutine(GameOverRoutine(targetPos));
    }

    private void ResolveMaskMaterial()
    {
        if (maskMaterial != null)
        {
            return;
        }

        Image image = GetComponent<Image>();
        if (image != null && image.material != null)
        {
            maskMaterial = image.material;
        }
    }

    private void HandleMinecartControls()
    {
        Vector2 input = ReadArrowInput();
        if (input.magnitude > 0.1f)
        {
            UpdateDirectionFromInput(input.normalized);
            currentMomentum = Mathf.Min(currentMomentum + momentumGain * Time.deltaTime, maxMomentum);
        }
        else
        {
            currentMomentum = Mathf.Max(currentMomentum - momentumDecay * Time.deltaTime, 0.1f);
        }

        velocity = currentDirection * currentMomentum;
    }

    private Vector2 ReadArrowInput()
    {
        float inputX = 0f;
        float inputY = 0f;

        if (Input.GetKey(KeyCode.RightArrow)) inputX += 1f;
        if (Input.GetKey(KeyCode.LeftArrow)) inputX -= 1f;
        if (Input.GetKey(KeyCode.UpArrow)) inputY += 1f;
        if (Input.GetKey(KeyCode.DownArrow)) inputY -= 1f;

        return new Vector2(inputX, inputY);
    }

    private void UpdateDirectionFromInput(Vector2 targetDirection)
    {
        float turnResistance = isRunaway ? runawayTurnResistance : normalTurnResistance;
        float speedFactor = Mathf.Clamp01(velocity.magnitude / runawaySpeedThreshold);
        float effectiveTurnResistance = Mathf.Lerp(0.7f, turnResistance, speedFactor);
        float turnAmount = (1f - effectiveTurnResistance) * Time.deltaTime * 3f;

        currentDirection = Vector2.Lerp(currentDirection, targetDirection, turnAmount);
    }

    private void ApplyRunawayPhysics()
    {
        if (!isRunaway && velocity.magnitude > runawaySpeedThreshold)
        {
            isRunaway = true;
        }

        if (isRunaway)
        {
            velocity += velocity.normalized * runawayAcceleration * Time.deltaTime;
            velocity *= 0.999f;
        }
        else
        {
            velocity *= speedDamping;
        }
    }

    private void UpdatePosition()
    {
        position += velocity * Time.deltaTime;

        bool hitWall = false;
        hitWall |= ClampAxis(ref position.x, ref velocity.x, 0.02f, 0.98f);
        hitWall |= ClampAxis(ref position.y, ref velocity.y, 0.02f, 0.98f);

        if (hitWall && isRunaway && velocity.magnitude < runawaySpeedThreshold * 0.7f)
        {
            isRunaway = false;
        }
    }

    private bool ClampAxis(ref float axisPosition, ref float axisVelocity, float min, float max)
    {
        if (axisPosition < min)
        {
            axisPosition = min;
            axisVelocity = Mathf.Abs(axisVelocity) * wallBounce;
            return true;
        }

        if (axisPosition > max)
        {
            axisPosition = max;
            axisVelocity = -Mathf.Abs(axisVelocity) * wallBounce;
            return true;
        }

        return false;
    }

    private IEnumerator CheckRunawayState()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);

            if (isRunaway && velocity.magnitude < runawaySpeedThreshold * 0.6f)
            {
                isRunaway = false;
            }
        }
    }

    private void UpdateMask()
    {
        if (maskMaterial == null)
        {
            return;
        }

        maskMaterial.SetVector("_Center", new Vector4(position.x, position.y, 0f, 0f));
        maskMaterial.SetFloat("_Radius", revealRadius);
        maskMaterial.SetFloat("_Softness", edgeSoftness);
    }

    private IEnumerator MoveToPlayerRoutine(Vector2 targetPos)
    {
        float elapsed = 0f;
        float duration = 0.1f;
        Vector2 startPos = position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            position = Vector2.Lerp(startPos, targetPos, elapsed / duration);
            UpdateMask();
            yield return null;
        }

        position = targetPos;
        UpdateMask();
    }

    private IEnumerator GameOverRoutine(Vector2 targetPos)
    {
        float moveElapsed = 0f;
        float moveDuration = 0.5f;
        Vector2 startPos = position;

        while (moveElapsed < moveDuration)
        {
            moveElapsed += Time.deltaTime;
            position = Vector2.Lerp(startPos, targetPos, moveElapsed / moveDuration);
            UpdateMask();
            yield return null;
        }

        position = targetPos;

        float expandElapsed = 0f;
        float expandDuration = 1.0f;
        float startRadius = revealRadius;
        float targetRadius = 2.0f;

        while (expandElapsed < expandDuration)
        {
            expandElapsed += Time.deltaTime;
            revealRadius = Mathf.Lerp(startRadius, targetRadius, expandElapsed / expandDuration);
            UpdateMask();
            yield return null;
        }
    }

    private void ResetToCenter()
    {
        position = new Vector2(0.5f, 0.5f);
        isRunaway = false;
    }
}
