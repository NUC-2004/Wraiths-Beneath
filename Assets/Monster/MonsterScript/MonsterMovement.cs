using UnityEngine;
using UnityEngine.AI; // 1. 引入AI命名空间

public class MonsterAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerTarget;
    private SpriteRenderer sp;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        sp = GetComponent<SpriteRenderer>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        // 获取组件
        agent = GetComponent<NavMeshAgent>();
        sp = GetComponent<SpriteRenderer>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // 寻找玩家
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    void Update()
    {
        // 1. 如果没有目标，啥也不做
        if (playerTarget == null) return;

        // 2. 核心修复：只有当怪物“脚踏实地”时，才移动
        if (agent.isOnNavMesh) 
        {
            agent.SetDestination(playerTarget.position);
            
            // 翻转图片逻辑
            if (agent.velocity.x > 0.1f) sp.flipX = false;
            else if (agent.velocity.x < -0.1f) sp.flipX = true;
        }
        else
        {
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out hit, 2.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                agent.Warp(hit.position); // 瞬移到正确的路面上
            }
        }
    }
}