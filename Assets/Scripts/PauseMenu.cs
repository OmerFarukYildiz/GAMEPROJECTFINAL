using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [Header("Menü Panelleri")]
    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;

    [Header("Ayarlar")]
    public Slider volumeSlider;

    void Start()
    {
        // Başlangıçta pause menüsünün kapalı olduğundan emin ol
        pauseMenuUI.SetActive(false);
        if (optionsMenuUI != null) optionsMenuUI.SetActive(false);
        GameIsPaused = false;
        Time.timeScale = 1f;

        // Sesi kayıttan çek
        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("GameVolume", 1f);
            SetVolume(volumeSlider.value);
        }
    }

    void Update()
    {
        // P veya ESC tuşuna basıldığında
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (GameIsPaused)
            {
                // Eğer options menüsündeysek ESC ile pause menüsüne dön, değilse oyuna dön
                if (optionsMenuUI != null && optionsMenuUI.activeSelf)
                {
                    ShowPauseMenu();
                }
                else
                {
                    Resume();
                }
            }
            else
            {
                Pause();
            }
        }
    }

    // ── TEMEL İŞLEMLER ──────────────────────────────────

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        if (optionsMenuUI != null) optionsMenuUI.SetActive(false);
        
        Time.timeScale = 1f; // Zamanı normal akışına döndür
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        if (optionsMenuUI != null) optionsMenuUI.SetActive(false);
        
        Time.timeScale = 0f; // Zamanı durdur
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f; // Ana menüye dönerken zamanı düzeltmeyi unutma!
        GameIsPaused = false;
        
        // "MainMenu" adında bir sahnen varsa onu yükler. Ya da 0. indexi yükleyebilirsin.
        // File -> Build Settings içindeki 0 numaralı sahne Ana Menü olmalı.
        SceneManager.LoadScene(0); 
    }

    // ── MENÜ GEÇİŞLERİ (OPTIONS) ─────────────────────────

    public void ShowOptions()
    {
        pauseMenuUI.SetActive(false);
        if (optionsMenuUI != null) optionsMenuUI.SetActive(true);
    }

    public void ShowPauseMenu()
    {
        if (optionsMenuUI != null) optionsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    // ── AYARLAR ─────────────────────────────────────────

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("GameVolume", volume);
    }
}
