using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ── SİNGLETON ────────────────────────────────────────────
    public static GameManager Instance { get; private set; }

    [Header("Oyun Durumu")]
    public int collectedCrystals = 0;
    public int playerHealth = 5;
    public int currentLevel = 1;

    void Awake()
    {
        // Singleton kontrolü
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Sahne değişse bile bu obje yok olmasın!
        DontDestroyOnLoad(gameObject);
    }

    // ── VERİ KAYDETME ─────────────────────────────────────────
    public void SaveProgress()
    {
        PlayerPrefs.SetInt("Crystals", collectedCrystals);
        PlayerPrefs.SetInt("Health", playerHealth);
        PlayerPrefs.SetInt("Level", currentLevel);
        PlayerPrefs.Save();
        Debug.Log("İlerleme kaydedildi! Mevcut Kristal: " + collectedCrystals);
    }

    public void LoadProgress()
    {
        collectedCrystals = PlayerPrefs.GetInt("Crystals", 0);
        playerHealth = PlayerPrefs.GetInt("Health", 5);
        currentLevel = PlayerPrefs.GetInt("Level", 1);
    }

    public void ResetProgress()
    {
        collectedCrystals = 0;
        playerHealth = 5;
        currentLevel = 1;
        PlayerPrefs.DeleteAll();
    }
}
