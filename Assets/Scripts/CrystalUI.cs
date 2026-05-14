using UnityEngine;
using TMPro;

public class CrystalUI : MonoBehaviour
{
    public TextMeshProUGUI crystalText;

    void Update()
    {
        if (ColorManager.Instance == null || crystalText == null) return;

        // "Alınan / Toplam" formatında yazdır
        crystalText.text = $"{ColorManager.Instance.GetCollectedCrystals()} / {ColorManager.Instance.GetTotalCrystals()}";
    }
}
