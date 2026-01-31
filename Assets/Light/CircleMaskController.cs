using UnityEngine;
using UnityEngine.UI;

public class CircleMaskController : MonoBehaviour
{
    [Header("遮罩设置")]
    public Material maskMaterial;      // 光圈遮罩材质
    public float moveSpeed = 0.5f;     // 光圈移动速度
    
    [Header("光圈参数")]
    [Range(0.05f, 0.5f)]
    public float radius = 0.2f;        // 光圈半径
    
    [Range(0f, 0.3f)]
    public float softness = 0.1f;      // 边缘羽化
    
    private Vector2 center = new Vector2(0.5f, 0.5f);  // 光圈中心位置
    
    void Start()
    {
        // 如果没有指定材质，尝试从Image组件获取
        if (maskMaterial == null)
        {
            Image image = GetComponent<Image>();
            if (image != null && image.material != null)
            {
                maskMaterial = image.material;
            }
        }
        
        // 初始化材质参数
        UpdateMaterialProperties();
    }
    
    void Update()
    {
        HandleKeyboardInput();
    }
    
    void HandleKeyboardInput()
    {
        // 获取键盘输入（方向键）
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        
        // 如果有输入，移动光圈
        if (moveX != 0 || moveY != 0)
        {
            // 计算移动距离（基于时间，保持平滑）
            float deltaX = moveX * moveSpeed * Time.deltaTime;
            float deltaY = moveY * moveSpeed * Time.deltaTime;
            
            // 更新中心位置
            center.x = Mathf.Clamp01(center.x + deltaX);
            center.y = Mathf.Clamp01(center.y + deltaY);
            
            // 更新材质
            UpdateMaterialProperties();
        }
    }
    
    // 更新材质参数
    void UpdateMaterialProperties()
    {
        if (maskMaterial != null)
        {
            maskMaterial.SetVector("_Center", new Vector4(center.x, center.y, 0, 0));
            maskMaterial.SetFloat("_Radius", radius);
            maskMaterial.SetFloat("_Softness", softness);
        }
    }
    
    // 设置光圈中心位置（归一化坐标，0-1）
    public void SetCenter(float x, float y)
    {
        center.x = Mathf.Clamp01(x);
        center.y = Mathf.Clamp01(y);
        UpdateMaterialProperties();
    }
    
    // 设置光圈半径
    public void SetRadius(float newRadius)
    {
        radius = Mathf.Clamp(newRadius, 0.05f, 0.5f);
        UpdateMaterialProperties();
    }
    
    // 设置边缘羽化
    public void SetSoftness(float newSoftness)
    {
        softness = Mathf.Clamp(newSoftness, 0f, 0.3f);
        UpdateMaterialProperties();
    }
    
    // 在编辑器中显示调试信息
    void OnDrawGizmosSelected()
    {
        if (maskMaterial != null)
        {
            UpdateMaterialProperties();
        }
    }
}