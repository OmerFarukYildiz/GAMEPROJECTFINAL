using UnityEngine;
using UnityEngine.EventSystems;

public enum SlotType
{
    CrystalStash,
    TalismanStash,
    CrystalEquip,
    TalismanEquip
}

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public SlotType slotType;
    public int slotIndex = -1; // Stash listesindeki yeri bilebilmek için

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;
        if (droppedObj != null)
        {
            DraggableItem draggableItem = droppedObj.GetComponent<DraggableItem>();
            if (draggableItem != null)
            {
                ItemData item = draggableItem.itemData;

                bool success = false;

                // Takma İşlemi (Equip)
                if (slotType == SlotType.CrystalEquip && item.itemType == ItemType.Crystal)
                {
                    InventoryManager.Instance.EquipItem(item);
                    success = true;
                }
                else if (slotType == SlotType.TalismanEquip && item.itemType == ItemType.Talisman)
                {
                    InventoryManager.Instance.EquipItem(item);
                    success = true;
                }
                // Çıkarma İşlemi (Depoya atma)
                else if (slotType == SlotType.CrystalStash && item.itemType == ItemType.Crystal)
                {
                    InventoryManager.Instance.UnequipItem(ItemType.Crystal);
                    success = true;
                }
                else if (slotType == SlotType.TalismanStash && item.itemType == ItemType.Talisman)
                {
                    InventoryManager.Instance.UnequipItem(ItemType.Talisman);
                    success = true;
                }
                else
                {
                    Debug.Log("Yanlış slot tipi!");
                }

                // Eğer işlem başarılıysa, sürüklediğimiz geçici (havada asılı kalan) objeyi yok et.
                // Çünkü sistem UpdateUI ile o objenin yenisini hedef kutunun içine mükemmel bir şekilde oluşturdu bile.
                if (success)
                {
                    Destroy(droppedObj);
                }
            }
        }
    }
}
