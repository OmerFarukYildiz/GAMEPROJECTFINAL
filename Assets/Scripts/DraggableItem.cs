using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public ItemData itemData; // Bu objenin temsil ettiği item
    public Image image;

    public void Setup(ItemData item)
    {
        itemData = item;
        image.sprite = item.itemIcon;
        image.enabled = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        
        // Sürüklenirken her şeyin üstünde görünmesi için
        transform.SetParent(transform.root); 
        transform.SetAsLastSibling();

        // Raycast'i kapat ki altındaki slotu algılayabilsin (OnDrop için)
        image.raycastTarget = false;
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null) cg.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null) cg.blocksRaycasts = true;

        // Geçersiz bir yere bırakılırsa kutusunun tam ortasına geri dönmesi için sıfırlıyoruz:
        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null) rt.anchoredPosition = Vector2.zero;
    }
}
