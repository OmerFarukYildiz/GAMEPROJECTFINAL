using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [Header("Referanslar")]
    public GameObject bossHealthPanel;   // Boss paneli (arkaplanı dahil)
    public Image bossHealthFill;         // Dolan bar
    public BossController bossController;

    void Start()
    {
        // Başlangıçta paneli gizle ki odaya girmeden görünmesin
        if (bossHealthPanel != null)
        {
            bossHealthPanel.SetActive(false);
        }

        // Eğer BossController referansı Inspector'dan atanmamışsa, sahnede otomatik bulmaya çalış
        if (bossController == null)
        {
            bossController = FindObjectOfType<BossController>();
        }
    }

    void Update()
    {
        // Gerekli referanslar yoksa (boşsa) hata vermesini önle
        if (bossController == null || bossHealthPanel == null || bossHealthFill == null) return;

        // Boss Idle veya Death durumunda değilse barı göster
        bool isActive = bossController.currentState != BossController.BossState.Idle
                     && bossController.currentState != BossController.BossState.Death;

        if (bossHealthPanel.activeSelf != isActive)
        {
            bossHealthPanel.SetActive(isActive);
        }

        if (isActive)
        {
            float fill = (float)bossController.GetCurrentHealth() / bossController.maxHealth;
            bossHealthFill.fillAmount = Mathf.Lerp(bossHealthFill.fillAmount, fill, Time.deltaTime * 8f);
        }
    }
}
