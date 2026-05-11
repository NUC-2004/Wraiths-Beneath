using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && GameManager.Instance != null)
        {
            GameManager.Instance.PlayerWin();
        }
    }
}
