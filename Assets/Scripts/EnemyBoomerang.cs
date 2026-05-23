using UnityEngine;

public class EnemyBoomerang : MonoBehaviour
{
    [Header("Bumerang Ayarları")]
    public float speed = 10f;
    public int damage = 2;
    public float flyTime = 0.4f; // Ne kadar süre ileri gidecek (Çok daha kısa)
    
    private Transform thrower;
    private Vector2 startDirection;
    private Rigidbody2D rb;
    private bool isReturning = false;
    private float timer = 0f;
    private Vector2 lastKnownThrowerPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 dir, Transform creator)
    {
        // Unity Inspector'da eski değerler kalmış olabileceği için buradan zorla (Force) düzeltiyoruz.
        flyTime = 1.1f;
        speed = 8f;

        startDirection = dir.normalized;
        thrower = creator;
        if (thrower != null) lastKnownThrowerPos = thrower.position;
        
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f; // Bumerang düşmez
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Kendi etrafında dönme efekti
        transform.Rotate(0, 0, 360 * Time.deltaTime);

        // Atıcı hala yaşıyorsa son pozisyonunu kaydet
        if (thrower != null)
        {
            lastKnownThrowerPos = thrower.position;
        }

        if (!isReturning)
        {
            // İleri uçuş
            if (rb != null)
            {
                rb.linearVelocity = startDirection * speed;
            }

            if (timer >= flyTime)
            {
                isReturning = true;
            }
        }
        else
        {
            // Geri dönüş (Thrower yaşasa da ölse de son bilinen yerine döner)
            Vector2 targetPos = (thrower != null) ? (Vector2)thrower.position : lastKnownThrowerPos;
            Vector2 returnDir = (targetPos - (Vector2)transform.position).normalized;
            
            // Bumerang geri dönerken ivmelenir
            if (rb != null)
            {
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, returnDir * speed, Time.deltaTime * 5f);
            }

            // Eğer hedef noktasına ulaştıysa (yakalandıysa veya öldüğü yere döndüyse)
            if (Vector2.Distance(transform.position, targetPos) < 0.5f)
            {
                if (thrower != null)
                {
                    EnemyBoomerangThrower throwerScript = thrower.GetComponent<EnemyBoomerangThrower>();
                    if (throwerScript != null) throwerScript.CatchBoomerang();
                }
                
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player"))
        {
            hitInfo.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
        // Bumerang duvarlara çarparsa sekebilir veya yok olabilir, şimdilik içinden geçsin ki takılmasın
    }
}
