using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TowerUIManager : MonoBehaviour
{
    public GameObject uiPanel;
    public TMP_Text infoText;
    public Button upgradeButton;
    public TMP_Text PriceText;
    public Button closeButton;

    private Spawner spawner;
    private Tower1 currentTower;
    private CoinManager coinManager;
    private GameManager gameManager;
    private int upgradePrice;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        closeButton.onClick.AddListener(HideUI);
        uiPanel.SetActive(false);
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        coinManager = FindFirstObjectByType<CoinManager>();
        spawner = FindFirstObjectByType<Spawner>();
    }

    public void ShowTowerInfo(Tower1 tower)
    {
        if (gameManager.isUIShowing) return;
        else gameManager.isUIShowing = true;
        currentTower = tower;
        upgradePrice = currentTower.GetPrice();
        PriceText.text = "Price: " + upgradePrice + "$";
        if (currentTower != null && spawner.IsSpawning() == false)
        {
            uiPanel.SetActive(true);
            UpdateTowerInfo();
        }
    }

    public void HideUI()
    {
        gameManager.isUIShowing = false;
        uiPanel.SetActive(false);
        currentTower = null;
    }

    private void UpdateTowerInfo()
    {
        if (currentTower != null)
        {
            upgradePrice = currentTower.GetPrice();
            PriceText.text = "Price: " + upgradePrice + "$";
            infoText.text = $"LV: {currentTower.GetLevel()}\n" +
                            $"ATK: {currentTower.GetDamage()}\n" +
                            $"AS: {currentTower.GetAttackSpeed()}\n" +
                            $"RNG: {currentTower.GetAttackRange()}";
        }
        if (currentTower.GetLevel() >= currentTower.GetMaxLevel())
        {
            upgradeButton.gameObject.SetActive(false);
            PriceText.gameObject.SetActive(false);
        }
        else
        {
            upgradeButton.gameObject.SetActive(true);
            PriceText.gameObject.SetActive(true);
        }
    }

    private void OnUpgradeButtonClicked()
    {
        if (currentTower != null)
        {
            if (coinManager.SpendGold(upgradePrice))
            {
                currentTower.Upgrade();
                UpdateTowerInfo();
            }
            else
            {
                Debug.Log("金幣不足，無法升級！");
            }
        }
    }           
}
