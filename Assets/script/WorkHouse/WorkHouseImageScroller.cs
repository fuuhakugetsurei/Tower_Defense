using UnityEngine;
using TMPro;


public class WorkHouseImageScroller : MonoBehaviour
{
    public RectTransform imageHolder;
    public float imageWidth = 200f;  // 一張圖片的寬度
    private int currentIndex = 0;
    public int maxIndex = 0;  // 圖片總數 - 1
    public TMP_Text infoText;
    public WorkHouseTowerInfo[] towerInfos; // 這是從 ScriptableObject 獲取的塔的資訊

    private WorkHouseTowerManager towerManager;
    
    void Start()
    {
        towerManager = FindFirstObjectByType<WorkHouseTowerManager>();
        InfoUpdate();
    }

    public void ScrollLeft()
    {
        if (currentIndex == 0)
        {
            return; // 如果已經在第一張圖片，則不執行任何操作
        }
        currentIndex = Mathf.Max(0, currentIndex - 1);
        UpdatePosition();
        InfoUpdate();
    }

    public void ScrollRight()
    {
        if (currentIndex == maxIndex)
        {
            return; // 如果已經在最後一張圖片，則不執行任何操作
        }
        currentIndex = Mathf.Min(maxIndex, currentIndex + 1);
        UpdatePosition();
        InfoUpdate();
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
        // todo:未來可以根據 currentIndex 動態選擇塔，例如：
        //towerManager.PurchaseTower(towerDatas[currentIndex].prefab, towerDatas[currentIndex].cost);*/
    }

    public void InfoUpdate()
    {
        WorkHouseTowerInfo data = towerInfos[currentIndex];
        if (infoText != null)
        {
            if (currentIndex == 0)
            {
                infoText.text = $"{data.towerName} :\n" +
                                $"等級上限 : {data.maxlevel}\n" +
                                $"價格 : {data.cost}";
            }
        }
        if (data.additionalAttributes != null && data.additionalAttributes.Count > 0)
        {
            foreach (var attribute in data.additionalAttributes)
            {
                infoText.text += $"\n{attribute.attributeName} : {attribute.value}";
            }
        }

    }

}
