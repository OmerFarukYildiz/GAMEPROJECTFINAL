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

    // Gizli değişkenler
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float horizontalInput;
    private bool isGrounded;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    void Start()
    {
        // Bileşenleri al
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Yatay hareket girdisi (-1 = sol, 0 = dur, 1 = sağ)
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Zemin kontrolü
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Coyote Time sayacı
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // Jump Buffer sayacı
        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        // Zıplama (Coyote Time + Jump Buffer ile)
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
        }

        // Zıplama tuşunu bırakınca hızı azalt (yüksekliği kontrol et)
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
        // Fiziği FixedUpdate'te uygula (daha smooth hareket için)
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    // Scene görünümünde zemin kontrol noktasını görsel olarak göster
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
