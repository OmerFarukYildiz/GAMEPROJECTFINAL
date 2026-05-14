using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Noktaları")]
    public Transform pointA;         // Sol nokta
    public Transform pointB;         // Sağ nokta
    public float speed = 3f;         // Yürüme hızı

    private Transform currentTarget;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        currentTarget = pointB; // Başlangıçta B noktasına git
    }

    void FixedUpdate()
    {
        // Hangi yöne gideceğimizi bul (sadece X ekseninde)
        float direction = Mathf.Sign(currentTarget.position.x - transform.position.x);
        
        // Sadece yatay hızı değiştir, dikey hızı (yerçekimi) koru
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        // Hedefe ulaştıysa hedefi değiştir
        if (Mathf.Abs(transform.position.x - currentTarget.position.x) < 0.2f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }

        // Yönüne göre sprite'ı çevir
        sr.flipX = (currentTarget == pointA);
    }
}
