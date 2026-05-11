using UnityEngine;

public class MonsterProximity : MonoBehaviour
{
    [Header("距离设置")]
    public float detectRange = 10f;

    [Header("音效组件")]
    public AudioSource proximitySource;

    private Transform player;
    private bool canPlay = true;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        if (!canPlay || player == null || proximitySource == null)
        {
            StopSourceIfPlaying();
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < detectRange)
        {
            PlaySourceIfStopped();
            proximitySource.volume = Mathf.Clamp01(1f - distance / detectRange);
            return;
        }

        StopSourceIfPlaying();
    }

    public void StopProximitySound()
    {
        canPlay = false;
        StopSourceIfPlaying();
    }

    private void PlaySourceIfStopped()
    {
        if (!proximitySource.isPlaying)
        {
            proximitySource.Play();
        }
    }

    private void StopSourceIfPlaying()
    {
        if (proximitySource != null && proximitySource.isPlaying)
        {
            proximitySource.Stop();
        }
    }
}
