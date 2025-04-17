using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TowerSet : MonoBehaviour
{
    public bool isOccupied = false;
    private GameObject currentTower;
    private WorkHouseTowerManager towerManager;
    
    [SerializeField]
    private Vector3 spawnOffset = Vector3.zero; // 可在 Inspector 中調整偏移
    private LuckyManager luckyManager;

    void Start()
    {
        luckyManager = Object.FindFirstObjectByType<LuckyManager>();
        towerManager = Object.FindFirstObjectByType<WorkHouseTowerManager>();
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
        Spawner spawner = Object.FindFirstObjectByType<Spawner>();
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
            SceneManager.MoveGameObjectToScene(currentTower, gameObject.scene); 
            isOccupied = true;
            if (currentTower.TryGetComponent<GoldTower>(out GoldTower towerScript))
            {
                bool isLucky = luckyManager.RollLucky();
                if (isLucky)
                {
                    int bonusMultiplier = 5; 
                    towerScript.ApplyLuckyBonus(bonusMultiplier);
                    luckyManager.SpendLucky(3);
                }
                else
                {   
                    luckyManager.AddLucky(5);
                }   
            }
            Debug.Log($"{gameObject.name} 放置了塔: {currentTower.name}，位置: {spawnPosition}");
            gameObject.SetActive(false);
            
        }
    }
}
