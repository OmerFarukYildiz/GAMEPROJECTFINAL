using UnityEngine;

public class EnemyDive : MonoBehaviour
{
    [Header("Yarasa Ayarları")]
    public float diveSpeed = 10f;
    public float returnSpeed = 3f;
    public float patrolSpeed = 2f;
    public float patrolDistance = 3f; // Sağa sola gitme mesafesi
    public float detectionWidth = 2f; // Oyuncuyu algılama genişliği (X ekseni)
    public float diveCooldown = 2f;   // Tekrar dalış yapmadan önce bekleme süresi
    
    private Vector3 originalPosition;
    private Transform player;
    private bool isDiving = false;
    private bool isReturning = false;
    private float lastDiveTime = 0f;

    void Start()
    {
        originalPosition = transform.position;
        
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        // Eğer dalış yapmıyorsa ve geri dönmüyorsa, oyuncuyu kontrol et ve devriye gez
        if (!isDiving && !isReturning)
        {
            // Devriye gez (Sağa sola hareket)
            float newX = originalPosition.x + Mathf.Sin(Time.time * patrolSpeed) * patrolDistance;
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

            // Cooldown dolduysa oyuncuyu kontrol et
            if (Time.time >= lastDiveTime + diveCooldown)
            {
                float xDiff = Mathf.Abs(transform.position.x - player.position.x);
                float yDiff = transform.position.y - player.position.y;

                if (xDiff < detectionWidth && yDiff > 0)
                {
                    isDiving = true;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (isDiving)
        {
            // Aşağı doğru hızlıca düş
            transform.position += Vector3.down * diveSpeed * Time.fixedDeltaTime;
        }
        else if (isReturning)
        {
            // Yukarı çıkarken de sağa sola salınımı hesaba kat ki zıplama/glitch olmasın
            float targetX = originalPosition.x + Mathf.Sin(Time.time * patrolSpeed) * patrolDistance;
            Vector3 targetPos = new Vector3(targetX, originalPosition.y, originalPosition.z);

            // Yavaşça yukarı geri dön
            transform.position = Vector3.MoveTowards(transform.position, targetPos, returnSpeed * Time.fixedDeltaTime);
            
            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                isReturning = false;
                lastDiveTime = Time.time; // Dalış bittiğinde cooldown'ı başlat
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDiving)
        {
            if (collision.CompareTag("Player"))
            {
                collision.SendMessage("TakeDamage", 1, SendMessageOptions.DontRequireReceiver);
                isDiving = false;
                isReturning = true;
            }
            // Zemin layer'ına çarparsa dön
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                isDiving = false;
                isReturning = true;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position - new Vector3(detectionWidth, 0, 0), transform.position - new Vector3(detectionWidth, 5, 0));
        Gizmos.DrawLine(transform.position + new Vector3(detectionWidth, 0, 0), transform.position + new Vector3(detectionWidth, 5, 0));
    }
}
