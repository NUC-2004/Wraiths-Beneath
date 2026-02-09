using UnityEngine;

public class MonsterProximity : MonoBehaviour
{
    private Transform player;
    
    [Header("距离设置")]
    public float detectRange = 10f;    // 开始听到声音的距离
    public float attackRange = 2f;     // 离得太近的危险距离

    [Header("音效组件")]
    public AudioSource proximitySource; // 拖入那个 3D 音效的 AudioSource

    void Start()
    {
        // 自动寻找玩家
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        // 1. 计算距离
        float distance = Vector3.Distance(transform.position, player.position);

        // 2. 根据距离控制音效 (如果你不想用 Unity 自带的 3D 衰减，可以用代码控)
        if (distance < detectRange)
        {
            if (!proximitySource.isPlaying) proximitySource.Play();
            
            // 越近越大声：音量从 0 到 1 变化
            float vol = 1 - (distance / detectRange);
            proximitySource.volume = Mathf.Clamp01(vol);
        }
        else
        {
            if (proximitySource.isPlaying) proximitySource.Stop();
        }

        // 3. (可选) 如果离得太近了，可以触发特定逻辑
        if (distance < attackRange)
        {
            // Debug.Log("怪物发现你了！");
        }
    }
}