using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    // --- 新增：在 Inspector 面板中控制速度 ---
    [Header("移动设置")]
    public float moveSpeed = 3.5f; // 默认速度设为 3.5

    private NavMeshAgent agent;
    private Transform playerTarget;
    private SpriteRenderer sp;

    void Start()
    {
        // 获取组件（清理了重复的代码）
        agent = GetComponent<NavMeshAgent>();
        sp = GetComponent<SpriteRenderer>();

        // 2D 导航的基础设置
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // --- 核心修改：将面板设置的速度应用到组件上 ---
        agent.speed = moveSpeed;

        // 寻找玩家
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTarget == null) return;

        // 如果你在运行中手动修改了 moveSpeed，这行代码能保证速度实时更新
        agent.speed = moveSpeed;

        if (agent.isOnNavMesh) 
        {
            agent.SetDestination(playerTarget.position);
            
            if (agent.velocity.x > 0.1f) sp.flipX = false;
            else if (agent.velocity.x < -0.1f) sp.flipX = true;
        }
        else
        {
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out hit, 2.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
    }

    // --- 新增：供外部调用的 Public 方法 ---
    // 比如你想在某个触发器里让怪物突然加速，就可以调用这个方法
    public void SetMonsterSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.PlayerLost();
        }
    }
}