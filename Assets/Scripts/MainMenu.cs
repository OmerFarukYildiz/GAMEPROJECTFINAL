using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Menü Panelleri")]
    public GameObject mainMenuPanel;
    public GameObject optionsMenuPanel;

    [Header("Ayarlar")]
    public Slider volumeSlider;

    void Start()
    {
        // Başlangıçta ana menüyü göster, ayarları gizle
        ShowMainMenu();

        // Eğer daha önceden kaydedilmiş bir ses ayarı varsa onu yükle, yoksa 1 (maksimum) yap
        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("GameVolume", 1f);
            SetVolume(volumeSlider.value);
        }
    }

    // ── ANA BUTONLAR ────────────────────────────────────

    public void PlayGame()
    {
        // 1. sahneyi yükle (File -> Build Settings kısmında MainMenu 0, Oyun sahnesi 1 olmalı)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Oyun Kapatılıyor...");
        Application.Quit();
    }

    // ── MENÜ GEÇİŞLERİ ──────────────────────────────────

    public void ShowOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        optionsMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // ── AYARLAR ─────────────────────────────────────────

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("GameVolume", volume); // Ses ayarını kaydet ki oyunu açıp kapattığında aynı kalsın
    }
}
