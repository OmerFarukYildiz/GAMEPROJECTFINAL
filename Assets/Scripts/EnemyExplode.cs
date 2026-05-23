using UnityEngine;
using System.Collections;

public class EnemyExplode : MonoBehaviour
{
    [Header("Patlama Ayarları")]
    public float explosionRadius = 2.5f;
    public int explosionDamage = 2;
    public float delayBeforeExplosion = 1f;
    public Color swellColor = Color.red;

    private bool isExploding = false;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // EnemyHealth tarafından çağrılacak
    public void TriggerExplosion()
    {
        if (isExploding) return;
        isExploding = true;
        
        // Hareketi durdur
        // Hareketi ve düşmeyi durdur
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static; // Aşağı düşmesini ve itilmesini engeller
        }

        // Varsa devriye gezme scriptini kapat
        MonoBehaviour patrol = GetComponent("EnemyPatrol") as MonoBehaviour;
        if (patrol != null) patrol.enabled = false;

        StartCoroutine(ExplosionRoutine());
    }

    IEnumerator ExplosionRoutine()
    {
        float timer = 0f;
        Vector3 originalScale = transform.localScale;
        Color originalColor = sr.color;

        while (timer < delayBeforeExplosion)
        {
            timer += Time.deltaTime;
            
            // Şişme efekti
            float scaleAmount = Mathf.Lerp(1f, 1.5f, timer / delayBeforeExplosion);
            transform.localScale = originalScale * scaleAmount;

            // Renk değişimi (kızarma)
            sr.color = Color.Lerp(originalColor, swellColor, timer / delayBeforeExplosion);

            yield return null;
        }

        // Patla ve hasar ver
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hitPlayers)
        {
            if (hit.CompareTag("Player"))
            {
                // Player'ın can scriptine hasar ver (oyuncunun can sistemi olduğunu varsayıyoruz)
                hit.SendMessage("TakeDamage", explosionDamage, SendMessageOptions.DontRequireReceiver);
            }
        }

        // TODO: Buraya patlama görsel efekti eklenebilir
        Debug.Log("Slime PATLADI!");
        Destroy(gameObject); // Kendini yok et
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
