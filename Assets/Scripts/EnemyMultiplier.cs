using UnityEngine;

public class EnemyMultiplier : MonoBehaviour
{
    [Header("Bölünme Ayarları")]
    public GameObject splitPrefab;
    public int splitCount = 2;
    public float splitForceY = 8f;
    public float splitForceX = 5f;

    public void TriggerSplit()
    {
        Debug.Log("TriggerSplit ÇAĞRILDI! Prefab: " + (splitPrefab != null ? splitPrefab.name : "YOK"));
        
        if (splitPrefab == null)
        {
            Debug.LogWarning("Bölünecek obje (splitPrefab) atanmamış!");
            return;
        }

        for (int i = 0; i < splitCount; i++)
        {
            float directionX = (i % 2 == 0) ? splitForceX : -splitForceX;
            float offsetX = (i % 2 == 0) ? 0.5f : -0.5f;

            // Orijinal objenin biraz üstünde ve sağ/sol yanlarında oluştur
            Vector3 spawnPos = transform.position + new Vector3(offsetX, 0.5f, 0f);
            GameObject splitObj = Instantiate(splitPrefab, spawnPos, Quaternion.identity);
            
            // ÇOK ÖNEMLİ: Yeni çıkan küçük slime'ları, büyük slime'ın %60'ı boyutuna zorla küçült.
            // Böylece devasa beyaz bloklar olarak görünmeyecekler!
            splitObj.transform.localScale = transform.localScale * 0.6f;
            
            Rigidbody2D rb = splitObj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Birini sağa, birini sola fırlat
                rb.linearVelocity = new Vector2(directionX, splitForceY);
            }
        }
    }
}
