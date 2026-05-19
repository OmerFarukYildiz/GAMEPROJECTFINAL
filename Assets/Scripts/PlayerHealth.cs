using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Can Ayarları")]
    public int maxHealth = 20;
    public float invincibilityDuration = 1.5f; // Hasar sonrası dokunulmazlık süresi

    private int currentHealth;
    private bool isInvincible = false;
    private SpriteRenderer sr;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();

        // Eğer Bonfire'da doğmamız gerekiyorsa oraya ışınlan
        if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.lastBonfireScene))
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == GameManager.Instance.lastBonfireScene)
            {
                transform.position = GameManager.Instance.lastBonfirePosition;
            }
        }
    }

    public void HealToFull()
    {
        currentHealth = maxHealth;
        Debug.Log("Can fullendi!");
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Can yenilendi! Mevcut Can: {currentHealth}");
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
        Debug.Log("Oyuncu öldü!");

        string targetScene = "MainMenu";

        // Eğer bir Bonfire açıldıysa oraya dön
        if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.lastBonfireScene))
        {
            targetScene = GameManager.Instance.lastBonfireScene;
        }

        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.LoadScene(targetScene);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetScene);
        }
    }

    public int GetCurrentHealth() => currentHealth;
}
