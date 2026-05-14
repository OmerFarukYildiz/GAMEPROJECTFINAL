using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    // ── BOSS DURUMLARI ──────────────────────────────────────────
    public enum BossState { Idle, Phase1, Phase2, Death }
    public BossState currentState = BossState.Idle;

    [Header("Referanslar")]
    public Transform player;
    public GameObject projectilePrefab;   // Mermi prefabı
    public Transform firePoint;           // Merminin çıktığı nokta

    [Header("Can Ayarları")]
    public int maxHealth = 20;
    private int currentHealth;

    [Header("Faz 1 Ayarları")]
    public float phase1Speed = 3f;
    public float shootInterval = 3f;      // Kaç saniyede bir ateş eder

    [Header("Faz 2 Ayarları")]
    public float phase2Speed = 6f;
    public float phase2ShootInterval = 1.5f;
    public int phase2Threshold = 10;      // Bu cana düşünce Faz 2'ye geçer

    [Header("Ground Slam")]
    public float groundSlamForce = 18f;
    public float groundSlamCooldown = 5f;

    [Header("Ödül")]
    public GameObject colorCrystalPrefab; // Kristal prefabı

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool bossActivated = false;
    private float shootTimer = 0f;
    private float groundSlamTimer = 0f;
    private bool isGrounded = true;
    private float moveDirection = 0f;
    private float currentSpeed = 0f;
    private bool isSlamming = false;

    public LayerMask groundLayer;
    public Transform groundCheck;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        if (player == null)
        {
            GameObject pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null) player = pObj.transform;
        }
    }

    void Update()
    {
        if (!bossActivated || currentState == BossState.Death) return;

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);
        }

        // Timers
        shootTimer += Time.deltaTime;
        groundSlamTimer += Time.deltaTime;

        // Faz 2'ye geçiş kontrolü
        if (currentHealth <= phase2Threshold && currentState == BossState.Phase1)
        {
            EnterPhase2();
        }

        switch (currentState)
        {
            case BossState.Phase1: Phase1Behavior(); break;
            case BossState.Phase2: Phase2Behavior(); break;
        }
    }

    void FixedUpdate()
    {
        if (!bossActivated || currentState == BossState.Death || isSlamming || player == null) return;
        
        // Fizik hareketini burada uygula (titremeyi önler)
        rb.linearVelocity = new Vector2(moveDirection * currentSpeed, rb.linearVelocity.y);
    }

    // ── FAZ 1: Normal Davranış ────────────────────────────────
    void Phase1Behavior()
    {
        if (player == null) return;
        CalculateMovement(phase1Speed);

        if (shootTimer >= shootInterval)
        {
            ShootProjectile();
            shootTimer = 0f;
        }
    }

    // ── FAZ 2: Agresif Davranış ──────────────────────────────
    void Phase2Behavior()
    {
        if (player == null) return;
        CalculateMovement(phase2Speed);

        if (shootTimer >= phase2ShootInterval)
        {
            ShootProjectile();
            ShootProjectile(30f);   // Açılı ikinci mermi
            ShootProjectile(-30f);  // Açılı üçüncü mermi
            shootTimer = 0f;
        }

        // Ground Slam
        if (groundSlamTimer >= groundSlamCooldown && isGrounded && !isSlamming)
        {
            StartCoroutine(GroundSlam());
            groundSlamTimer = 0f;
        }
    }

    // ── HAREKET HESAPLAMA ─────────────────────────────────────
    void CalculateMovement(float speed)
    {
        moveDirection = (player.position.x > transform.position.x) ? 1f : -1f;
        currentSpeed = speed;
        sr.flipX = (moveDirection < 0);
    }

    // ── ATEŞ ─────────────────────────────────────────────────
    void ShootProjectile(float angleOffset = 0f)
    {
        if (projectilePrefab == null || firePoint == null) return;

        Vector2 direction = (player.position - firePoint.position).normalized;

        // Açı uygula
        if (angleOffset != 0f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angleOffset;
            direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        }

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.GetComponent<Rigidbody2D>().linearVelocity = direction * 8f;
    }

    // ── GROUND SLAM ──────────────────────────────────────────
    IEnumerator GroundSlam()
    {
        isSlamming = true;
        // Önce zıpla
        rb.linearVelocity = new Vector2(0f, groundSlamForce);

        // Havaya çıkmasını bekle
        yield return new WaitForSeconds(0.5f);

        // Hızla aşağı düş
        while (!isGrounded)
        {
            rb.linearVelocity = new Vector2(0f, -groundSlamForce);
            yield return null;
        }

        // Yere çarpınca çevre düşmanlara/oyuncuya hasar (alan hasarı)
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 4f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerHealth>()?.TakeDamage(2);

                // Knockback
                Rigidbody2D prb = hit.GetComponent<Rigidbody2D>();
                if (prb != null)
                {
                    Vector2 dir = (hit.transform.position - transform.position).normalized;
                    prb.AddForce(dir * 10f, ForceMode2D.Impulse);
                }
            }
        }

        Debug.Log("GROUND SLAM!");
        yield return new WaitForSeconds(0.5f); // Kısa bir bekleme
        isSlamming = false;
    }

    // ── FAZ 2 GEÇİŞİ ─────────────────────────────────────────
    void EnterPhase2()
    {
        currentState = BossState.Phase2;
        sr.color = new Color(0.7f, 0f, 0f); // Daha koyu kırmızı
        Debug.Log("BOSS FAZ 2'YE GEÇTİ!");

        // Kısa bir duraklama
        StartCoroutine(Phase2EntrancePause());
    }

    IEnumerator Phase2EntrancePause()
    {
        isSlamming = true; // Hareketi durdurmak için
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1.5f);
        isSlamming = false;
    }

    // ── HASAR ALMA ────────────────────────────────────────────
    public void TakeDamage(int damage)
    {
        if (currentState == BossState.Death) return;

        currentHealth -= damage;
        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator HitFlash()
    {
        sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        sr.color = (currentState == BossState.Phase2) ? new Color(0.7f, 0f, 0f) : Color.red;
    }

    // ── BOSS ÖLÜMÜ ────────────────────────────────────────────
    void Die()
    {
        currentState = BossState.Death;
        rb.linearVelocity = Vector2.zero;
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        Debug.Log("BOSS ÖLDÜ!");

        for (int i = 0; i < 10; i++)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.15f);
        }

        // Kristal bırak
        if (colorCrystalPrefab != null)
        {
            Instantiate(colorCrystalPrefab, transform.position + Vector3.up, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    // ── AKTİFLEŞTİRME ────────────────────────────────────────
    public void ActivateBoss()
    {
        bossActivated = true;
        currentState = BossState.Phase1;
        Debug.Log("Boss savaşı başladı!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 4f); // Ground slam menzili
    }

    // ── UI İÇİN GETTER ──────────────────────────────────────────
    public int GetCurrentHealth() => currentHealth;
}
