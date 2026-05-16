using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Ölüm Ayarları")]
    [Tooltip("Eğer işaretlenirse, oyuncunun canına bakmaksızın anında öldürür. İşaretli değilse oyuncuya belirli bir hasar verir.")]
    public bool instantKill = true;
    public int damageAmount = 9999;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Eğer çarpan obje "Player" tag'ine sahipse
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            
            if (playerHealth != null)
            {
                if (instantKill)
                {
                    // Oyuncunun mevcut canı kadar hasar vererek kesin ölmesini sağla
                    playerHealth.TakeDamage(playerHealth.GetCurrentHealth());
                }
                else
                {
                    // Anında öldürme kapalıysa sadece belirlenen hasarı ver
                    playerHealth.TakeDamage(damageAmount);
                }
            }
        }
    }
}
