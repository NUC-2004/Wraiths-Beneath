using UnityEngine;

[RequireComponent(typeof(RigSwitcher2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMoveAndFacing2D : MonoBehaviour
{
    public float speed = 3f;

    [Header("音效设置")]
    public AudioSource footstepSource; 
    public AudioClip[] footstepClips; // 💡 Public 数组，在面板里填 3，把三个音效拖进去
    public float stepInterval = 0.5f; // 💡 Public 间隔时间，默认 0.5s

    private float stepTimer;
    private RigSwitcher2D switcher;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Awake()
    {
        switcher = GetComponent<RigSwitcher2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; 
    }

    void Update()
    {
        // --- 原有移动逻辑 ---
        float moveX = 0f;
        float moveY = 0f;
        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        else if (Input.GetKey(KeyCode.S)) moveY = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        else if (Input.GetKey(KeyCode.D)) moveX = 1f;

        moveInput = new Vector2(moveX, moveY);
        bool isPressingKeys = moveInput.sqrMagnitude > 0.01f;
        switcher.SetMoving(isPressingKeys);

        if (isPressingKeys)
        {
            moveInput = moveInput.normalized;
            if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
                switcher.SetFacing(moveX > 0 ? RigSwitcher2D.Facing.Right : RigSwitcher2D.Facing.Left);
            else
                switcher.SetFacing(moveY > 0 ? RigSwitcher2D.Facing.Up : RigSwitcher2D.Facing.Down);
        }

        // --- 💡 新增：随机音效逻辑 ---
        HandleFootsteps(isPressingKeys);
    }

    void HandleFootsteps(bool isPressingKeys)
    {
        if (footstepSource == null || footstepClips.Length == 0) return;

        // 如果正在按键 且 没撞墙 (有物理速度)
        if (isPressingKeys && rb.velocity.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime; // 计时器倒计时

            if (stepTimer <= 0)
            {
                // 随机选一个音效
                int index = Random.Range(0, footstepClips.Length);
                footstepSource.PlayOneShot(footstepClips[index]);

                // 重置计时器
                stepTimer = stepInterval;
            }
        }
        else
        {
            // 停止移动时，计时器清零，保证下次走动立刻响第一声
            stepTimer = 0;
        }
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput * speed;
    }
}