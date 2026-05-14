using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public BossController boss;

    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            if (boss != null)
                boss.ActivateBoss();
        }
    }
}
