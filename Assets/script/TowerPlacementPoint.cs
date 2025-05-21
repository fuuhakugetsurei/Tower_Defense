using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public class TowerPlacementPoint : MonoBehaviour
{
    public bool isOccupied = false;
    private GameObject currentTower;
    private TowerManager towerManager;
    
    
    [SerializeField]
    private Vector3 spawnOffset = Vector3.zero; // 可在 Inspector 中調整偏移
    private LuckyManager luckyManager;

    void Start()
    {
        luckyManager = FindFirstObjectByType<LuckyManager>();
        towerManager = FindFirstObjectByType<TowerManager>();
        if (towerManager == null)
        {
            Debug.LogError("場景中未找到 TowerManager！");
        }
    }
    
    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        Debug.Log($"{gameObject.name} 被點擊");
        Spawner spawner = Object.FindFirstObjectByType<Spawner>();
        if (spawner != null)
        {
            Debug.Log($"Spawner 找到，IsSpawning: {spawner.IsSpawning()}");
        }
        else
        {
            Debug.LogError("Spawner 未找到！");
        }

        Debug.Log($"IsOccupied: {isOccupied}");
        if (spawner != null && !spawner.IsSpawning() && !isOccupied)
        {
            if (towerManager != null)
            {
                Debug.Log("條件滿足，顯示 UI");
                towerManager.ShowPurchaseUI(this);
            }
            else
            {
                Debug.LogError("TowerManager 未設置！");
            }
        }
        else if (isOccupied)
        {
            Debug.Log($"{gameObject.name} 已被佔用");
        }
        else
        {
            Debug.Log("關卡進行中，無法放置塔");
        }
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }

    public void PlaceTower(GameObject towerPrefab)
    {
        if (!isOccupied)
        {
            // 使用 spawnOffset 調整生成位置
            Vector3 spawnPosition = transform.position + spawnOffset;
            currentTower = Instantiate(towerPrefab, spawnPosition, Quaternion.identity);
            isOccupied = true;
            if (currentTower.TryGetComponent<Tower1>(out Tower1 towerScript))
            {
                bool isLucky = luckyManager.RollLucky();
                if (isLucky)
                {
                    float bonusMultiplier = 1.2f; // 加成倍率 20%
                    towerScript.ApplyLuckyBonus(bonusMultiplier);
                    towerScript.luckytimes++;
                    luckyManager.SpendLucky(3);
                    Debug.Log("幸運塔放置成功！攻擊力提升！");
                    TooltipManager.Instance.ShowTooltip("幸運值加成！");
                }
                else
                {   
                    float bonusMultiplier = 0.8f;
                    towerScript.ApplyLuckyBonus(bonusMultiplier);
                    luckyManager.AddLucky(5);
                    towerScript.luckytimes--;
                    Debug.Log("不是幸運塔");
                }   
            }
            Debug.Log($"{gameObject.name} 放置了塔: {currentTower.name}，位置: {spawnPosition}");
            gameObject.SetActive(false);
            
        }
    }
}