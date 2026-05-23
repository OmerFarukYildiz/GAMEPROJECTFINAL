using UnityEngine;

public class EnemyRanged : MonoBehaviour
{
    [Header("Menzilli Saldırı Ayarları")]
    public float detectionRange = 7f;
    public float retreatRange = 3f; // Bu mesafeden daha yakına gelirse kaçar
    public float moveSpeed = 3f;
    public float fireRate = 2f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float nextFireTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Yönünü oyuncuya çevir
        sr.flipX = (player.position.x < transform.position.x);

        if (distance < detectionRange && distance > retreatRange)
        {
            // Ateş etme menzilinde, dur ve ateş et
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
        else if (distance <= retreatRange)
        {
            // Oyuncu çok yaklaştı, geri kaç!
            float retreatDirection = (transform.position.x > player.position.x) ? 1 : -1;
            rb.linearVelocity = new Vector2(retreatDirection * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            // Menzil dışı, dur.
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            
            // Merminin yönünü ayarla
            Vector2 shootDir = (player.position - firePoint.position).normalized;
            EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
            if (ep != null)
            {
                ep.SetDirection(shootDir);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // Saldırı sınırı
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, retreatRange); // Kaçma sınırı
    }
}
