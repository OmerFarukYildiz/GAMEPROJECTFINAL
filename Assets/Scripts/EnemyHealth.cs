using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Can Ayarları")]
    public int maxHealth = 10;
    private int currentHealth;
    
    private SpriteRenderer sr;
    private Color originalColor;
    private Coroutine flashCoroutine;
    private bool isDead = false;

    private Collider2D myCollider;
    private static System.Collections.Generic.List<Collider2D> allEnemyColliders = new System.Collections.Generic.List<Collider2D>();

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;

        // Bütün düşmanların birbirinin içinden geçmesi için çarpışmaları görmezden gel
        myCollider = GetComponent<Collider2D>();
        if (myCollider != null)
        {
            foreach (var otherCol in allEnemyColliders)
            {
                if (otherCol != null)
                {
                    Physics2D.IgnoreCollision(myCollider, otherCol);
                }
            }
            allEnemyColliders.Add(myCollider);
        }

        // Fizik ayarlarını otomatik düzelt (Takla atmayı ve ittirilmeyi engelle)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.freezeRotation = true; // Z ekseninde dönmeyi (takla atmayı) kilitler
            rb.mass = 1000f; // Oyuncunun düşmanı itmesini veya havaya uçurmasını engeller
        }
    }

    void OnDestroy()
    {
        if (myCollider != null)
        {
            allEnemyColliders.Remove(myCollider);
        }
    }

    public void TakeDamage(int damage, Transform damageSource = null)
    {
        if (isDead) return;

        // Zırh kontrolü: Eğer zırhlıysa ve hasar önden geliyorsa blokla
        EnemyArmored armored = GetComponent<EnemyArmored>();
        if (armored != null && armored.BlocksDamage(damageSource))
        {
            Debug.Log("Saldırı Şövalye tarafından BLOKLANDI!");
            
            // Blokladığında gri renkte parlasın (görsel geri bildirim)
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashColor(Color.gray));
            
            return; // Hasar alma
        }

        currentHealth -= damage;
        Debug.Log($"Düşman hasar aldı! Kalan can: {currentHealth}");

        // Vurulma efekti (kırmızı parlasın)
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashColor(Color.red));

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    System.Collections.IEnumerator FlashColor(Color color)
    {
        if (sr != null)
        {
            sr.color = color;
            yield return new WaitForSeconds(0.1f);
            sr.color = originalColor;
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Düşman öldü!");
        
        EnemyExplode explode = GetComponent<EnemyExplode>();
        EnemyMultiplier multiplier = GetComponent<EnemyMultiplier>();

        // Eğer çoğalan bir düşmansa bölünmeyi tetikle
        if (multiplier != null)
        {
            multiplier.TriggerSplit();
        }

        // Eğer patlayan bir düşmansa patlamayı tetikle
        if (explode != null)
        {
            explode.TriggerExplosion();
        }
        else
        {
            // Patlamayacaksa direkt yok et (bölünme prefabları zaten üretildi)
            Destroy(gameObject);
        }
    }
}
