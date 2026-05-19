using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    public static PlayerMana Instance { get; private set; }

    [Header("Mana Ayarları")]
    public int maxMana = 100;
    private int currentMana;

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
        currentMana = maxMana; // Başlangıçta full

        // UI'ı başlat
        if (ManaUI.Instance != null)
        {
            ManaUI.Instance.UpdateMana(currentMana, maxMana);
        }
    }

    public void RestoreMana(int amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        Debug.Log($"Mana kazanıldı! Mevcut Mana: {currentMana}");
        
        if (ManaUI.Instance != null)
        {
            ManaUI.Instance.UpdateMana(currentMana, maxMana);
        }
    }

    public bool UseMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            Debug.Log($"Mana Harcandı! Mevcut Mana: {currentMana}");
            
            if (ManaUI.Instance != null)
            {
                ManaUI.Instance.UpdateMana(currentMana, maxMana);
            }
            return true;
        }
        
        Debug.Log("Yetersiz Mana!");
        return false;
    }

    public int GetCurrentMana()
    {
        return currentMana;
    }
}
