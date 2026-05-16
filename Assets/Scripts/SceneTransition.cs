using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    // ── SİNGLETON ────────────────────────────────────────────
    public static SceneTransition Instance { get; private set; }

    [Header("Fade Paneli")]
    public Image fadePanel;              // Siyah panel
    public float fadeDuration = 1f;     // Fade hızı

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // Sahne açılırken Fade In (siyahtan şeffafa)
        if (fadePanel != null)
        {
            StartCoroutine(FadeIn());
        }
    }

    // ── FADE IN (siyah → şeffaf) ──────────────────────────────
    public IEnumerator FadeIn()
    {
        fadePanel.gameObject.SetActive(true);
        float elapsed = 0f;
        Color c = fadePanel.color;
        c.a = 1f;
        fadePanel.color = c;

        while (elapsed < fadeDuration)
        {
            if (fadePanel == null) yield break; // Obje silindiyse durdur
            elapsed += Time.deltaTime;
            c.a = 1f - (elapsed / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }

        if (fadePanel != null)
        {
            c.a = 0f;
            fadePanel.color = c;
            fadePanel.gameObject.SetActive(false);
        }
    }

    // ── FADE OUT (şeffaf → siyah) + Sahne Geçişi ─────────────
    public IEnumerator FadeOut(string sceneName)
    {
        fadePanel.gameObject.SetActive(true);
        float elapsed = 0f;
        Color c = fadePanel.color;
        c.a = 0f;
        fadePanel.color = c;

        while (elapsed < fadeDuration)
        {
            if (fadePanel == null) yield break; // Obje silindiyse durdur
            elapsed += Time.deltaTime;
            c.a = elapsed / fadeDuration;
            fadePanel.color = c;
            yield return null;
        }

        if (fadePanel != null)
        {
            c.a = 1f;
            fadePanel.color = c;
        }

        // Sahneyi arka planda yükle
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        if (asyncLoad == null)
        {
            Debug.LogError($"'{sceneName}' adında bir sahne bulunamadı! Lütfen Build Settings'e eklendiğinden emin ol.");
            yield break;
        }

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // ── KOLAY KULLANIM ────────────────────────────────────────
    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }
}
