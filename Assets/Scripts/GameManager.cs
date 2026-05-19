using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ── SİNGLETON ────────────────────────────────────────────
    public static GameManager Instance { get; private set; }

    [Header("Oyun Durumu")]
    public int collectedCrystals = 0;
    public int playerHealth = 5;
    public int currentLevel = 1;

    [Header("Bonfire & Fast Travel")]
    public string lastBonfireID = "";
    public string lastBonfireScene = "";
    public Vector3 lastBonfirePosition;
    
    [System.Serializable]
    public class BonfireData
    {
        public string bonfireID;
        public string sceneName;
        public Vector3 spawnPosition;
    }
    
    public System.Collections.Generic.List<BonfireData> unlockedBonfires = new System.Collections.Generic.List<BonfireData>();

    [Header("Kalıcı Kayıt (Dünya Sıfırlanınca Kalanlar)")]
    public System.Collections.Generic.List<string> deadBosses = new System.Collections.Generic.List<string>();
    public System.Collections.Generic.List<string> collectedCrystalsList = new System.Collections.Generic.List<string>();

    // ── KALICI KAYIT FONKSİYONLARI ────────────────────────────
    public void RegisterDeadBoss(string bossID)
    {
        if (!string.IsNullOrEmpty(bossID) && !deadBosses.Contains(bossID)) deadBosses.Add(bossID);
    }

    public bool IsBossDead(string bossID)
    {
        return deadBosses.Contains(bossID);
    }

    public void RegisterCollectedCrystal(string crystalID)
    {
        if (!string.IsNullOrEmpty(crystalID) && !collectedCrystalsList.Contains(crystalID)) collectedCrystalsList.Add(crystalID);
    }

    public bool IsCrystalCollected(string crystalID)
    {
        return collectedCrystalsList.Contains(crystalID);
    }

    // ── BONFIRE FONKSİYONLARI ─────────────────────────────────
    public void UnlockBonfire(string id, string scene, Vector3 pos)
    {
        if (!unlockedBonfires.Exists(b => b.bonfireID == id))
        {
            unlockedBonfires.Add(new BonfireData { bonfireID = id, sceneName = scene, spawnPosition = pos });
            Debug.Log("Yeni Bonfire açıldı: " + id);
        }
    }

    public void SetLastBonfire(string id, string scene, Vector3 pos)
    {
        lastBonfireID = id;
        lastBonfireScene = scene;
        lastBonfirePosition = pos;
        Debug.Log("Kayıt noktası güncellendi: " + id);
    }

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
        unlockedBonfires.Clear();
        deadBosses.Clear();
        collectedCrystalsList.Clear();
        lastBonfireID = "";
        lastBonfireScene = "";
        PlayerPrefs.DeleteAll();
    }
}
