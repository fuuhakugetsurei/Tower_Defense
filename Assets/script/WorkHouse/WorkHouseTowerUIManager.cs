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

    private Spawner spawner;
    private GoldTower currentTower;
    private CoinManager coinManager;
    private WorkHouseGameManager workHouseGameManager;
    private int luckytimes;
    [SerializeField] private Sprite[] sprites;

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
        PriceText.text = "Price: " + upgradePrice + "$";
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
        if (luckytimes > 0)
        {
            image.sprite = sprites[0];
        }
        else if (luckytimes == 0)
        {
            image.gameObject.SetActive(false);
        }
        else
        {
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
                Debug.Log("金幣不足，無法升級！");
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
