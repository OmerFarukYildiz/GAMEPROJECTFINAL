using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 2;
    public float lifetime = 4f; // 4 saniye sonra yok olur

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Eğer çarptığımız şey fiziksel bir duvar değil de sadece bir alan (Trigger) ise yok olma!
        if (other.isTrigger) return;

        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
        else
        {
            // Çarptığı şey Player değilse, Boss olup olmadığına bak.
            // Boss değilse (yani duvar, zemin vs. ise) mermiyi yok et.
            BossController boss = other.GetComponent<BossController>();
            if (boss == null)
            {
                Destroy(gameObject);
            }
        }
    }
}
