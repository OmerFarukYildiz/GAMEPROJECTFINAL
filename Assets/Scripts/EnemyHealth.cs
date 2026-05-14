using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Can Ayarları")]
    public int maxHealth = 6;
    private int currentHealth;
    
    private SpriteRenderer sr;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Düşman hasar aldı! Kalan can: {currentHealth}");

        // Vurulma efekti
        StartCoroutine(FlashWhite());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    System.Collections.IEnumerator FlashWhite()
    {
        if (sr != null)
        {
            Color originalColor = sr.color;
            sr.color = Color.white; // Vurulunca bembeyaz parlasın
            yield return new WaitForSeconds(0.15f);
            sr.color = originalColor; // Sonra rengi normale dönsün
        }
    }

    void Die()
    {
        Debug.Log("Düşman öldü!");
        Destroy(gameObject);
    }
}
