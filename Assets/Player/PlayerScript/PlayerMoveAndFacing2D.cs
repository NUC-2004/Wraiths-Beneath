using UnityEngine;

[RequireComponent(typeof(RigSwitcher2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMoveAndFacing2D : MonoBehaviour
{
    public float speed = 3f;

    private RigSwitcher2D switcher;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Awake()
    {
        switcher = GetComponent<RigSwitcher2D>();
        rb = GetComponent<Rigidbody2D>();

        // 基础物理设置
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // 如果你的墙体很薄，建议开启连续检测防止高速穿墙
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; 
    }

    void Update()
    {
        // 1. 只响应 WASD 控制 (手动检测按键，排除掉方向键)
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        else if (Input.GetKey(KeyCode.S)) moveY = -1f; // 使用 else if 防止同时按下 WS 抵消

        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        else if (Input.GetKey(KeyCode.D)) moveX = 1f;

        moveInput = new Vector2(moveX, moveY);

        // 2. 更新动画状态
        bool moving = moveInput.sqrMagnitude > 0.01f;
        switcher.SetMoving(moving);

        if (moving)
        {
            moveInput = moveInput.normalized; // 防止斜向移动加速

            // 设置朝向
            if (Mathf.Abs(moveInput.x) >= Mathf.Abs(moveInput.y))
            {
                switcher.SetFacing(moveInput.x > 0 ? RigSwitcher2D.Facing.Right : RigSwitcher2D.Facing.Left);
            }
            else
            {
                switcher.SetFacing(moveInput.y > 0 ? RigSwitcher2D.Facing.Up : RigSwitcher2D.Facing.Down);
            }
        }
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput * speed; 
    }
}