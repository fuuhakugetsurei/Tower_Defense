using UnityEngine;
using UnityEngine.UI;


public class WorkHouseTowerManager : MonoBehaviour
{
    public GameObject purchasePanel;
    public GameObject tower1Prefab;
    public Button tower1Button;
    public Button cancelButton;
    

    [SerializeField]
    private int tower1Cost = 30;  // 塔1 的價格

    private TowerSet currentPlacementPoint;
    private CoinManager coinManager;
    private WorkHouseGameManager workHouseGameManager;
    
    void Start()
    {
        // 查找 CoinManager
        coinManager = Object.FindFirstObjectByType<CoinManager>();
        purchasePanel.SetActive(false);
        tower1Button.onClick.AddListener(() => PurchaseTower(tower1Prefab, tower1Cost));
        cancelButton.onClick.AddListener(CancelPurchase);
        Debug.Log("按鈕監聽器已設置");
        workHouseGameManager = FindFirstObjectByType<WorkHouseGameManager>();

    }

    public void ShowPurchaseUI(TowerSet placementPoint)
    {
        if (workHouseGameManager.isUIShowing) return;
        else workHouseGameManager.isUIShowing = true;
        currentPlacementPoint = placementPoint;
        if (purchasePanel != null)
        {
            purchasePanel.SetActive(true);
            Debug.Log("顯示購買 UI");
        }
        else
        {
            Debug.LogError("PurchasePanel 是 null，無法顯示 UI");
        }
    }

    private void PurchaseTower(GameObject towerPrefab, int cost)
    {
        if (currentPlacementPoint != null && coinManager != null)
        {
            if (coinManager.SpendGold(cost)) // 檢查並扣除金幣
            {
                currentPlacementPoint.PlaceTower(towerPrefab);
                purchasePanel.SetActive(false);
                currentPlacementPoint = null;
                workHouseGameManager.UpdateUI();
                workHouseGameManager.isUIShowing = false;
                Debug.Log($"成功購買塔，花费 {cost} 金幣");
            }
            else
            {
                Debug.Log("金幣不足，無法購買塔！");
                // 可選：這裡可以顯示 UI 提示，例如 "金幣不足"
            }
        }
    }
    private void CancelPurchase()
    {
        purchasePanel.SetActive(false);
        currentPlacementPoint = null;
        Debug.Log("取消購買");
        workHouseGameManager.isUIShowing = false;
    }
}