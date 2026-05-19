using UnityEngine;

public class TalismanPickup : MonoBehaviour
{
    [Header("Tılsım Verisi")]
    public ItemData talismanData; // ScriptableObject'ten atayacağız

    [Header("Toplama Efekti")]
    public GameObject collectEffectPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (talismanData != null && talismanData.itemType == ItemType.Talisman)
            {
                // Depoya eklemeyi dene
                bool added = false;
                if (InventoryManager.Instance != null)
                {
                    added = InventoryManager.Instance.AddItemToStash(talismanData);
                }

                if (added)
                {
                    // Efekt
                    if (collectEffectPrefab != null)
                    {
                        Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
                    }

                    Debug.Log(talismanData.itemName + " envantere eklendi!");

                    // Kalıcı Kayıt için (Eğer GameManager'a eklenecekse, sonradan buraya eklenebilir)

                    // Objeyi yok et
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Envanter dolu!");
                }
            }
        }
    }
}
