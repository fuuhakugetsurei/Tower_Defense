using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TowerManager : MonoBehaviour
{
    public GameObject purchasePanel;
    public GameObject tower1Prefab;
    public Button cancelButton;
    public Button closeButton;
    
    public int tower1Cost = 50;  // 塔1 的價格

    private TowerPlacementPoint currentPlacementPoint;
    private CoinManager coinManager;
    private GameManager gameManager;
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        coinManager = FindFirstObjectByType<CoinManager>();
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

        if (cancelButton == null)
        {
            Debug.LogError("某個按鈕未設置！");
        }
        else
        {
            //tower1Button.onClick.AddListener(() => PurchaseTower(tower1Prefab, tower1Cost));
            cancelButton.onClick.AddListener(CancelPurchase);
            closeButton.onClick.AddListener(CancelPurchase);
            Debug.Log("按鈕監聽器已設置");
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && purchasePanel.activeInHierarchy == true)
        {
            CancelPurchase();
        }
    }
    public void ShowPurchaseUI(TowerPlacementPoint placementPoint)
    {
        if (gameManager.isUIShowing) 
        {
            return;
        }
        else
        {
            gameManager.isUIShowing = true;
        }
        currentPlacementPoint = placementPoint;
        if (purchasePanel != null)
        {
            StartCoroutine(ShowUI());
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
                Debug.Log($"成功購買塔，花费 {cost} 金幣");
                gameManager.isUIShowing = false;
            }
            else
            {
                TooltipManager.Instance.ShowTooltip("金幣不足！");
                Debug.Log("金幣不足，無法購買塔！");
                // 可選：這裡可以顯示 UI 提示，例如 "金幣不足"
            }
        }
    }
    private IEnumerator ShowUI()
    {
        purchasePanel.SetActive(true);
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        if (closeButton != null)
        {
            closeButton.gameObject.SetActive(true);
        }
    }
    private void CancelPurchase()
    {
        purchasePanel.SetActive(false);
        closeButton.gameObject.SetActive(false);
        currentPlacementPoint = null;
        gameManager.isUIShowing = false;
        Debug.Log("取消購買");
    }
}