using UnityEngine;
using UnityEngine.SceneManagement;

public class Bonfire : MonoBehaviour
{
    [Header("Bonfire Ayarları")]
    public string bonfireID = "Bonfire_Orman_1"; // Her bonfire için benzersiz bir isim olmalı
    public bool isLit = false;
    
    [Header("Görseller")]
    public GameObject fireParticles; // Ateşin yanma efekti
    public GameObject interactPrompt; // "E'ye Bas" yazısı (İsteğe bağlı)

    private bool playerInRange = false;

    void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);

        // Oyun başladığında bu bonfire GameManager'da açıksa otomatik yanık gelsin
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.unlockedBonfires.Exists(b => b.bonfireID == this.bonfireID))
            {
                isLit = true;
                if (fireParticles != null) fireParticles.SetActive(true);
            }
            else
            {
                if (fireParticles != null) fireParticles.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    void Interact()
    {
        if (!isLit)
        {
            // 1. AŞAMA: ATEŞİ YAKMA
            isLit = true;
            if (fireParticles != null) fireParticles.SetActive(true);
            
            GameManager.Instance.UnlockBonfire(bonfireID, SceneManager.GetActiveScene().name, transform.position);
            GameManager.Instance.SetLastBonfire(bonfireID, SceneManager.GetActiveScene().name, transform.position);
            
            Debug.Log("Bonfire yakıldı! Dinlenmek için tekrar E'ye basın.");
        }
        else
        {
            // 2. AŞAMA: ATEŞE OTURMA
            GameManager.Instance.SetLastBonfire(bonfireID, SceneManager.GetActiveScene().name, transform.position);
            
            // Canı Fullüyoruz
            PlayerHealth pHealth = FindAnyObjectByType<PlayerHealth>();
            if (pHealth != null) pHealth.HealToFull();

            // Arayüzü Açıyoruz (Bu da zamanı durduracak ve düşman aggrosunu kesecek)
            if (FastTravelUI.Instance != null)
            {
                FastTravelUI.Instance.OpenBonfireMenu();
            }
            else
            {
                Debug.LogWarning("Sahnede FastTravelUI bulunamadı!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }
}
