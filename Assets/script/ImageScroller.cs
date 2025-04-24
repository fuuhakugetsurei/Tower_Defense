using UnityEngine;

public class ImageScroller : MonoBehaviour
{
    public RectTransform imageHolder;
    public float imageWidth = 200f;  // 一張圖片的寬度
    private int currentIndex = 0;
    public int maxIndex = 0;  // 圖片總數 - 1

    private TowerManager towerManager;
    void Start()
    {
        towerManager = FindFirstObjectByType<TowerManager>();
    }

    public void ScrollLeft()
    {
        if (currentIndex == 0)
        {
            return; // 如果已經在第一張圖片，則不執行任何操作
        }
        currentIndex = Mathf.Max(0, currentIndex - 1);
        UpdatePosition();
    }

    public void ScrollRight()
    {
        if (currentIndex == maxIndex)
        {
            return; // 如果已經在最後一張圖片，則不執行任何操作
        }
        currentIndex = Mathf.Min(maxIndex, currentIndex + 1);
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        // 每次滾動圖片，將 ImageHolder 的 anchoredPosition 調整，使圖片停在正確位置
        Vector2 newPos = new Vector2(-currentIndex * imageWidth, 0);
        imageHolder.anchoredPosition = newPos;
    }
    public void Enter()
    {
        if (currentIndex == 0)
        {
            towerManager.PurchaseTower(towerManager.tower1Prefab, towerManager.tower1Cost);
        }
    }
}
