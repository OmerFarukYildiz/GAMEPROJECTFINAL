using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [Header("Referanslar")]
    public GameObject bossHealthPanel;   // Boss paneli (arkaplanı dahil)
    public Image bossHealthFill;         // Dolan bar
    public BossController bossController;

    void Update()
    {
        if (bossController == null) return;

        // Boss Idle veya Death durumunda değilse barı göster
        bool isActive = bossController.currentState != BossController.BossState.Idle
                     && bossController.currentState != BossController.BossState.Death;

        bossHealthPanel.SetActive(isActive);

        if (isActive)
        {
            float fill = (float)bossController.GetCurrentHealth() / bossController.maxHealth;
            bossHealthFill.fillAmount = Mathf.Lerp(bossHealthFill.fillAmount, fill, Time.deltaTime * 8f);
        }
    }
}
