using UnityEngine;
using UnityEngine.UI;

public class SlipperyLightControl : MonoBehaviour
{
    [Header("基础设置")]
    public Material maskMaterial;
    public float baseSpeed = 20f;
    
    [Header("滑冰式惯性")]
    public float acceleration = 3.0f;      // 加速度
    public float deceleration = 0.2f;      // 减速度（越小越滑）
    public float turnResponsiveness = 0.2f; // 转向灵敏度（越小越难转向）
    public float maxSlideSpeed = 0.15f;    // 最大滑行速度
    
    [Header("随机干扰")]
    public bool enableRandomForces = true;
    public float randomForceInterval = 0.5f;  // 随机力间隔
    public float randomForceStrength = 0.01f; // 随机力强度
    
    [Header("光圈参数")]
    public float lightRadius = 0.15f;
    //public float edgeSoftness = 0.08f;
    
    private Vector2 position = new Vector2(0.5f, 0.5f);
    private Vector2 velocity = Vector2.zero;
    private Vector2 targetDirection = Vector2.zero;
    private float lastRandomForceTime = 0f;
    
    void Start()
    {
        if (maskMaterial == null)
        {
            Image image = GetComponent<Image>();
            if (image != null && image.material != null)
            {
                maskMaterial = image.material;
            }
        }
        
        UpdateLight();
    }
    
    void Update()
    {
        HandleSlipperyInput();
        ApplySlidingPhysics();
        
        if (enableRandomForces)
        {
            ApplyRandomForces();
        }
        
        UpdateLight();
        
        // 调试显示
        Debug.Log($"速度: {velocity.magnitude:F3}, 方向: {targetDirection}");
    }
    
    void HandleSlipperyInput()
    {
        // 获取输入方向
        Vector2 input = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );
        
        // 输入平滑处理
        if (input.magnitude > 0.1f)
        {
            // 缓慢地改变目标方向（模拟转向延迟）
            targetDirection = Vector2.Lerp(
                targetDirection, 
                input.normalized, 
                turnResponsiveness * Time.deltaTime * 10f
            );
            
            // 加速（但加速缓慢）
            float currentSpeed = velocity.magnitude;
            float targetSpeed = Mathf.Min(baseSpeed * input.magnitude, maxSlideSpeed);
            
            if (currentSpeed < targetSpeed)
            {
                velocity += targetDirection * acceleration * Time.deltaTime;
            }
        }
        else
        {
            // 没有输入时，非常缓慢地减速（模拟冰面）
            if (velocity.magnitude > 0.01f)
            {
                velocity = Vector2.Lerp(velocity, Vector2.zero, deceleration * Time.deltaTime);
            }
            else
            {
                velocity = Vector2.zero;
            }
        }
    }
    
    void ApplySlidingPhysics()
    {
        // 应用速度
        position.x = Mathf.Clamp01(position.x + velocity.x * Time.deltaTime);
        position.y = Mathf.Clamp01(position.y + velocity.y * Time.deltaTime);
        
        // 边界处理：碰壁后失去大部分速度
        if (position.x <= 0.01f || position.x >= 0.99f)
        {
            velocity.x *= -0.3f; // 反弹但损失能量
            position.x = Mathf.Clamp(position.x, 0.01f, 0.99f);
        }
        if (position.y <= 0.01f || position.y >= 0.99f)
        {
            velocity.y *= -0.3f;
            position.y = Mathf.Clamp(position.y, 0.01f, 0.99f);
        }
        
        // 模拟空气阻力（很小，让惯性持续）
        velocity *= 0.995f;
    }
    
    void ApplyRandomForces()
    {
        if (Time.time - lastRandomForceTime > randomForceInterval)
        {
            // 添加随机方向的力
            Vector2 randomForce = Random.insideUnitCircle * randomForceStrength;
            velocity += randomForce;
            
            lastRandomForceTime = Time.time;
            
            Debug.Log($"随机力: {randomForce}");
        }
    }
    
    void UpdateLight()
    {
        if (maskMaterial != null)
        {
            maskMaterial.SetVector("_Center", new Vector4(position.x, position.y, 0, 0));
            maskMaterial.SetFloat("_Radius", lightRadius);
            //maskMaterial.SetFloat("_Softness", edgeSoftness);
        }
    }
    
    // 外部调用：增加干扰
    public void AddDisturbance(Vector2 force)
    {
        velocity += force;
    }
    
    // 重置位置和速度
    public void ResetToCenter()
    {
        position = new Vector2(0.5f, 0.5f);
        velocity = Vector2.zero;
        targetDirection = Vector2.zero;
    }
}