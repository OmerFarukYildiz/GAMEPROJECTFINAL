using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class ColorManager : MonoBehaviour
{
    // ── SİNGLETON PATTERN ──────────────────────────────────────
    public static ColorManager Instance { get; private set; }

    [Header("Ayarlar (Bu Sahnenin Rengi İçin)")]
    public System.Collections.Generic.List<string> requiredCrystalIDs = new System.Collections.Generic.List<string>() { "Crystal_1" };
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

        // Başlangıç rengini ayarla (Toplanan kristale göre)
        if (colorAdjustments != null)
        {
            int collectedCount = GetCollectedCount();
            float startSaturation = -100f;
            
            if (requiredCrystalIDs.Count > 0)
            {
                startSaturation = Mathf.Lerp(-100f, 0f, (float)collectedCount / requiredCrystalIDs.Count);
            }
            
            colorAdjustments.saturation.value = startSaturation;
        }
    }

    private int GetCollectedCount()
    {
        int count = 0;
        if (GameManager.Instance != null)
        {
            foreach (string id in requiredCrystalIDs)
            {
                if (GameManager.Instance.IsCrystalCollected(id)) count++;
            }
        }
        return count;
    }

    // ── KRİSTAL TOPLANDI ─────────────────────────────────────
    public void CrystalCollected(string crystalID)
    {
        if (requiredCrystalIDs.Contains(crystalID) || requiredCrystalIDs.Count == 0)
        {
            int collectedCount = GetCollectedCount();
            if (GameManager.Instance != null && !GameManager.Instance.IsCrystalCollected(crystalID))
            {
                collectedCount++; 
            }
            
            collectedCount = Mathf.Min(collectedCount, requiredCrystalIDs.Count);

            float targetSaturation = -100f;
            if (requiredCrystalIDs.Count > 0)
            {
                targetSaturation = Mathf.Lerp(-100f, 0f, (float)collectedCount / requiredCrystalIDs.Count);
            }

            // Smooth geçiş başlat
            StopAllCoroutines();
            StartCoroutine(TransitionColor(targetSaturation, collectedCount >= requiredCrystalIDs.Count));

            Debug.Log($"Kristal toplandı! Sahne Rengi Açılıyor: {collectedCount}/{requiredCrystalIDs.Count}");
        }
    }

    // ── SMOOTH RENK GEÇİŞİ ──────────────────────────────────
    IEnumerator TransitionColor(float targetSaturation, bool allCollected)
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
        if (allCollected && requiredCrystalIDs.Count > 0)
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

    // ── UI İÇİN GETTER (Opsiyonel) ───────────────────────────
    public int GetCollectedCrystals() => GetCollectedCount();
    public int GetTotalCrystals() => requiredCrystalIDs.Count;
}
