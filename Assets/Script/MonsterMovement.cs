using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 3.5f; // 怪物移动速度

    private NavMeshAgent agent;
    private Transform playerTarget;
    private Animator anim; // 动画控制器组件

    void Start()
    {
        // 获取导航组件
        agent = GetComponent<NavMeshAgent>();
        // 获取动画组件
        anim = GetComponent<Animator>();

        // 2D 导航的基础设置，防止怪物在 2D 场景里乱转
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // 应用速度
        agent.speed = moveSpeed;

        // 根据标签寻找玩家
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTarget == null) return;

        // 实时更新速度（方便在 Inspector 面板调试）
        agent.speed = moveSpeed;

        if (agent.isOnNavMesh) 
        {
            // 设置导航目标为玩家位置
            agent.SetDestination(playerTarget.position);
            
            // 核心逻辑：根据移动方向更新 Animator 参数
            UpdateAnimationDirection();
        }
        else
        {
            // 如果怪物脱离导航网格，尝试将其拉回
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out hit, 2.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
    }

    /// <summary>
    /// 根据 NavMeshAgent 的当前速度决定播放哪个方向的动画
    /// </summary>
    void UpdateAnimationDirection()
    {
        if (anim == null) return;

        Vector2 velocity = agent.velocity;

        // 如果移动速度非常小（几乎静止），则保持当前动画
        if (velocity.magnitude < 0.1f) return;

        // 比较 X 轴和 Y 轴的移动量，取较大的那个方向作为主方向
        if (Mathf.Abs(velocity.y) > Mathf.Abs(velocity.x))
        {
            // 纵向移动为主
            if (velocity.y > 0) 
                anim.SetInteger("dir", 1); // 1 代表向上
            else 
                anim.SetInteger("dir", 2); // 2 代表向下
        }
        else
        {
            // 横向移动为主
            if (velocity.x < 0) 
                anim.SetInteger("dir", 3); // 3 代表向左
            else 
                anim.SetInteger("dir", 4); // 4 代表向右
        }
    }

    // 供其他脚本调用的方法：设置怪物速度
    public void SetMonsterSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }
    }

    // 碰撞检测：如果碰到玩家则触发失败逻辑
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerLost();
            }
        }
    }
}