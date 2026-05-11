using UnityEngine;

[RequireComponent(typeof(RigSwitcher2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMoveAndFacing2D : MonoBehaviour
{
    public float speed = 3f;

    [Header("音效设置")]
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;
    public float stepInterval = 0.5f;

    private RigSwitcher2D switcher;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float stepTimer;

    void Awake()
    {
        switcher = GetComponent<RigSwitcher2D>();
        rb = GetComponent<Rigidbody2D>();
        ConfigureRigidbody();
    }

    void Update()
    {
        moveInput = ReadMoveInput();
        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        switcher.SetMoving(isMoving);
        if (isMoving)
        {
            UpdateFacing(moveInput);
            moveInput = moveInput.normalized;
        }

        HandleFootsteps(isMoving);
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput * speed;
    }

    public void StopMovement()
    {
        moveInput = Vector2.zero;
        stepTimer = 0f;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (footstepSource != null)
        {
            footstepSource.Stop();
        }

        if (switcher != null)
        {
            switcher.SetMoving(false);
        }
    }

    private void ConfigureRigidbody()
    {
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private Vector2 ReadMoveInput()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        else if (Input.GetKey(KeyCode.S)) moveY = -1f;

        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        else if (Input.GetKey(KeyCode.D)) moveX = 1f;

        return new Vector2(moveX, moveY);
    }

    private void UpdateFacing(Vector2 input)
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            switcher.SetFacing(input.x > 0 ? RigSwitcher2D.Facing.Right : RigSwitcher2D.Facing.Left);
        }
        else
        {
            switcher.SetFacing(input.y > 0 ? RigSwitcher2D.Facing.Up : RigSwitcher2D.Facing.Down);
        }
    }

    private void HandleFootsteps(bool isMoving)
    {
        if (footstepSource == null || footstepClips == null || footstepClips.Length == 0)
        {
            return;
        }

        if (isMoving && rb.velocity.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                int index = Random.Range(0, footstepClips.Length);
                footstepSource.PlayOneShot(footstepClips[index]);
                stepTimer = stepInterval;
            }

            return;
        }

        stepTimer = 0f;
    }
}
