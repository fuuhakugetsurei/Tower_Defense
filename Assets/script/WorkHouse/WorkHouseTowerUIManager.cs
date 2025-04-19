using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WorkHouseTowerUIManager : MonoBehaviour
{
    public GameObject uiPanel;
    public TMP_Text infoText;
    public Button upgradeButton;
    public TMP_Text PriceText;
    public Button closeButton;

    private Spawner spawner;
    private GoldTower currentTower;
    private CoinManager coinManager;
    private WorkHouseGameManager workHouseGameManager;

    private int upgradePrice;

    void Start()
    {
        workHouseGameManager = FindFirstObjectByType<WorkHouseGameManager>();
        closeButton.onClick.AddListener(HideUI);
        uiPanel.SetActive(false);
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        coinManager = FindFirstObjectByType<CoinManager>();
        spawner = FindFirstObjectByType<Spawner>();
    }

    public void ShowTowerInfo(GoldTower tower)
    {
        if (workHouseGameManager.isUIShowing) return;
        else workHouseGameManager.isUIShowing = true;
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
        uiPanel.SetActive(false);
        currentTower = null;
        workHouseGameManager.isUIShowing = false;
    }

    private void UpdateTowerInfo()
    {
        if (currentTower != null)
        {
            upgradePrice = currentTower.GetPrice();
            PriceText.text = "Price: " + upgradePrice + "$";
            infoText.text = $"LV: {currentTower.GetLevel()}\n" + 
                            $"GoldPerSecond: {(float)currentTower.GoldPerSecond / 2}";
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
