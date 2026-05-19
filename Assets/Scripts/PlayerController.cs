using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 8f;        // Yürüme hızı
    public float jumpForce = 16f;       // Zıplama gücü

    [Header("Zemin Kontrolü")]
    public Transform groundCheck;       // Yerden kontrol noktası
    public LayerMask groundLayer;       // Zemin katmanı
    public float groundCheckRadius = 0.2f;

    [Header("Coyote Time")]
    public float coyoteTime = 0.15f;    // Kenarda biraz kalınca da zıplayabilme
    public float jumpBufferTime = 0.1f; // Zıplamaya erken basınca da tepki

    [Header("Dash Ayarları")]
    public float dashSpeed = 20f;       // Dash hızı
    public float dashDuration = 0.2f;   // Dash süresi
    public float dashCooldown = 1f;     // Kaç saniyede bir dash atılabilir

    // Gizli değişkenler
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float horizontalInput;
    private bool isGrounded;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    // Yeni mekanikler için gizli değişkenler
    private bool canDoubleJump;
    private bool isDashing;
    private float dashCooldownTimer;
    private float originalGravity;

    [Header("Duvar Mekaniği (Wall Jump & Cling)")]
    public LayerMask wallLayer;             // Hangi duvarlara tırmanılabilir (ClimbableWall layerı)
    public float wallCheckDistance = 0.5f;  // Duvarı ne kadar ileriden kontrol edecek
    public float wallCheckRadius = 0.2f;    // Duvar kontrol dairesi yarıçapı
    public float wallSlideSpeed = 2f;       // Tutunduktan sonraki kısa kayma hızı
    public float wallSlideDuration = 0.15f; // Ne kadar süre kayıp duracağı
    public KeyCode wallClingKey = KeyCode.Mouse0; // Tutunma tuşu (Sol Tık)

    private bool isTouchingWall;
    private bool isWallClinging;
    private float wallSlideTimer;
    private int wallDirection;

    void Start()
    {
        // Bileşenleri al
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        originalGravity = rb.gravityScale; // Fiziği bozmamak için orijinal yerçekimini kaydet
        
        // Duvara doğru koşarken sürtünmeden dolayı havada asılı kalmayı (takılmayı) engellemek için:
        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null && coll.sharedMaterial == null)
        {
            PhysicsMaterial2D noFrictionMat = new PhysicsMaterial2D("NoFriction");
            noFrictionMat.friction = 0f;
            noFrictionMat.bounciness = 0f;
            coll.sharedMaterial = noFrictionMat;
        }
    }

    void Update()
    {
        // Eğer dash atıyorsak başka bir girdi alma (havada asılı kalsın)
        if (isDashing) return;

        // Yatay hareket girdisi (-1 = sol, 0 = dur, 1 = sağ)
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Zemin kontrolü
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Duvar kontrolü (baktığı yöne göre)
        wallDirection = sr.flipX ? -1 : 1;
        Vector2 wallCheckPos = (Vector2)transform.position + new Vector2(wallDirection * wallCheckDistance, 0f);
        isTouchingWall = Physics2D.OverlapCircle(wallCheckPos, wallCheckRadius, wallLayer);

        // Duvara Tutunma Başlangıcı
        if (!isGrounded && isTouchingWall && Input.GetKeyDown(wallClingKey))
        {
            isWallClinging = true;
            wallSlideTimer = wallSlideDuration;
        }

        // Duvardan kopma (Yere değdiğinde veya duvardan uzaklaştığında)
        if (isGrounded || (isWallClinging && !isTouchingWall))
        {
            isWallClinging = false;
            rb.gravityScale = originalGravity;
        }

        if (isWallClinging)
        {
            // Duvarda Zıplama
            if (Input.GetButtonDown("Jump"))
            {
                isWallClinging = false;
                rb.gravityScale = originalGravity;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                canDoubleJump = true; // Duvar zıplaması sonrası havada double jump hakkı ver
                return; // Bu frame için diğer jump kontrollerini atla
            }

            // Duvarda kayma ve asılı kalma mekaniği
            if (wallSlideTimer > 0)
            {
                rb.linearVelocity = new Vector2(0f, -wallSlideSpeed);
                wallSlideTimer -= Time.deltaTime;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
            rb.gravityScale = 0f;

            // Asılıyken dash atmasını veya normal yürümesini engellemek için geri kalan kontrolleri atla
            return; 
        }
        else
        {
            // Duvara tutunmuyorsak yerçekimi normal olmalı
            rb.gravityScale = originalGravity;
        }

        // Coyote Time ve Double Jump sıfırlaması
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            canDoubleJump = true; // Yere değince double jump hakkını geri ver
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump Buffer sayacı
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Dash Cooldown sayacı
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Dash Tuşu Kontrolü (Q)
        if (Input.GetKeyDown(KeyCode.Q) && dashCooldownTimer <= 0f)
        {
            StartCoroutine(DashRoutine());
        }

        // Zıplama (Coyote Time + Jump Buffer ile veya Double Jump)
        if (jumpBufferCounter > 0f)
        {
            if (coyoteTimeCounter > 0f)
            {
                // Normal Zıplama
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpBufferCounter = 0f;
            }
            else if (canDoubleJump)
            {
                // Havada Double Jump
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Havada hızı sıfırlayıp zıplat
                jumpBufferCounter = 0f;
                canDoubleJump = false; // Hakkını kullan
            }
        }

        // Zıplama tuşunu bırakınca hızı azalt (kısa basma - uzun basma kontrolü)
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        // Karakter yönü (sağa gidince sprite'ı çevirme)
        if (horizontalInput > 0)
            sr.flipX = false;
        else if (horizontalInput < 0)
            sr.flipX = true;
    }

    void FixedUpdate()
    {
        if (isDashing) return; // Dash atıyorsak normal hareketi durdur
        if (isWallClinging) return; // Duvara tutunurken yatay hareketi (sağ/sol) fiziksel olarak durdur

        // Fiziği FixedUpdate'te uygula (daha smooth hareket için)
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private System.Collections.IEnumerator DashRoutine()
    {
        isDashing = true;
        
        // Havada dash atarken yerçekiminden etkilenmemesi için sıfırla
        rb.gravityScale = 0f; 

        // Baktığı yöne göre dash at (Eğer karakter solaysa -1, sağaysa 1)
        float dashDirection = sr.flipX ? -1f : 1f;

        // Anlık yüksek hız ver
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        // Dash süresi kadar bekle
        yield return new WaitForSeconds(dashDuration);

        // Yerçekimini geri getir ve dash'i bitir
        rb.gravityScale = originalGravity;
        isDashing = false;

        // Dash cooldown'ı başlat
        dashCooldownTimer = dashCooldown;
    }

    // Scene görünümünde zemin kontrol noktasını görsel olarak göster
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Duvar kontrolünü göster (varsayılan olarak sağ tarafı gösterelim veya sprite'a göre)
        SpriteRenderer gizmoSr = GetComponent<SpriteRenderer>();
        int dir = 1;
        if (gizmoSr != null && gizmoSr.flipX) dir = -1;

        Vector2 wallCheckPos = (Vector2)transform.position + new Vector2(dir * wallCheckDistance, 0f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(wallCheckPos, wallCheckRadius);
    }
}
