using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))] // 或者 CircleCollider2D
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 2.5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // --- 关键设置：防止玩家受重力下坠 ---
        rb.gravityScale = 0f; 
        
        // --- 关键设置：防止玩家撞墙后旋转 ---
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; 
        
        // --- 关键设置：确保碰撞检测是连续的（防止高速穿墙） ---
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Update()
    {
        // 1. 处理输入 (只响应 WASD)
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        if (Input.GetKey(KeyCode.S)) moveY = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;

        // 归一化向量，防止斜向移动速度变快
        movement = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        rb.velocity = movement * moveSpeed;
    }
}