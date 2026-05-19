using UnityEngine;
using UnityEngine.UI;

public class ManaUI : MonoBehaviour
{
    public static ManaUI Instance { get; private set; }

    [Header("Referanslar")]
    public Slider manaSlider;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void UpdateMana(int currentMana, int maxMana)
    {
        if (manaSlider != null)
        {
            manaSlider.maxValue = maxMana;
            manaSlider.value = currentMana;
        }
    }
}
