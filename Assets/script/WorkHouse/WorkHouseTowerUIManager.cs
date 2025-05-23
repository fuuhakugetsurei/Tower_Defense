using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class WorkHouseTowerUIManager : MonoBehaviour
{
    public GameObject uiPanel;
    public TMP_Text infoText;
    public Button upgradeButton;
    public TMP_Text PriceText;
    public Button cancelButton;
    public Button closeButton;  
    public Image image;
    public TMP_Text TitleText;

    private Spawner spawner;
    private GoldTower currentTower;
    private CoinManager coinManager;
    private WorkHouseGameManager workHouseGameManager;
    private int luckytimes;
    [SerializeField] private Sprite[] sprites;
    private string towerName;

    private int upgradePrice;

    void Start()
    {
        workHouseGameManager = FindFirstObjectByType<WorkHouseGameManager>();
        closeButton.onClick.AddListener(HideUI);
        cancelButton.onClick.AddListener(HideUI);
        uiPanel.SetActive(false);
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        coinManager = FindFirstObjectByType<CoinManager>();
        spawner = FindFirstObjectByType<Spawner>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && uiPanel.activeInHierarchy == true)
        {
            HideUI();
        }
    }
    public void ShowTowerInfo(GoldTower tower)
    {
        if (workHouseGameManager.isUIShowing) return;
        else workHouseGameManager.isUIShowing = true;
        currentTower = tower;
        upgradePrice = currentTower.GetPrice();
        luckytimes = tower.luckytimes;
        towerName = currentTower.GetTowerName();
        TitleText.text = towerName;
        PriceText.text = "價格: " + upgradePrice + "$";
        if (currentTower != null && spawner.IsSpawning() == false)
        {
            StartCoroutine(ShowUI());
            UpdateTowerInfo();
        }
    }

    public void HideUI()
    {
        uiPanel.SetActive(false);
        closeButton.gameObject.SetActive(false);
        currentTower = null;
        workHouseGameManager.isUIShowing = false;
    }

    private void UpdateTowerInfo()
    {
        if (currentTower != null)
        {
            upgradePrice = currentTower.GetPrice();
            PriceText.text = "價格: " + upgradePrice + "$";
            infoText.text = $"等級: {currentTower.GetLevel()}\n" + 
                            $"每秒生產效率: {(float)currentTower.GoldPerSecond / 2}";
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
        if (luckytimes > 0)
        {
            image.gameObject.SetActive(true);
            image.sprite = sprites[0];
        }
        else if (luckytimes == 0)
        {
            image.gameObject.SetActive(false);
        }
        else
        {
            image.gameObject.SetActive(true);
            image.sprite = sprites[1];
        }
    }

    private void OnUpgradeButtonClicked()
    {
        if (currentTower != null)
        {
            if (coinManager.SpendGold(upgradePrice))
            {
                currentTower.Upgrade();
                luckytimes = currentTower.luckytimes;
                UpdateTowerInfo();
            }
            else
            {
                TooltipManager.Instance.ShowTooltip("金幣不足！");
            }
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
