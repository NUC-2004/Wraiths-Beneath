using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 3.5f;

    private const string DirectionParameter = "dir";

    private NavMeshAgent agent;
    private Transform playerTarget;
    private Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = moveSpeed;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTarget == null)
        {
            return;
        }

        agent.speed = moveSpeed;

        if (agent.isOnNavMesh)
        {
            agent.SetDestination(playerTarget.position);
            UpdateAnimationDirection();
            return;
        }

        TryWarpBackToNavMesh();
    }

    public void SetMonsterSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }
    }

    private void UpdateAnimationDirection()
    {
        if (anim == null)
        {
            return;
        }

        Vector2 velocity = agent.velocity;
        if (velocity.magnitude < 0.1f)
        {
            return;
        }

        if (Mathf.Abs(velocity.y) > Mathf.Abs(velocity.x))
        {
            anim.SetInteger(DirectionParameter, velocity.y > 0f ? 1 : 2);
        }
        else
        {
            anim.SetInteger(DirectionParameter, velocity.x < 0f ? 3 : 4);
        }
    }

    private void TryWarpBackToNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && GameManager.Instance != null)
        {
            GameManager.Instance.PlayerLost();
        }
    }
}
