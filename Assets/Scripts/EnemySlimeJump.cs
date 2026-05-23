using UnityEngine;
using System.Collections;

public class EnemySlimeJump : MonoBehaviour
{
    [Header("Zıplama Ayarları")]
    public float jumpForceX = 3f;
    public float jumpForceY = 6f;
    public float jumpCooldown = 2f;
    public float detectionRange = 7f;
    public int damageAmount = 2; // Slime'ın oyuncuya vereceği hasar

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool canJump = true;
    private bool isGrounded = true;

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

        if (isGrounded && distance <= detectionRange)
        {
            if (player.position.x < transform.position.x)
                sr.flipX = true; // sola dön
            else
                sr.flipX = false; // sağa dön
        }

        // Oyuncu menzildeyse ve zıplama bekleme süresi dolmuşsa ve yerdeyse
        if (distance <= detectionRange && canJump && isGrounded)
        {
            JumpTowardsPlayer();
        }
    }

    void JumpTowardsPlayer()
    {
        canJump = false;
        isGrounded = false;

        float dirX = (player.position.x > transform.position.x) ? 1f : -1f;

        // Zıplamadan önce yerdeki hızını sıfırla ve yeni hız ver
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(dirX * jumpForceX, jumpForceY);
        }

        StartCoroutine(JumpCooldownRoutine());
    }

    IEnumerator JumpCooldownRoutine()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Oyuncuya çarparsa hasar ver
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.SendMessage("TakeDamage", damageAmount, SendMessageOptions.DontRequireReceiver);
        }

        // Zemin veya platform katmanına çarparsa yerdedir
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            isGrounded = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // Collider'ı Is Trigger yapılmışsa da hasar verebilsin
        if (collider.CompareTag("Player"))
        {
            collider.SendMessage("TakeDamage", damageAmount, SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
