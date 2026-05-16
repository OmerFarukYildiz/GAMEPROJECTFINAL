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
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Faz 1 Ayarları")]
    public float phase1Speed = 3f;
    public float shootInterval = 3f;      // Kaç saniyede bir ateş eder

    [Header("Faz 2 Ayarları")]
    public float phase2Speed = 6f;
    public float phase2ShootInterval = 1.5f;
    public int phase2Threshold = 25;      // Bu cana düşünce Faz 2'ye geçer

    [Header("Ground Slam")]
    public float groundSlamForce = 18f;
    public float groundSlamCooldown = 5f;
    public int groundSlamDamage = 6;

    [Header("Ödül & Geçiş")]
    public GameObject colorCrystalPrefab; // Kristal prefabı
    public GameObject wallToDestroy;      // Boss ölünce yok olacak duvar
    public GameObject portalToEnable;     // Boss ölünce açılacak kapı

    [Header("Arena Sınırları (Opsiyonel)")]
    public bool useArenaLimits = false;
    public float minX;
    public float maxX;

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
    private Vector3 startPosition;
    private Color originalColor;
    private Coroutine resetCoroutine;
    private bool isPlayerOutside = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        startPosition = transform.position;
        originalColor = sr.color; // Orijinal rengi kaydet

        if (player == null)
        {
            GameObject pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null) player = pObj.transform;
        }
    }

    void Update()
    {
        if (!bossActivated || currentState == BossState.Death) return;

        // isGrounded artık OnCollisionStay2D ile daha güvenilir bir şekilde kontrol edilecek (Eğer manuel ayar yapılmadıysa)
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.5f, groundLayer);
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

        // Eğer arena sınırları aktifse, boss'un pozisyonunu sınırla
        if (useArenaLimits)
        {
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
            transform.position = clampedPosition;
        }
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

        // Ground Slam (isGrounded zorunluluğunu esnettik veya güvenli hale getirdik)
        if (groundSlamTimer >= groundSlamCooldown && !isSlamming)
        {
            // Eğer isGrounded bir şekilde buga girdiyse bile boss'un zıplamasını sağlamak için ekstra güvenlik
            if (isGrounded || rb.linearVelocity.y == 0)
            {
                StartCoroutine(GroundSlam());
                groundSlamTimer = 0f;
            }
        }
    }

    // ── HAREKET HESAPLAMA ─────────────────────────────────────
    void CalculateMovement(float speed)
    {
        float targetX;
        
        if (isPlayerOutside)
        {
            // Oyuncu arenadan çıktıysa başladığı noktaya (odanın ortasına) dön
            targetX = startPosition.x;
        }
        else
        {
            // Oyuncu arenadaysa onu takip et
            targetX = player.position.x;
        }

        float distanceX = targetX - transform.position.x;
        
        // Tolerans: Dışarıdaysa tam noktaya gitsin (0.5), oyuncuyu kovalıyorsa biraz uzakta dursun (2.5) ki kapıya sıkışmasın
        float stopDistance = isPlayerOutside ? 0.5f : 2.5f;

        if (Mathf.Abs(distanceX) < stopDistance)
        {
            moveDirection = 0f;
        }
        else
        {
            moveDirection = Mathf.Sign(distanceX); // 1 veya -1
        }

        currentSpeed = speed;

        // Yönümüz varsa (ve oyuncuyu kovalıyorsak) ateş edeceğimiz tarafı ayarla
        if (moveDirection != 0f && !isPlayerOutside)
        {
            sr.flipX = (moveDirection < 0);
            
            if (firePoint != null)
            {
                Vector3 fpLocalPos = firePoint.localPosition;
                fpLocalPos.x = Mathf.Abs(fpLocalPos.x) * (sr.flipX ? -1f : 1f);
                firePoint.localPosition = fpLocalPos;
            }
        }
        else if (!isPlayerOutside)
        {
            // Eğer duruyorsak (oyuncuya yeterince yakınsak) yine oyuncuya doğru dönük kalalım
            float lookDist = player.position.x - transform.position.x;
            sr.flipX = (lookDist < 0);
            
            if (firePoint != null)
            {
                Vector3 fpLocalPos = firePoint.localPosition;
                fpLocalPos.x = Mathf.Abs(fpLocalPos.x) * (sr.flipX ? -1f : 1f);
                firePoint.localPosition = fpLocalPos;
            }
        }
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
        // Önce zıpla (Yukarı daha fazla güç verelim)
        rb.linearVelocity = new Vector2(0f, groundSlamForce);

        // Havaya çıkmasını bekle
        yield return new WaitForSeconds(0.6f);

        // Hızla aşağı düş (Eğer isGrounded tetiklenmezse diye süre sınırı ekledik)
        float fallTimer = 0f;
        while (!isGrounded && fallTimer < 1.5f)
        {
            rb.linearVelocity = new Vector2(0f, -groundSlamForce * 1.5f);
            fallTimer += Time.deltaTime;
            yield return null;
        }

        // Yere çarpınca çevre düşmanlara/oyuncuya hasar (alan hasarı)
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 4f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerHealth>()?.TakeDamage(groundSlamDamage);

                // Knockback
                Rigidbody2D prb = hit.GetComponent<Rigidbody2D>();
                if (prb != null)
                {
                    Vector2 dir = (hit.transform.position - transform.position).normalized;
                    prb.AddForce(dir * 10f, ForceMode2D.Impulse);
                }
            }
        }

        // Kamera Sarsıntısı (Vuruş hissi)
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(0.3f, 0.4f);
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

        // Duvarı kaldır ve portalı aç
        if (wallToDestroy != null) wallToDestroy.SetActive(false);
        if (portalToEnable != null) portalToEnable.SetActive(true);

        Destroy(gameObject);
    }

    // ── SIFIRLAMA (ARENADAN ÇIKINCA) ────────────────────────
    public void StartResetTimer()
    {
        isPlayerOutside = true;
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        resetCoroutine = StartCoroutine(ResetTimerRoutine());
    }

    private IEnumerator ResetTimerRoutine()
    {
        Debug.Log("Oyuncu odadan çıktı. Boss 10 saniye içinde sıfırlanacak...");
        yield return new WaitForSeconds(10f);
        ResetBoss();
    }

    public void CancelResetTimer()
    {
        isPlayerOutside = false;
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
            Debug.Log("Oyuncu odaya geri döndü, boss sıfırlaması iptal edildi.");
        }
    }

    public void ResetBoss()
    {
        isPlayerOutside = false;
        bossActivated = false;
        currentState = BossState.Idle;
        currentHealth = maxHealth;
        transform.position = startPosition;
        sr.color = originalColor; // Başlangıç rengine dön
        isSlamming = false;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Boss tamamen sıfırlandı!");
    }

    // ── AKTİFLEŞTİRME ────────────────────────────────────────
    public void ActivateBoss()
    {
        if (bossActivated) return; // Zaten aktifse fazı sıfırlama

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

    // Boss'un isGrounded durumunu garanti altına almak için çarpışma kontrolü
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Temas edilen obje groundLayer içinde mi kontrol et
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f) // Üstüne bastığı bir şey
                {
                    isGrounded = true;
                    return;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
        }
    }
}
