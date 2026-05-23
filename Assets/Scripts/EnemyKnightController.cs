using UnityEngine;

public class EnemyKnightController : MonoBehaviour
{
    [Header("Şövalye Hareket Ayarları")]
    public float walkSpeed = 1.5f;
    public float detectionRange = 6f;
    
    [Header("Saldırı Ayarları")]
    public float attackRange = 1.5f;
    public int attackDamage = 3;
    public float attackCooldown = 2f;
    public float turnCooldown = 1.5f; // Yönünü çevirme gecikmesi (hantal hissettirir)

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    
    private float nextAttackTime = 0f;
    private float timeFacingWrongWay = 0f;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            // Yönünü oyuncuya çevir (Fakat hantal, anında dönemez)
            float direction = (player.position.x > transform.position.x) ? 1f : -1f;
            bool wantsToFaceLeft = (direction < 0);
            
            if (sr.flipX != wantsToFaceLeft)
            {
                timeFacingWrongWay += Time.deltaTime;
                // Şövalye 1.5 saniye boyunca oyuncuya ters bakarsa ancak o zaman dönebilir
                if (timeFacingWrongWay >= turnCooldown)
                {
                    sr.flipX = wantsToFaceLeft;
                    timeFacingWrongWay = 0f;
                }
            }
            else
            {
                timeFacingWrongWay = 0f;
            }

            if (distance <= attackRange)
            {
                // Yaklaştıysa dur ve vur
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                if (Time.time >= nextAttackTime)
                {
                    PerformAttack();
                }
            }
            else
            {
                // Menzilde ama uzakta, üstüne yürü
                rb.linearVelocity = new Vector2(direction * walkSpeed, rb.linearVelocity.y);
            }
        }
        else
        {
            // Menzil dışıysa dur
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void PerformAttack()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;
        
        // Vururken hareketi durdur
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        // Vurma simülasyonu: 0.5 sn bekleyip hasar verir, tam 2 saniye boyunca kilitlenip hareketsiz kalır
        Invoke("DealDamage", 0.5f);
        Invoke("ResetAttack", 2.0f);
        
        Debug.Log("Şövalye kılıcını savuruyor...");
    }

    void DealDamage()
    {
        if (player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        
        // Eğer kılıç savurma anında oyuncu hala yakınındaysa ve şövalyenin önündeyse hasar ver
        float directionToPlayer = (player.position.x > transform.position.x) ? 1f : -1f;
        float currentFacing = sr.flipX ? -1f : 1f; // flipX true ise sola (-1) bakıyor

        if (distance <= attackRange + 0.5f && currentFacing == directionToPlayer)
        {
            player.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
            Debug.Log("Şövalye oyuncuya hasar verdi!");
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
