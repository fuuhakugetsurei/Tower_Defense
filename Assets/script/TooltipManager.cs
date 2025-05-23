using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private float displayTime = 0.3f; // 已改為 0.3 秒
    [SerializeField] private RectTransform canvasRect;
    private RectTransform tooltipRect;
    private CanvasGroup canvasGroup;
    private bool isShowing;
    private bool isHiding; // 追蹤是否正在執行隱藏動畫

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        canvasGroup = tooltipPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = tooltipPanel.AddComponent<CanvasGroup>();
        }
        tooltipPanel.SetActive(false);
    }

    public void ShowTooltip(string message)
    {
        // 終止所有動畫和計時器
        StopAllCoroutines(); // 停止所有 Coroutine
        tooltipRect.DOKill();
        canvasGroup.DOKill();

        // 重置狀態
        tooltipText.text = message;
        tooltipPanel.SetActive(true);
        isShowing = true;
        isHiding = false;
        canvasGroup.alpha = 1f;
        tooltipRect.localScale = Vector3.one;
        tooltipRect.anchoredPosition = Vector2.zero;

        // 強制更新文字佈局
        tooltipText.ForceMeshUpdate();

        // 使用 Coroutine 控制隱藏（不受 Time.timeScale 影響）
        StartCoroutine(HideTooltipAfterDelay(displayTime));
    }

    private IEnumerator HideTooltipAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // 使用 WaitForSecondsRealtime
        HideTooltip();
    }

    public void HideTooltip()
{
    if (!isShowing || isHiding) // 防止重複或被新提示中斷
    {
        return;
    }

    isHiding = true;
    isShowing = false;

    // 動畫：向上移動 + 淡出
    float moveDistance = 50f; // 向上移動 50 單位
    float animationDuration = 0.5f; // 動畫持續時間
    Vector2 targetPos = tooltipRect.anchoredPosition + new Vector2(0, moveDistance);
    
    // 使用 unscaledTime 確保動畫不受 Time.timeScale 影響
    canvasGroup.DOFade(0f, animationDuration).SetEase(Ease.InQuad).SetUpdate(UpdateType.Normal, true);
    tooltipRect.DOAnchorPos(targetPos, animationDuration).SetEase(Ease.InQuad).SetUpdate(UpdateType.Normal, true).OnComplete(() =>
    {
        tooltipPanel.SetActive(false);
        // 重置狀態以備下次使用
        tooltipRect.anchoredPosition = Vector2.zero;
        canvasGroup.alpha = 1f;
        tooltipRect.localScale = Vector3.one;
        isHiding = false; // 完成隱藏
    });
}
}