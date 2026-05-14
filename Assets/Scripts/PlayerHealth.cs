using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Can Ayarları")]
    public int maxHealth = 5;
    public float invincibilityDuration = 1.5f; // Hasar sonrası dokunulmazlık süresi

    private int currentHealth;
    private bool isInvincible = false;
    private SpriteRenderer sr;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return; // Dokunulmazsa hasar alma

        currentHealth -= damage;
        Debug.Log($"Oyuncu hasar aldı! Kalan can: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityFrames());
        }
    }

    System.Collections.IEnumerator InvincibilityFrames()
    {
        isInvincible = true;

        // Yanıp sönme efekti
        float timer = 0f;
        while (timer < invincibilityDuration)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        sr.enabled = true;
        isInvincible = false;
    }

    void Die()
    {
        Debug.Log("Oyuncu öldü! Game Over.");
        // Faz 6'da UI ekleyeceğiz, şimdilik sahnesi yenile
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public int GetCurrentHealth() => currentHealth;
}
