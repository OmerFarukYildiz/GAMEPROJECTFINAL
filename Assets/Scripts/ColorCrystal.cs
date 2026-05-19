using UnityEngine;

public class ColorCrystal : MonoBehaviour
{
    [Header("Kayıt Ayarları")]
    public string crystalID = "Crystal_1"; // GameManager'da kalıcı olarak silinmesi için kimlik
    
    [Header("Envanter Verisi")]
    public ItemData itemData; // Envantere eklenecek veri (ScriptableObject)

    [Header("Görsel Ayarlar")]
    public Color crystalColor = Color.cyan;  // Kristalin rengi
    public float bobSpeed = 2f;              // Yukarı-aşağı sallanma hızı
    public float bobAmount = 0.3f;           // Sallanma miktarı
    public float rotationSpeed = 90f;        // Dönme hızı (derece/sn)

    [Header("Toplama")]
    public GameObject collectEffectPrefab;  // Toplama particle efekti

    private Vector3 startPosition;
    private SpriteRenderer sr;

    void Start()
    {
        // Daha önce toplandıysa direkt yok ol
        if (GameManager.Instance != null && GameManager.Instance.IsCrystalCollected(crystalID))
        {
            Destroy(gameObject);
            return;
        }

        startPosition = transform.position;
        sr = GetComponent<SpriteRenderer>();

        // Rengi uygula
        if (sr != null)
            sr.color = crystalColor;
    }

    void Update()
    {
        // Yukarı-aşağı sallanma (Bob efekti)
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Dönme efekti
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    void Collect()
    {
        // Önce envantere eklemeyi dene
        bool addedToInventory = true;
        if (itemData != null && InventoryManager.Instance != null)
        {
            addedToInventory = InventoryManager.Instance.AddItemToStash(itemData);
        }

        // Eğer eklendiyse veya ItemData ayarlanmadıysa devam et (Geriye uyumluluk için)
        if (addedToInventory || itemData == null)
        {
            // Particle efekti oluştur (varsa)
            if (collectEffectPrefab != null)
            {
                Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
            }

            // ColorManager'ı bildir (Dünya renklensin)
            if (ColorManager.Instance != null)
            {
                ColorManager.Instance.CrystalCollected(crystalID);
            }

            // Kalıcı olarak toplandığını kaydet
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegisterCollectedCrystal(crystalID);
            }

            // Kristali yok et
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Envanter dolu, Kristal alınamadı!");
        }
    }
}
