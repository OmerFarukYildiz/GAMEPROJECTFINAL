using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [Header("Ayarlar")]
    public float speed = 15f;
    public int damage = 4; // Uzaktan atış, normal kılıçtan (2) daha güçlü olabilir
    public float lifeTime = 2f;
    public GameObject hitEffect; // Çarptığında çıkacak ufak parlama (opsiyonel)

    private Rigidbody2D rb;

    void Start()
    {
        // Belli bir süre sonra (hiçbir şeye çarpmazsa) yok ol
        Destroy(gameObject, lifeTime);
    }

    public void Initialize(float direction)
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f; // Dümdüz uçması için yerçekimini kapat
            rb.linearVelocity = new Vector2(direction * speed, 0f);
            
            // Eğer sola doğru gidiyorsa sprite'ı döndür
            if (direction < 0)
            {
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Oyuncuya çarpmayalım (Tag yerine Component kontrolü yapıyoruz ki hata vermesin)
        if (hitInfo.GetComponent<PlayerController>() != null || hitInfo.GetComponent<PlayerHealth>() != null) return;

        EnemyHealth enemy = hitInfo.GetComponent<EnemyHealth>();
        BossController boss = hitInfo.GetComponent<BossController>();

        // Eğer çarptığımız şey düşman veya boss DEĞİLSE ve bir sensörse (örn: Bonfire alanı, Kamera alanı) içinden geçip gitsin
        if (enemy == null && boss == null && hitInfo.isTrigger) return;

        bool hitEntity = false;

        if (enemy != null)
        {
            enemy.TakeDamage(damage, transform);
            hitEntity = true;
        }

        if (boss != null)
        {
            boss.TakeDamage(damage);
            hitEntity = true;
        }

        // Eğer bir düşmana, bossa, zemine veya duvara çarptıysa yok ol
        if (hitEntity || hitInfo.gameObject.layer == LayerMask.NameToLayer("Ground") || hitInfo.gameObject.layer == LayerMask.NameToLayer("ClimbableWall"))
        {
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
