using UnityEngine;
using UnityEngine.EventSystems;

public class GoldTower : MonoBehaviour
{
    public int GoldPerSecond { get; private set; } = 1; // 公開屬性
    private LuckyManager luckyManager;
    private WorkHouseGameManager workHouseGameManager;
    private int Price = 10;
    private int level = 1;
    private int maxLevel = 20;
    public int luckytimes = 0; 

    void Start()
    {
        GoldTowerManager.Instance.AddTower(this); // 註冊到 CoinManager
        luckyManager = FindFirstObjectByType<LuckyManager>();
        workHouseGameManager = FindFirstObjectByType<WorkHouseGameManager>();   
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
                UpdatePrice();
            }
            else
            {
                GoldPerSecond += 1;
                luckyManager.AddLucky(5);
                luckytimes--;
                workHouseGameManager.UpdateUI();
                UpdatePrice();
            }
            UpdatePrice();

        }
        else
        {
            Debug.Log($"{gameObject.name} 已達最大等級 Lv{level}");
        }
    }

    private void UpdatePrice()
    {
        Price += 10 * (level-1);
    }
    public int GetPrice() => Price;
    public int GetLevel() => level;
    public int GetMaxLevel() => maxLevel;
}