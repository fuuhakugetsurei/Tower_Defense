using UnityEngine;
using TMPro;


public class ImageScroller : MonoBehaviour
{
    public RectTransform imageHolder;
    public float imageWidth = 200f;  // 一張圖片的寬度
    private int currentIndex = 0;
    public int maxIndex = 0;  // 圖片總數 - 1
    public TMP_Text infoText;
    public TowerInfo[] towerInfos; // 這是從 ScriptableObject 獲取的塔的資訊

    private TowerManager towerManager;
    
    void Start()
    {
        towerManager = FindFirstObjectByType<TowerManager>();
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
        TowerInfo data = towerInfos[currentIndex];
        if (infoText != null)
        {
            if (currentIndex == 0)
            {
                infoText.text = $"{data.towerName} :\n" +
                                $"等級上限 : {data.maxlevel}\n" +
                                $"攻擊速度 : {(data.attackSpeed)*0.8}" + " ~ " + $"{(data.attackSpeed)*1.2}" + "\n" +
                                $"攻擊範圍 : {data.attackRange}\n" +
                                $"傷害 : {(data.damage)*0.8}" + " ~ " + $"{(data.damage)*1.2}" + "\n" +
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
