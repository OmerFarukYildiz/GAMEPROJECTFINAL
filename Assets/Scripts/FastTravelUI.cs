using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro desteği için

public class FastTravelUI : MonoBehaviour
{
    public static FastTravelUI Instance;
    
    [Header("UI Elemanları")]
    public GameObject uiPanel;            // Arka plan paneli
    public Transform buttonContainer;     // Butonların dizileceği kutu (Vertical Layout Group olan yer)
    public GameObject buttonPrefab;       // Liste için yaratılacak buton şablonu
    
    private void Awake()
    {
        Instance = this;
        if (uiPanel != null) uiPanel.SetActive(false);
    }
    
    public void OpenBonfireMenu()
    {
        if (uiPanel == null) return;

        uiPanel.SetActive(true);
        Time.timeScale = 0f; // Oyunu durdur (düşmanlar saldıramasın)
        
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        ClearButtons();
        
        // 1. Seçenek: Burada Dinlen (Dünyayı Yeniler)
        CreateButton("Ateşte Dinlen", RestAtBonfire);
        
        // 2. Seçenek: Işınlanma Alt Menüsü
        CreateButton("Işınlan", ShowTeleportMenu);

        // 3. Seçenek: Kapat / Kalk
        CreateButton("Oyuna Dön", CloseMenu);
    }

    public void ShowTeleportMenu()
    {
        ClearButtons();
        
        if(GameManager.Instance != null)
        {
            foreach(var bonfire in GameManager.Instance.unlockedBonfires)
            {
                // Mevcut sahnede aynı bonfire'ı göstermeyebiliriz ama göstermek de sorun olmaz.
                string btnText = bonfire.sceneName + " -> " + bonfire.bonfireID;
                CreateButton(btnText, () => FastTravelTo(bonfire));
            }
        }

        CreateButton("<- Geri", ShowMainMenu);
    }

    private void ClearButtons()
    {
        foreach(Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
    }
    
    private void CreateButton(string text, UnityEngine.Events.UnityAction action)
    {
        if (buttonPrefab == null)
        {
            Debug.LogError("Button Prefab atanmamış veya silinmiş! Lütfen Inspector'dan Mavi Prefab'ı atayın, sahnedekini değil.");
            return;
        }

        GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
        Button btn = btnObj.GetComponent<Button>();
        
        // Klasik Text desteği
        Text txt = btnObj.GetComponentInChildren<Text>();
        if (txt != null) txt.text = text;
        
        // TextMeshPro desteği
        TextMeshProUGUI tmp = btnObj.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = text;
        
        btn.onClick.AddListener(action);
    }
    
    public void RestAtBonfire()
    {
        CloseMenu();

        // Sahneyi yeniden yükle (Düşmanlar yeniden doğsun, bosslar ve kristallerin durumu save dosyanızdan veya GameManager'dan korunmalı)
        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    
    public void FastTravelTo(GameManager.BonfireData data)
    {
        CloseMenu();

        // Hedef bonfire'ı son nokta olarak ayarla
        GameManager.Instance.SetLastBonfire(data.bonfireID, data.sceneName, data.spawnPosition);
        
        // İlgili sahneye geç
        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.LoadScene(data.sceneName);
        }
        else
        {
            SceneManager.LoadScene(data.sceneName);
        }
    }
    
    public void CloseMenu()
    {
        uiPanel.SetActive(false);
        Time.timeScale = 1f; // Zamanı tekrar başlat
    }
}
