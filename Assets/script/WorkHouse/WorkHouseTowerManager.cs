using UnityEngine;
using UnityEngine.UI;


public class WorkHouseTowerManager : MonoBehaviour
{
    public GameObject purchasePanel;
    public GameObject tower1Prefab;
    public Button cancelButton;
    public Button closeButton;
    public int tower1Cost = 30;  

    private TowerSet currentPlacementPoint;
    private CoinManager coinManager;
    private WorkHouseGameManager workHouseGameManager;
    
    void Start()
    {
        // 查找 CoinManager
        coinManager = FindFirstObjectByType<CoinManager>();
        purchasePanel.SetActive(false);
        closeButton.onClick.AddListener(CancelPurchase);
        cancelButton.onClick.AddListener(CancelPurchase);
        Debug.Log("按鈕監聽器已設置");
        workHouseGameManager = FindFirstObjectByType<WorkHouseGameManager>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && purchasePanel.activeInHierarchy == true)
        {
            CancelPurchase();
        }
    }
    public void ShowPurchaseUI(TowerSet placementPoint)
    {
        if (workHouseGameManager.isUIShowing) return;
        else workHouseGameManager.isUIShowing = true;
        currentPlacementPoint = placementPoint;
        if (purchasePanel != null)
        {
            purchasePanel.SetActive(true);
            closeButton.gameObject.SetActive(true);
            Debug.Log("顯示購買 UI");
        }
        else
        {
            Debug.LogError("PurchasePanel 是 null，無法顯示 UI");
        }
    }

    public void PurchaseTower(GameObject towerPrefab, int cost)
    {
        if (currentPlacementPoint != null && coinManager != null)
        {
            if (coinManager.SpendGold(cost)) // 檢查並扣除金幣
            {
                currentPlacementPoint.PlaceTower(towerPrefab);
                purchasePanel.SetActive(false);
                closeButton.gameObject.SetActive(false);
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
        closeButton.gameObject.SetActive(false);
        currentPlacementPoint = null;
        Debug.Log("取消購買");
        workHouseGameManager.isUIShowing = false;
    }
}