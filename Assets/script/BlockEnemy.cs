using UnityEngine;
using TMPro; // 使用 TextMeshPro
using DG.Tweening; // 使用 DOTween
using System;

public class BlockEnemy : BaseEnemy
{
    private LuckyManager luckyManager;
    private float probability;
    [SerializeField] private TextMeshProUGUI blockTextPrefab; // 文字預製件

    protected override void Start()
    {
        luckyManager = FindFirstObjectByType<LuckyManager>();
        healthBarsCanvas = GameObject.Find("HealthBarsCanvas")?.GetComponent<Canvas>();
        maxHealth *= (float)Math.Pow(1.45f, gameSettings.currentLevel - 1);
        base.Start();

        // 初始化 DOTween（可選，通常自動完成）
        DOTween.Init();
    }

    public override void TakeDamage(float damage)
    {
        if (!isInitialized) return;
        probability = Mathf.Min(((100 - luckyManager.GetLucky()) / 2f) + (damage * 0.1f), 70f);

        if (IsBlock(probability))
        {
            //Debug.Log("格檔 triggered, calling ShowBlockText");
            ShowBlockText(); // 顯示格檔文字
        }
        else
        {
            currentHealth -= damage;
            targetHealth = currentHealth; // 更新目標血量
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void ShowBlockText()
    {


        // 生成文字物件
        TextMeshProUGUI blockText = Instantiate(blockTextPrefab, healthBarsCanvas.transform);
        blockText.text = "格檔!";
        blockText.alpha = 1f; // 確保初始透明度為 1

        // 設置 RectTransform 屬性
        RectTransform textRect = blockText.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f); // 錨點設為中心
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = new Vector2(100f, 50f); // 設置合理大小

        // 計算敵人頭頂的世界座標
        Vector3 offset = Vector3.up * 1f; // 比血條高 1.5 單位
        Vector3 worldPoint = transform.position + offset;
        Debug.Log($"Enemy world position: {transform.position}, Text world position: {worldPoint}");

        // 將世界座標轉換為螢幕座標
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPoint);
        Debug.Log($"Screen point: {screenPoint} (Screen size: {Screen.width}x{Screen.height})");

        // 如果 Canvas 是 Screen Space - Overlay，直接使用螢幕座標
        if (healthBarsCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            textRect.position = screenPoint;
            Debug.Log($"Text positioned at screen point: {screenPoint}");
        }

        // 使用 DOTween 創建動畫
        float duration = 2f; // 動畫持續時間（秒）
        float moveDistance = 50f; // 向上移動的像素距離
        Vector2 startPos = textRect.anchoredPosition;

        textRect
            .DOAnchorPosY(startPos.y + moveDistance, duration) // 向上移動
            .SetEase(Ease.Linear)
            .SetUpdate(UpdateType.Normal, true); // 不受 Time.timeScale 影響

        blockText
            .DOFade(0f, duration) // 淡出
            .SetEase(Ease.Linear)
            .SetUpdate(UpdateType.Normal, true)
            .OnComplete(() => Destroy(blockText.gameObject));
    }

    public bool IsBlock(float probability)
    {
        int roll = UnityEngine.Random.Range(0, 100);
        Debug.Log($"Block roll: {roll}, probability: {probability}");
        return roll < probability;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        // 清理與該物件相關的 DOTween 動畫（可選）
        DOTween.KillAll(gameObject);
    }
}