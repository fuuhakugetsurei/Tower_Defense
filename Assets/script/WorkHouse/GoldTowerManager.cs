using UnityEngine;
using System.Collections.Generic;

public class GoldTowerManager : MonoBehaviour
{
    public static GoldTowerManager Instance { get; private set; }
    private List<GoldTower> towers = new List<GoldTower>();
    private float timer = 0f;

    private LuckyManager luckyManager;
    private WorkHouseGameManager workHouseGameManager;
    private Spawner spawner;
    private CoinManager coinManager;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); // 保持運行
        luckyManager = FindFirstObjectByType<LuckyManager>();
        workHouseGameManager = FindFirstObjectByType<WorkHouseGameManager>();
        spawner = FindFirstObjectByType<Spawner>();
        coinManager = FindFirstObjectByType<CoinManager>();
    }

    public void AddTower(GoldTower tower)
    {
        towers.Add(tower);
    }

    public void RemoveTower(GoldTower tower)
    {
        towers.Remove(tower);
    }

    void Update()
    {
        if (spawner != null && spawner.IsSpawning())
        {
            timer += Time.deltaTime;
            if (timer >= 2f)
            {
                foreach (var tower in towers)
                {
                    if (tower != null) // 確保塔未銷毀
                    {
                        if (luckyManager.RollLucky())
                        {
                            coinManager.AddGold(tower.GoldPerSecond * 2);
                            luckyManager.SpendLucky(3);
                            workHouseGameManager.UpdateUI();
                        }
                        else
                        {
                            coinManager.AddGold(tower.GoldPerSecond);
                            workHouseGameManager.UpdateUI();
                            luckyManager.AddLucky(5);
                        }
                    }
                }
                timer -= 2f;
            }
        }
    }
}