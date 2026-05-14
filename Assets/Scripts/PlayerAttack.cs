using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Saldırı Ayarları")]
    public float attackRange = 2.5f;   // Vuruş menzili
    public int attackDamage = 5;       // Vuruş hasarı (Boss 20 can, 4 vuruşta ölür)
    public KeyCode attackKey = KeyCode.F; // Vurma tuşu (F)
    
    void Update()
    {
        // F tuşuna basıldığında
        if (Input.GetKeyDown(attackKey))
        {
            Attack();
        }
    }

    void Attack()
    {
        // Karakterin etrafındaki belirli bir alandaki tüm objeleri bul (Bölge saldırısı)
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        
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

            // Eğer objenin üzerinde EnemyHealth varsa (Tag'e gerek kalmadan bulur)
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                // Küçük düşmanlara 5 vurursak tek yerler, o yüzden onlara 2 veya 3 vuralım
                enemy.TakeDamage(2); 
                hitSomething = true;
            }
        }

        if (hitSomething)
        {
            Debug.Log("KILIÇ SAVRULDU: Hedefe isabet etti!");
        }
        else
        {
            Debug.Log("KILIÇ SAVRULDU: Boşa gitti.");
        }
    }

    // Unity Scene ekranında vuruş menzilini kırmızı bir daire olarak gösterir
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
