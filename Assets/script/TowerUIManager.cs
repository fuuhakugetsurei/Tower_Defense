using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TowerUIManager : MonoBehaviour
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
    private Tower1 currentTower;
    private CoinManager coinManager;
    private GameManager gameManager;
    private int upgradePrice;
    private int luckytimes;
    private string towerName;
    [SerializeField] private Sprite[] sprites;
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
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
    public void ShowTowerInfo(Tower1 tower)
    {
        if (gameManager.isUIShowing) return;
        else gameManager.isUIShowing = true;
        currentTower = tower;
        upgradePrice = currentTower.GetPrice();
        towerName = currentTower.GetTowerName();
        TitleText.text = towerName;
        PriceText.text = "價格: " + upgradePrice + "$";
        luckytimes = tower.luckytimes;
        if (currentTower != null && spawner.IsSpawning() == false)
        {
            StartCoroutine(ShowUI());
            UpdateTowerInfo();
        }
    }

    public void HideUI()
    {
        gameManager.isUIShowing = false;
        uiPanel.SetActive(false);
        closeButton.gameObject.SetActive(false);
        currentTower = null;
    }

    private void UpdateTowerInfo()
    {
        if (currentTower != null)
        {
            upgradePrice = currentTower.GetPrice();
            PriceText.text = "價格: " + upgradePrice + "$";
            infoText.text = $"等級: {currentTower.GetLevel()}\n" +
                            $"攻擊傷害: {currentTower.GetDamage()}\n" +
                            $"攻擊速度: {currentTower.GetAttackSpeed()}\n" +
                            $"攻擊範圍: {currentTower.GetAttackRange()}";
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
