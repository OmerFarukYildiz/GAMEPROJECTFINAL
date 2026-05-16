using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public BossController boss;

    void Start()
    {
        // Boss odasının sınırlarını (BoxCollider) alıp otomatik olarak Boss'a ilet
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null && boss != null)
        {
            boss.minX = col.bounds.min.x;
            boss.maxX = col.bounds.max.x;
            boss.useArenaLimits = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (boss != null)
            {
                boss.CancelResetTimer(); // Varsa sıfırlama sayacını durdur
                boss.ActivateBoss();     // Boss'u başlat veya devam ettir
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (boss != null)
            {
                boss.StartResetTimer(); // 10 saniyelik sayacı başlat
            }
        }
    }
}
