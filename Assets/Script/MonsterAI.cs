using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterAI : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 3.5f;
    public float sightBoostMultiplier = 1.5f;

    private const string DirectionParameter = "dir";

    private NavMeshAgent agent;
    private Transform playerTarget;
    private Animator anim;
    private bool isStopped;
    private bool hasSightBoost;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        ApplyAgentSpeed();
        if (agent.enabled)
        {
            agent.isStopped = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    void Update()
    {
        if (isStopped || playerTarget == null)
        {
            return;
        }

        ApplyAgentSpeed();

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
        ApplyAgentSpeed();
    }

    public void SetSightBoost(bool boosted, float multiplier)
    {
        hasSightBoost = boosted;
        sightBoostMultiplier = Mathf.Max(1f, multiplier);
        ApplyAgentSpeed();
    }

    public void StopChasing()
    {
        isStopped = true;

        if (agent == null)
        {
            return;
        }

        if (!agent.enabled || !agent.isOnNavMesh)
        {
            return;
        }

        agent.ResetPath();
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
    }

    private void ApplyAgentSpeed()
    {
        if (agent == null || isStopped)
        {
            return;
        }

        agent.speed = moveSpeed * (hasSightBoost ? sightBoostMultiplier : 1f);
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
        if (!isStopped && other.CompareTag("Player") && GameManager.Instance != null)
        {
            GameManager.Instance.PlayerLost();
        }
    }
}
