using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 4f; // 4 saniye sonra yok olur

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.gameObject.tag != "Enemy" && other.gameObject.tag != "Boss")
        {
            // Duvara veya zemine çarptı
            Destroy(gameObject);
        }
    }
}
