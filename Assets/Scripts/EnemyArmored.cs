using UnityEngine;

public class EnemyArmored : MonoBehaviour
{
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public bool BlocksDamage(Transform damageSource)
    {
        if (damageSource == null) return false;

        // Düşmanın baktığı yön (flipX true ise sola bakıyor demektir)
        bool isFacingRight = sr != null ? !sr.flipX : true;
        
        // Hasar kaynağının düşmana göre pozisyonu
        bool sourceIsOnRight = damageSource.position.x > transform.position.x;

        // Eğer düşman sağa bakıyorsa ve hasar sağdan geliyorsa bloklar
        if (isFacingRight && sourceIsOnRight) return true;
        
        // Eğer düşman sola bakıyorsa ve hasar soldan geliyorsa bloklar
        if (!isFacingRight && !sourceIsOnRight) return true;

        // Arkadan geliyorsa bloklayamaz
        return false;
    }
}
