using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Referanslar")]
    public Image healthBarFill;     // Yeşil dolan bar
    public PlayerHealth playerHealth;

    void Update()
    {
        if (playerHealth == null || healthBarFill == null) return;

        // Fill Amount = mevcut can / max can (0 ile 1 arası bir değer)
        float fillAmount = (float)playerHealth.GetCurrentHealth() / playerHealth.maxHealth;
        
        // Daha pürüzsüz (smooth) bir azalış efekti
        healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, fillAmount, Time.deltaTime * 10f);
    }
}
