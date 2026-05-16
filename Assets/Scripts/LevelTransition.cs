using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    [Header("Geçiş Ayarları")]
    public string nextSceneName = "Level_02";  // Sonraki sahnenin adı
    public bool requireBossDefeated = true;    // Boss'un kesilmesi/kristalin alınması zorunlu mu?

    private bool playerEntered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (playerEntered) return;

        if (other.CompareTag("Player"))
        {
            // Eğer boss kesilme / kristal alınma şartı varsa kontrol et
            if (requireBossDefeated && ColorManager.Instance != null)
            {
                // Örnek: Eğer hiç kristal toplanmamışsa (0 ise) geçişe izin verme. 
                // İleride her level için "mevcut level kristali" kontrolü yapılabilir.
                if (ColorManager.Instance.GetCollectedCrystals() == 0)
                {
                    Debug.Log("Önce bu bölümün Boss'unu kesip kristalini almalısın!");
                    return;
                }
            }

            playerEntered = true;

            // İlerlemeyi kaydet
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SaveProgress();
            }

            // Sahneyi geçiş efektiyle yükle
            if (SceneTransition.Instance != null)
            {
                SceneTransition.Instance.LoadScene(nextSceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}
