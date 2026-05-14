using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class ColorManager : MonoBehaviour
{
    // ── SİNGLETON PATTERN ──────────────────────────────────────
    public static ColorManager Instance { get; private set; }

    [Header("Ayarlar")]
    public int totalCrystals = 5;          // Toplam kristal sayısı
    public float transitionDuration = 2f;  // Renk geçiş süresi

    [Header("Post-Processing")]
    public Volume globalVolume;            // Sahnedeki Global Volume

    private ColorAdjustments colorAdjustments;
    private int collectedCrystals = 0;

    void Awake()
    {
        // Singleton: Sadece bir tane ColorManager olsun
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // Global Volume'dan ColorAdjustments bileşenini al
        if (globalVolume != null)
        {
            globalVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
        }

        // Başlangıçta tam siyah-beyaz
        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = -100f;
        }
    }

    // ── KRİSTAL TOPLANDI ─────────────────────────────────────
    public void CrystalCollected()
    {
        collectedCrystals++;
        collectedCrystals = Mathf.Clamp(collectedCrystals, 0, totalCrystals);

        // Hedef doygunluk değerini hesapla
        // 0 kristal = -100 (tam siyah-beyaz)
        // totalCrystals kristal = 0 (tam renkli)
        float targetSaturation = Mathf.Lerp(-100f, 0f, (float)collectedCrystals / totalCrystals);

        // Smooth geçiş başlat
        StopAllCoroutines();
        StartCoroutine(TransitionColor(targetSaturation));

        Debug.Log($"Kristal toplandı! {collectedCrystals}/{totalCrystals}");
    }

    // ── SMOOTH RENK GEÇİŞİ ──────────────────────────────────
    IEnumerator TransitionColor(float targetSaturation)
    {
        if (colorAdjustments == null) yield break;

        float startSaturation = colorAdjustments.saturation.value;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;

            // EaseInOut ile daha güzel geçiş
            t = t * t * (3f - 2f * t);

            colorAdjustments.saturation.value = Mathf.Lerp(startSaturation, targetSaturation, t);
            yield return null;
        }

        colorAdjustments.saturation.value = targetSaturation;

        // Tüm kristaller toplandıysa
        if (collectedCrystals >= totalCrystals)
        {
            OnAllCrystalsCollected();
        }
    }

    // ── TÜM KRİSTALLER TOPLANDINDA ──────────────────────────
    void OnAllCrystalsCollected()
    {
        Debug.Log("TÜM KRİSTALLER TOPLANDII! DÜNYA RENKLENDİ!");
        
        // Sahnede kalan tüm küçük düşmanları bul ve arındır (yok et)
        EnemyHealth[] remainingEnemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        foreach (EnemyHealth enemy in remainingEnemies)
        {
            Destroy(enemy.gameObject);
        }
        
        Debug.Log(remainingEnemies.Length + " adet karanlık düşman aydınlığa kavuşup yok oldu!");
        // Faz 7'de buraya level geçişi ekleyeceğiz
    }

    // ── UI İÇİN GETTER ───────────────────────────────────────
    public int GetCollectedCrystals() => collectedCrystals;
    public int GetTotalCrystals() => totalCrystals;
}
