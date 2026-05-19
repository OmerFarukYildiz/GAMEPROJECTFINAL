using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Saldırı Ayarları")]
    public float attackRange = 2.5f;   // Vuruş menzili
    public int attackDamage = 2;       // Vuruş hasarı
    public KeyCode attackKey = KeyCode.Mouse0; // Vurma tuşu (Sol Tık)
    [Header("Menzilli Saldırı (Büyü)")]
    public GameObject projectilePrefab; // Atılacak mermi
    public Transform firePoint;         // Merminin çıkacağı nokta
    public KeyCode rangedAttackKey = KeyCode.Mouse1; // Sağ Tık
    public int rangedManaCost = 33;     // Büyü bedeli
    
    [Header("Mana Kazanımı")]
    public int manaPerHit = 15;         // Kılıçla vurunca kazanılacak mana

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Normal Kılıç Saldırısı
        if (Input.GetKeyDown(attackKey))
        {
            Attack();
        }

        // Menzilli Büyü Saldırısı
        if (Input.GetKeyDown(rangedAttackKey))
        {
            RangedAttack();
        }
    }

    void Attack()
    {
        // Karakterin baktığı yönü bul
        float facingDirection = (sr != null && sr.flipX) ? -1f : 1f;
        
        // Vuruş merkezini karakterin önüne kaydır (sadece önüne vurması için)
        Vector2 attackCenter = (Vector2)transform.position + new Vector2(facingDirection * (attackRange * 0.5f), 0f);
        
        // Çemberi küçülterek arkaya taşmasını engelle
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackCenter, attackRange * 0.6f);
        
        bool hitSomething = false;

        foreach (Collider2D hit in hits)
        {
            // Eğer objenin üzerinde BossController varsa (Tag'e gerek kalmadan bulur)
            BossController boss = hit.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(attackDamage);
                hitSomething = true;
            }

            // Eğer objenin üzerinde EnemyHealth varsa
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage); 
                hitSomething = true;
            }
        }

        if (hitSomething)
        {
            Debug.Log("KILIÇ SAVRULDU: Hedefe isabet etti!");
            
            // Mana Kazanımı
            if (PlayerMana.Instance != null)
            {
                PlayerMana.Instance.RestoreMana(manaPerHit);
            }
        }
        else
        {
            Debug.Log("KILIÇ SAVRULDU: Boşa gitti.");
        }
    }

    void RangedAttack()
    {
        // 1. Mermi prefabı veya çıkış noktası yoksa iptal
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Menzilli saldırı için Projectile Prefab veya Fire Point atanmamış!");
            return;
        }

        // 2. Mana Kontrolü
        if (PlayerMana.Instance != null)
        {
            if (!PlayerMana.Instance.UseMana(rangedManaCost))
            {
                // Yetersiz mana (Buraya "mana yok" sesi veya görseli eklenebilir)
                return;
            }
        }

        // 3. Atışı Gerçekleştir
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        
        PlayerProjectile projScript = proj.GetComponent<PlayerProjectile>();
        if (projScript != null)
        {
            // Karakterin baktığı yöne göre fırlat
            float direction = (sr != null && sr.flipX) ? -1f : 1f;
            projScript.Initialize(direction);
        }

        Debug.Log("BÜYÜ FIRLATILDI!");
    }

    // Unity Scene ekranında vuruş menzilini kırmızı bir daire olarak gösterir
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        float facingDirection = 1f;
        if (Application.isPlaying && sr != null) {
            facingDirection = sr.flipX ? -1f : 1f;
        } else {
            SpriteRenderer gizmoSr = GetComponent<SpriteRenderer>();
            if (gizmoSr != null && gizmoSr.flipX) facingDirection = -1f;
        }
        
        Vector2 attackCenter = (Vector2)transform.position + new Vector2(facingDirection * (attackRange * 0.5f), 0f);
        Gizmos.DrawWireSphere(attackCenter, attackRange * 0.6f);
    }
}
