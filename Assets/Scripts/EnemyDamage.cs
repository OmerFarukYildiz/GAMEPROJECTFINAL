using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [Header("Hasar Ayarları")]
    public int damageAmount = 3;
    public float knockbackForce = 5f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Çarpışan obje Player mi?
        if (collision.gameObject.CompareTag("Player"))
        {
            // Oyuncunun can scriptini al
            PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damageAmount);
            }

            // Knockback (geri atma)
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(new Vector2(knockbackDir.x * knockbackForce, knockbackForce), ForceMode2D.Impulse);
            }
        }
    }
}
