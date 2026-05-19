using UnityEngine;

public enum ItemType
{
    Crystal,
    Talisman
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemID;
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;

    [Header("Tılsım Özellikleri (Sadece Talisman için)")]
    [Tooltip("Eğer bu bir tılsımsa ve can veriyorsa buraya değer girin (Örn: 1)")]
    public int healthBonus = 0;
    
    [Tooltip("Tılsım için mana bonusu")]
    public int manaBonus = 0;

    [Tooltip("Tılsım için hareket hızı bonusu")]
    public float speedBonus = 0f;
}
