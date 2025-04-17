using UnityEngine;
using UnityEngine.UI;

public class TowerManager : MonoBehaviour
{
    public GameObject purchasePanel;
    public GameObject tower1Prefab;
    public GameObject tower2Prefab;
    public Button tower1Button;
    public Button tower2Button;
    public Button cancelButton;

    [SerializeField]
    private int tower1Cost = 50;  // 塔1 的價格
    [SerializeField]
    private int tower2Cost = 100; // 塔2 的價格

    private TowerPlacementPoint currentPlacementPoint;
    private CoinManager coinManager;
    private GameManager gameManager;
    void Start()
    {
        gameManager = Object.FindFirstObjectByType<GameManager>();
        coinManager = Object.FindFirstObjectByType<CoinManager>();
        if (coinManager == null)
        {
            Debug.LogError("場景中未找到 CoinManager！");
        }

        if (purchasePanel == null)
        {
            Debug.LogError("PurchasePanel 未設置！");
        }
        else
        {
            Debug.Log("PurchasePanel 已設置");
            purchasePanel.SetActive(false);
        }

        if (tower1Button == null || tower2Button == null || cancelButton == null)
        {
            Debug.LogError("某個按鈕未設置！");
        }
        else
        {
            tower1Button.onClick.AddListener(() => PurchaseTower(tower1Prefab, tower1Cost));
            tower2Button.onClick.AddListener(() => PurchaseTower(tower2Prefab, tower2Cost));
            cancelButton.onClick.AddListener(CancelPurchase);
            Debug.Log("按鈕監聽器已設置");
        }
    }

    public void ShowPurchaseUI(TowerPlacementPoint placementPoint)
    {
        if (gameManager.isUIShowing) 
        {
            return;
        }else
        {
            gameManager.isUIShowing = true;
        }
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
                Debug.Log($"成功購買塔，花费 {cost} 金幣");
                gameManager.isUIShowing = false;
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
        gameManager.isUIShowing = false;
        Debug.Log("取消購買");
    }
    public void BackToMain()
    {
        tower1Button.onClick.AddListener(() => PurchaseTower(tower1Prefab, tower1Cost));
        tower2Button.onClick.AddListener(() => PurchaseTower(tower2Prefab, tower2Cost));
        cancelButton.onClick.AddListener(CancelPurchase);
    }
}