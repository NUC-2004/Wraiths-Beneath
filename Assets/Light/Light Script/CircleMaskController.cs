using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RunawayMinecartEffect : MonoBehaviour
{
    [Header("矿车失控设置")]
    public float initialSpeed = 0.8f;       // 巨大的初速度
    public Vector2 initialDirection = new Vector2(0.7f, 0.7f); // 初始方向
    
    [Header("动量系统")]
    public float momentumGain = 0.1f;       // 按键时动量增加
    public float momentumDecay = 0.002f;    // 动量衰减（非常慢）
    public float maxMomentum = 2.5f;        // 最大动量
    
    [Header("失控加速")]
    public float runawaySpeedThreshold = 1.2f; // 失控速度阈值
    public float runawayAcceleration = 0.15f;  // 失控时自动加速
    public float runawayTurnResistance = 0.95f; // 失控时转向阻力
    
    [Header("物理效果")]
    public float wallBounce = 0.85f;        // 墙壁反弹系数
    public float speedDamping = 0.998f;     // 速度衰减（接近1=几乎不减速）
    
    [Header("遮罩参数")]
    public Material maskMaterial;
    public float revealRadius = 0.18f;
    public float edgeSoftness = 0.05f;
    
    private Vector2 position = new Vector2(0.5f, 0.5f);
    private Vector2 velocity;
    private Vector2 currentDirection;
    private float currentMomentum = 1.0f;
    private bool isRunaway = false;
    private float normalTurnResistance = 0.85f;
    
    void Start()
    {
        // 初始化材质
        if (maskMaterial == null)
        {
            Image image = GetComponent<Image>();
            if (image != null && image.material != null)
            {
                maskMaterial = image.material;
            }
        }
        
        // 设置巨大初速度
        currentDirection = initialDirection.normalized;
        velocity = currentDirection * initialSpeed;
        currentMomentum = initialSpeed;
        
        // 初始化位置为随机点
        position = new Vector2(0.3f, 0.3f);
        
        UpdateMask();
        
        // 启动失控监测
        StartCoroutine(CheckRunawayState());
    }
    
    void Update()
    {
        HandleMinecartControls();
        ApplyRunawayPhysics();
        UpdatePosition();
        UpdateMask();
    }
    
    void HandleMinecartControls()
    {
        // 获取输入方向
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
        
        if (input.magnitude > 0.1f)
        {
            // 矿车控制：非常难改变方向
            Vector2 targetDirection = input.normalized;
            
            // 根据当前速度计算转向难度
            float currentTurnResistance = isRunaway ? runawayTurnResistance : normalTurnResistance;
            float speedFactor = Mathf.Clamp01(velocity.magnitude / runawaySpeedThreshold);
            float effectiveTurnResistance = Mathf.Lerp(0.7f, currentTurnResistance, speedFactor);
            
            // 缓慢改变方向
            currentDirection = Vector2.Lerp(currentDirection, targetDirection, 
                (1f - effectiveTurnResistance) * Time.deltaTime * 3f);
            
            // 积累动量（按键越久跑越快）
            currentMomentum = Mathf.Min(currentMomentum + momentumGain * Time.deltaTime, maxMomentum);
        }
        else
        {
            // 松开按键：动量衰减极慢，保持滑行
            currentMomentum = Mathf.Max(currentMomentum - momentumDecay * Time.deltaTime, 0.1f);
        }
        
        // 更新速度（方向 × 动量）
        velocity = currentDirection * currentMomentum;
    }
    
    void ApplyRunawayPhysics()
    {
        // 检测是否进入失控状态
        if (!isRunaway && velocity.magnitude > runawaySpeedThreshold)
        {
            isRunaway = true;
        }
        
        // 失控状态：自动加速，更难控制
        if (isRunaway)
        {
            // 持续加速
            velocity += velocity.normalized * runawayAcceleration * Time.deltaTime;
            
            // 速度衰减更慢
            velocity *= 0.999f;
        }
        else
        {
            // 正常状态的速度衰减
            velocity *= speedDamping;
        }
        
        
    }
    
    void UpdatePosition()
    {
        // 应用速度
        position.x += velocity.x * Time.deltaTime;
        position.y += velocity.y * Time.deltaTime;
        
        // 边界碰撞检测
        bool hitWall = false;
        
        // 左右边界
        if (position.x < 0.02f)
        {
            position.x = 0.02f;
            velocity.x = Mathf.Abs(velocity.x) * wallBounce;
            hitWall = true;
        }
        else if (position.x > 0.98f)
        {
            position.x = 0.98f;
            velocity.x = -Mathf.Abs(velocity.x) * wallBounce;
            hitWall = true;
        }
        
        // 上下边界
        if (position.y < 0.02f)
        {
            position.y = 0.02f;
            velocity.y = Mathf.Abs(velocity.y) * wallBounce;
            hitWall = true;
        }
        else if (position.y > 0.98f)
        {
            position.y = 0.98f;
            velocity.y = -Mathf.Abs(velocity.y) * wallBounce;
            hitWall = true;
        }
        
        // 撞墙后可能退出失控状态
        if (hitWall && isRunaway && velocity.magnitude < runawaySpeedThreshold * 0.7f)
        {
            isRunaway = false;
        }
    }
    
    IEnumerator CheckRunawayState()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            
            // 如果速度降到阈值以下，退出失控状态
            if (isRunaway && velocity.magnitude < runawaySpeedThreshold * 0.6f)
            {
                isRunaway = false;
            }
        }
    }
    
    void UpdateMask()
    {
        if (maskMaterial != null)
        {
            maskMaterial.SetVector("_Center", new Vector4(position.x, position.y, 0, 0));
            maskMaterial.SetFloat("_Radius", revealRadius);
            maskMaterial.SetFloat("_Softness", edgeSoftness);
        }
    }
    
    // 外力干扰（可用于游戏事件）
    public void ApplyExternalForce(Vector2 force)
    {
        velocity += force;
        currentMomentum = velocity.magnitude;
        
        // 外力可能引发失控
        if (velocity.magnitude > runawaySpeedThreshold)
        {
            isRunaway = true;
        }
    }
    
    
    
    void ResetToCenter()
    {
        position = new Vector2(0.5f, 0.5f);
        velocity = initialDirection.normalized * initialSpeed;
        currentMomentum = initialSpeed;
        currentDirection = initialDirection.normalized;
        isRunaway = false;
    }
}