using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class WorkHouseCastle : MonoBehaviour
{
    public GameObject uiPanel;
    public TMP_Text infoText;
    public Button upgradeButton;
    public TMP_Text PriceText;
    public Button cancelButton;
    public Button closeButton;
    public Button HealButton;
    public TMP_Text HealPriceText;

    private Spawner spawner;
    private CoinManager coinManager;
    private Castle castle;
    private WorkHouseGameManager workHouseGameManager;

    void Start()
    {
        workHouseGameManager = FindFirstObjectByType<WorkHouseGameManager>();
        castle = FindFirstObjectByType<Castle>();
        cancelButton.onClick.AddListener(HideUI);
        uiPanel.SetActive(false);
        closeButton.onClick.AddListener(HideUI);
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        coinManager = FindFirstObjectByType<CoinManager>();
        spawner = FindFirstObjectByType<Spawner>();
        HealButton.onClick.AddListener(OnHealButtonClicked);
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && uiPanel.activeInHierarchy == true)
        {
            HideUI();
        }
    }
    public void HideUI()
    {
        uiPanel.SetActive(false);
        closeButton.gameObject.SetActive(false);    
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
        else 
        {
            TooltipManager.Instance.ShowTooltip("金幣不足！");
        }
    }
    private void OnHealButtonClicked()
    {
        if (coinManager.SpendGold(Castle.Instance.healPrice))
        {
            castle.Heal();
            WorkHouseGameManager.Instance.UpdateUI();
            UpdateInfo();
            TooltipManager.Instance.ShowTooltip("已回復50%血量！");
        }else 
        {
            TooltipManager.Instance.ShowTooltip("金幣不足！");
        }
    }
    private void OnMouseDown()
    {
        if (workHouseGameManager.isUIShowing) return;
        else workHouseGameManager.isUIShowing = true;
        if (spawner.IsSpawning() == false)
        {
            StartCoroutine(ShowUI());
            UpdateInfo();
        }
    }
    private void UpdateInfo()
    {
        PriceText.text = "價格: " + Castle.Instance.upgradePrice + "$";
        HealPriceText.text = "價格: " + Castle.Instance.healPrice + "$";
        infoText.text = $"等級: {Castle.Instance.level}\n" +
                        $"最大血量: {Castle.Instance.maxHealth}";

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
    private IEnumerator ShowUI()
    {
        uiPanel.SetActive(true);
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(true);
        }
    }

}
