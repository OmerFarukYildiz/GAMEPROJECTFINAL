using UnityEngine;

public class EnemyBoomerangThrower : MonoBehaviour
{
    [Header("Bumerang Atan Ayarları")]
    public float detectionRange = 8f;
    public GameObject boomerangPrefab;
    public Transform throwPoint;
    public float throwCooldown = 2f; // Bumerangı tuttuktan sonraki bekleme süresi
    
    private Transform player;
    private SpriteRenderer sr;
    
    private bool hasBoomerang = true; // Elinde bumerang var mı?
    private float nextThrowTime = 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Yönünü oyuncuya çevir
        if (player.position.x < transform.position.x)
            sr.flipX = true; // Sola bak
        else
            sr.flipX = false; // Sağa bak

        if (distance < detectionRange)
        {
            // Elinde bumerang varsa ve bekleme süresi dolduysa at
            if (hasBoomerang && Time.time >= nextThrowTime)
            {
                ThrowBoomerang();
            }
        }
    }

    void ThrowBoomerang()
    {
        if (boomerangPrefab == null)
        {
            Debug.LogWarning("Bumerang Prefab atanmamış!");
            return;
        }

        hasBoomerang = false;

        // Throw point yoksa kendinden fırlat
        Vector3 spawnPos = throwPoint != null ? throwPoint.position : transform.position;

        GameObject boomerang = Instantiate(boomerangPrefab, spawnPos, Quaternion.identity);
        EnemyBoomerang boomScript = boomerang.GetComponent<EnemyBoomerang>();
        
        if (boomScript != null)
        {
            Vector2 throwDir = (player.position - spawnPos).normalized;
            boomScript.Initialize(throwDir, transform);
        }
    }

    public void CatchBoomerang()
    {
        hasBoomerang = true;
        nextThrowTime = Time.time + throwCooldown; // Tuttuktan sonra 2 saniye bekle
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
