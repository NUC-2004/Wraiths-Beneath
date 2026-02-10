using UnityEngine;

public class MonsterProximity : MonoBehaviour
{
    private Transform player;
    private bool canPlay = true; // --- 新增控制开关 ---
    
    [Header("距离设置")]
    public float detectRange = 10f;
    public float attackRange = 2f;

    [Header("音效组件")]
    public AudioSource proximitySource;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        // --- 核心逻辑：如果被禁用或者玩家死了，就停掉声音并不再运行 ---
        if (!canPlay || player == null) 
        {
            if (proximitySource.isPlaying) proximitySource.Stop();
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < detectRange)
        {
            if (!proximitySource.isPlaying) proximitySource.Play();
            float vol = 1 - (distance / detectRange);
            proximitySource.volume = Mathf.Clamp01(vol);
        }
        else
        {
            if (proximitySource.isPlaying) proximitySource.Stop();
        }
    }

    // --- 新增：供 GameManager 远程关闭的方法 ---
    public void StopProximitySound()
    {
        canPlay = false;
        if (proximitySource != null) proximitySource.Stop();
    }
}