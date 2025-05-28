using UnityEngine;
using UnityEngine.EventSystems;

public class GoldTower : MonoBehaviour
{
    public int GoldPerSecond { get; private set; } = 1; // 公開屬性
    private LuckyManager luckyManager;
    private CoinManager coinManager;
    private WorkHouseGameManager workHouseGameManager;
    private int Price = 10;
    private int level = 1;
    private int maxLevel = 20;
    public int luckytimes = 0; 
    private string towerName = "金幣塔";
    private int cost = 30; // 總價值
    public TowerSet towerSet;

    void Start()
    {
        GoldTowerManager.Instance.AddTower(this); // 註冊到 CoinManager
        luckyManager = FindFirstObjectByType<LuckyManager>();
        workHouseGameManager = FindFirstObjectByType<WorkHouseGameManager>();   
        coinManager = FindFirstObjectByType<CoinManager>();
    }

    void OnDestroy()
    {
        GoldTowerManager.Instance.RemoveTower(this); // 清理
    }

    public void ApplyLuckyBonus(int multiplier)
    {
        GoldPerSecond += multiplier;
        Debug.Log($"Lucky bonus applied: {multiplier}, new goldPerSecond: {GoldPerSecond}");
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        WorkHouseTowerUIManager uiManager = FindFirstObjectByType<WorkHouseTowerUIManager>();
        if (uiManager != null)
        {
            uiManager.ShowTowerInfo(this);
        }
    }

    public void Upgrade()
    {
        if (level < maxLevel)
        {
            bool isLucky = luckyManager.RollLucky();
            level++;
            if (isLucky)
            {
                GoldPerSecond += 2;
                luckyManager.SpendLucky(3);
                luckytimes++;
                workHouseGameManager.UpdateUI();
                
                TooltipManager.Instance.ShowTooltip("幸運值加成！");
            }
            else
            {
                GoldPerSecond += 1;
                luckyManager.AddLucky(5);
                luckytimes--;
                workHouseGameManager.UpdateUI();
                
            }
            cost += Price;
            UpdatePrice();

        }
        else
        {
            Debug.Log($"{gameObject.name} 已達最大等級 Lv{level}");
        }
    }

    private void UpdatePrice()
    {
        Price += 20 * (level-1);
    }
    public void DestroyTower()
    {
        if (coinManager != null)
        {
            coinManager.AddGold((int)(cost * 0.8f)); // 銷毀塔時返還80%金幣
            workHouseGameManager.UpdateUI();
        }
        GoldTowerManager.Instance.RemoveTower(this);
        towerSet.RemoveTower();
        Destroy(gameObject);
    }
    public int GetPrice() => Price;
    public int GetLevel() => level;
    public int GetMaxLevel() => maxLevel;
    public string GetTowerName() => towerName;
}