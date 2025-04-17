using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Net.Http.Headers;

public class WorkHouseCastle : MonoBehaviour
{
    public GameObject uiPanel;
    public TMP_Text infoText;
    public Button upgradeButton;
    public TMP_Text PriceText;
    public Button closeButton;

    private Spawner spawner;
    private CoinManager coinManager;
    private Castle castle;
    private WorkHouseGameManager workHouseGameManager;

    void Start()
    {
        workHouseGameManager = FindFirstObjectByType<WorkHouseGameManager>();
        castle = FindFirstObjectByType<Castle>();
        closeButton.onClick.AddListener(HideUI);
        uiPanel.SetActive(false);
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        coinManager = FindFirstObjectByType<CoinManager>();
        spawner = FindFirstObjectByType<Spawner>();
    }

    public void HideUI()
    {
        uiPanel.SetActive(false);
        workHouseGameManager.isUIShowing = false;
    }

    private void OnUpgradeButtonClicked()
    {
        if (coinManager.SpendGold(Castle.Instance.upgradePrice))
        {
            castle.Upgrade();
            WorkHouseGameManager.Instance.UpdateUI();
            UpdateInfo();
        }
    }
    private void OnMouseDown()
    {
        if (workHouseGameManager.isUIShowing) return;
        else workHouseGameManager.isUIShowing = true;
        if (spawner.IsSpawning() == false)
        {
            uiPanel.SetActive(true);
            UpdateInfo();
        }
    }
    private void UpdateInfo()
    {
        PriceText.text = "Price: " + Castle.Instance.upgradePrice + "$";
        infoText.text = $"LV: {Castle.Instance.level}\n" +
                        $"MaxHP: {Castle.Instance.maxHealth}";

        if (Castle.Instance.level >= Castle.Instance.maxLevel)
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
}
