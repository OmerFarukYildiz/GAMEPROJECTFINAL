using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [Header("Takip Ayarları")]
    public Transform player;
    public float chaseSpeed = 5f;
    public float detectionRange = 6f;   // Görüş menzili
    public float attackRange = 0.8f;    // Saldırı menzili
    public GameObject exclamationPrefab; // (!) Ünlem İşareti Prefabı
    public float exclamationOffset = 2.5f; // Ünlemin yüksekliğini buradan ayarlayabilirsiniz
    
    private bool isChasing = false;
    private GameObject currentExclamation; // Ünlemi takip etmek için

    [Header("Patrol (Takip yokken)")]
    public float patrolSpeed = 2f;
    private Vector2 patrolDirection = Vector2.right;
    private float patrolTimer = 0f;
    public float patrolTurnTime = 2f;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private float distanceToPlayer;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // Player'ı otomatik bul
        if (player == null)
        {
            GameObject pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null)
                player = pObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;
        
        // Ünlem varsa her karede tam olarak düşmanın üstünde tut (Parenting sorunlarını çözer)
        if (currentExclamation != null)
        {
            Vector3 targetPos = transform.position + Vector3.up * exclamationOffset;
            targetPos.z = -2f;
            currentExclamation.transform.position = targetPos;
        }

        // Mesafe hesabını ve yön dönme işlemini (flipX) Update'te yapıyoruz
        distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (!isChasing)
            {
                isChasing = true;
                ShowExclamation();
            }
            Vector2 direction = (player.position - transform.position).normalized;
            sr.flipX = (direction.x < 0);
        }
        else
        {
            isChasing = false;
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolTurnTime)
            {
                patrolDirection = -patrolDirection;
                patrolTimer = 0f;
            }
            sr.flipX = (patrolDirection.x < 0);
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Fizik hareketlerini FixedUpdate'te yapıyoruz ki titreme olmasın
        if (distanceToPlayer <= detectionRange)
        {
            // CHASE MODU: Oyuncuya doğru koş
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);
        }
        else
        {
            // PATROL MODU: İleri geri yürü
            rb.linearVelocity = new Vector2(patrolDirection.x * patrolSpeed, rb.linearVelocity.y);
        }
    }

    // Scene'de görüş menzilininin görsel gösterimi
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void ShowExclamation()
    {
        if (exclamationPrefab != null)
        {
            Debug.Log("Asker oyuncuyu gördü, ünlem çıkıyor!");
            
            // Askerin parent'ı olmadan, tamamen bağımsız olarak oluştur
            Vector3 spawnPos = transform.position + Vector3.up * exclamationOffset;
            spawnPos.z = -2f;
            currentExclamation = Instantiate(exclamationPrefab, spawnPos, Quaternion.identity);
            
            Destroy(currentExclamation, 1f); // 1 saniye sonra ünlemi sil
        }
    }
}
