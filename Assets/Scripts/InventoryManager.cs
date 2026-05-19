using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Depo Limitleri")]
    public int maxCrystals = 4;
    public int maxTalismans = 10;

    [Header("Envanter Verisi")]
    public List<ItemData> crystalStash = new List<ItemData>();
    public List<ItemData> talismanStash = new List<ItemData>();

    [Header("Takılı Eşyalar")]
    public ItemData equippedCrystal;
    public ItemData equippedTalisman;

    public event Action OnInventoryChanged; // UI güncellemesi için event

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Depoya eşya ekleme (Yerden alındığında çağrılır)
    public bool AddItemToStash(ItemData item)
    {
        if (item.itemType == ItemType.Crystal)
        {
            if (crystalStash.Count < maxCrystals)
            {
                crystalStash.Add(item);
                OnInventoryChanged?.Invoke();
                return true;
            }
        }
        else if (item.itemType == ItemType.Talisman)
        {
            if (talismanStash.Count < maxTalismans)
            {
                talismanStash.Add(item);
                OnInventoryChanged?.Invoke();
                return true;
            }
        }
        return false;
    }

    // Slota Eşya Takma
    public void EquipItem(ItemData item)
    {
        if (item.itemType == ItemType.Crystal)
        {
            if (equippedCrystal != null)
            {
                // Zaten bir kristal takılıysa onu depoya geri gönder
                crystalStash.Add(equippedCrystal);
            }
            equippedCrystal = item;
            crystalStash.Remove(item);
        }
        else if (item.itemType == ItemType.Talisman)
        {
            if (equippedTalisman != null)
            {
                // Eski tılsımın özelliklerini geri al
                RemoveTalismanEffects(equippedTalisman);
                talismanStash.Add(equippedTalisman);
            }
            equippedTalisman = item;
            talismanStash.Remove(item);
            
            // Yeni tılsımın özelliklerini uygula
            ApplyTalismanEffects(equippedTalisman);
        }

        OnInventoryChanged?.Invoke();
    }

    // Slottan Eşyayı Çıkarma (Depoya Geri Gönderme)
    public void UnequipItem(ItemType type)
    {
        if (type == ItemType.Crystal && equippedCrystal != null)
        {
            crystalStash.Add(equippedCrystal);
            equippedCrystal = null;
        }
        else if (type == ItemType.Talisman && equippedTalisman != null)
        {
            RemoveTalismanEffects(equippedTalisman);
            talismanStash.Add(equippedTalisman);
            equippedTalisman = null;
        }

        OnInventoryChanged?.Invoke();
    }

    // Tılsım Etkilerini Uygulama
    private void ApplyTalismanEffects(ItemData talisman)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null && talisman.healthBonus > 0)
            {
                health.maxHealth += talisman.healthBonus;
                health.Heal(talisman.healthBonus); // Max can artınca mevcut canı da artır
            }

            PlayerMana mana = player.GetComponent<PlayerMana>();
            if (mana != null && talisman.manaBonus > 0)
            {
                mana.maxMana += talisman.manaBonus;
                mana.RestoreMana(talisman.manaBonus);
            }

            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null && talisman.speedBonus > 0)
            {
                controller.moveSpeed += talisman.speedBonus;
            }
            
            Debug.Log($"Tılsım takıldı: {talisman.itemName}. Etkiler uygulandı.");
        }
    }

    // Tılsım Etkilerini Geri Alma
    private void RemoveTalismanEffects(ItemData talisman)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null && talisman.healthBonus > 0)
            {
                health.maxHealth -= talisman.healthBonus;
                // Mevcut can max candan büyükse kırp (PlayerHealth.cs'de ayarlanacak)
                if (health.GetCurrentHealth() > health.maxHealth)
                {
                    health.TakeDamage(health.GetCurrentHealth() - health.maxHealth); // Düzeltme
                }
            }

            PlayerMana mana = player.GetComponent<PlayerMana>();
            if (mana != null && talisman.manaBonus > 0)
            {
                mana.maxMana -= talisman.manaBonus;
                mana.RestoreMana(0); // Clamps and updates UI
            }

            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null && talisman.speedBonus > 0)
            {
                controller.moveSpeed -= talisman.speedBonus;
            }
            
            Debug.Log($"Tılsım çıkarıldı: {talisman.itemName}. Etkiler geri alındı.");
        }
    }
}
