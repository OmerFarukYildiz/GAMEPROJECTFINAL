using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("UI Referansları")]
    public GameObject inventoryPanel; // Envanteri aç/kapat için ana panel
    public GameObject draggableItemPrefab; // Sürüklenen obje prefabı

    [Header("Slot Referansları")]
    public List<InventorySlot> crystalStashSlots;
    public List<InventorySlot> talismanStashSlots;
    public InventorySlot crystalEquipSlot;
    public InventorySlot talismanEquipSlot;

    private CanvasGroup canvasGroup;
    private bool isOpen = false;

    void Awake()
    {
        Instance = this;
        
        // Eğer InventoryPanel kendisine atandıysa kapanmaması için CanvasGroup kullanıyoruz
        if (inventoryPanel != null)
        {
            canvasGroup = inventoryPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null) 
            {
                canvasGroup = inventoryPanel.AddComponent<CanvasGroup>();
            }
        }
        
        HideInventory(); // Başlangıçta gizle
    }

    void Start()
    {
        if(InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += UpdateUI;
            UpdateUI(); // Başlangıçta doldur
        }
    }

    void Update()
    {
        // I veya Tab tuşu ile envanteri aç kapa
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            ShowInventory();
        }
        else
        {
            HideInventory();
        }
    }

    private void ShowInventory()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        
        Time.timeScale = 0f; // Oyunu durdur
        UpdateUI(); // Açıldığında güncelle
    }

    private void HideInventory()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        
        Time.timeScale = 1f; // Oyunu devam ettir
    }

    public void UpdateUI()
    {
        if(InventoryManager.Instance == null) return;

        // Bütün slotları temizle
        ClearAllSlots();

        // Crystal Stash Doldur
        for (int i = 0; i < InventoryManager.Instance.crystalStash.Count; i++)
        {
            if (i < crystalStashSlots.Count)
                SpawnDraggableItem(InventoryManager.Instance.crystalStash[i], crystalStashSlots[i].transform);
        }

        // Talisman Stash Doldur
        for (int i = 0; i < InventoryManager.Instance.talismanStash.Count; i++)
        {
            if (i < talismanStashSlots.Count)
                SpawnDraggableItem(InventoryManager.Instance.talismanStash[i], talismanStashSlots[i].transform);
        }

        // Equip Slotlarını Doldur
        if (InventoryManager.Instance.equippedCrystal != null && crystalEquipSlot != null)
        {
            SpawnDraggableItem(InventoryManager.Instance.equippedCrystal, crystalEquipSlot.transform);
        }

        if (InventoryManager.Instance.equippedTalisman != null && talismanEquipSlot != null)
        {
            SpawnDraggableItem(InventoryManager.Instance.equippedTalisman, talismanEquipSlot.transform);
        }
    }

    private void ClearAllSlots()
    {
        // Kristal depo slotlarını temizle
        foreach (var slot in crystalStashSlots)
        {
            foreach (Transform child in slot.transform)
                Destroy(child.gameObject);
        }
        
        // Tılsım depo slotlarını temizle
        foreach (var slot in talismanStashSlots)
        {
            foreach (Transform child in slot.transform)
                Destroy(child.gameObject);
        }

        // Equip slotlarını temizle
        if(crystalEquipSlot != null)
        {
            foreach (Transform child in crystalEquipSlot.transform) Destroy(child.gameObject);
        }
        if(talismanEquipSlot != null)
        {
            foreach (Transform child in talismanEquipSlot.transform) Destroy(child.gameObject);
        }
    }

    private void SpawnDraggableItem(ItemData item, Transform parentSlot)
    {
        GameObject newItem = Instantiate(draggableItemPrefab, parentSlot);
        
        // Obje oluşturulurken kaymaması için tam ortaya (0,0) sabitliyoruz
        RectTransform rt = newItem.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
        }

        DraggableItem dragScript = newItem.GetComponent<DraggableItem>();
        if (dragScript != null)
        {
            dragScript.Setup(item);
        }
    }

    void OnDestroy()
    {
        if(InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= UpdateUI;
        }
    }
}
