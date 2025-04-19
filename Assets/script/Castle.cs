using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Castle : MonoBehaviour
{
    public static Castle Instance { get; private set; }
    public int level { get; private set; } = 1;
    public int maxLevel { get; private set; } = 20;
    public int upgradePrice { get; private set; } = 1000;
    public int maxHealth { get; private set; } = 100;
    private int currentHealth;
    private float targetHealth;
    private float displayedHealth;
    public TMP_Text HealthValueText; // 血量文字，在 CastleHealthValue 畫布
    public GameObject healthBarPrefab;
    private Slider healthBar;
    private Canvas healthBarsCanvas;
    private RectTransform healthBarsCanvasRect;
    private Canvas castleHealthCanvas; // CastleHealthValue 畫布
    private RectTransform castleHealthCanvasRect;
    private GameManager gameManager;
    private Camera mainCam;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        mainCam = Camera.main;
        gameManager = FindFirstObjectByType<GameManager>();
        currentHealth = maxHealth;
        targetHealth = maxHealth;
        displayedHealth = maxHealth;

        // 初始化血條 Canvas
        GameObject healthCanvasObj = GameObject.Find("HealthBarsCanvas");
        if (healthCanvasObj != null)
        {
            healthBarsCanvas = healthCanvasObj.GetComponent<Canvas>();
            healthBarsCanvasRect = healthCanvasObj.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("找不到 HealthBarsCanvas，請檢查場景設置");
        }

        // 初始化血條
        if (healthBarPrefab != null && healthBarsCanvas != null)
        {
            GameObject healthBarObj = Instantiate(healthBarPrefab, healthBarsCanvas.transform);
            healthBar = healthBarObj.GetComponent<Slider>();
            if (healthBar != null)
            {
                healthBar.maxValue = maxHealth;
                healthBar.minValue = 0f;
                healthBar.value = currentHealth;
                // 確保血條可見
                healthBar.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("healthBarPrefab 未包含 Slider 組件");
            }
        }
        else
        {
            Debug.LogError("healthBarPrefab 或 HealthBarsCanvas 未設置");
        }

        // 初始化文字 Canvas
        GameObject textCanvasObj = GameObject.Find("CastleHealthValue");
        if (textCanvasObj != null)
        {
            castleHealthCanvas = textCanvasObj.GetComponent<Canvas>();
            castleHealthCanvasRect = textCanvasObj.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("找不到 CastleHealthValue 畫布，請檢查場景設置");
        }

        if (HealthValueText != null)
        {
            UpdateHealthText(); // 初始化文字顯示
        }
        else
        {
            Debug.LogError("HealthValueText 未賦值，請在 Inspector 中設置");
        }

        UpdateHealthBarPosition(); // 初始血條位置
        UpdateHealthTextPosition(); // 初始文字位置
    }

    void Update()
    {
        UpdateHealthBarPosition(); // 更新血條位置
        UpdateHealthTextPosition(); // 更新文字位置
        UpdateHealthBar(); // 更新血條和文字值
    }

    public void hurt(int damage)
    {
        currentHealth -= damage;
        targetHealth = currentHealth;
        if (currentHealth <= 0)
        {
            targetHealth = 0;
            Gameover();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null) return;

        // 平滑插值血條值
        float lerpSpeed = 10f; // 變化速度（約 0.1 秒）
        displayedHealth = Mathf.Lerp(displayedHealth, targetHealth, lerpSpeed * Time.deltaTime);
        healthBar.value = displayedHealth;

        // 確保精確到達目標值
        if (Mathf.Abs(displayedHealth - targetHealth) < 0.01f)
        {
            displayedHealth = targetHealth;
            healthBar.value = targetHealth;
        }

        // 更新文字顯示
        UpdateHealthText();
    }

    private void UpdateHealthBarPosition()
    {
        if (healthBar == null || mainCam == null || healthBarsCanvas == null || healthBarsCanvasRect == null)
        {
            Debug.LogWarning("血條位置更新失敗：缺少必要組件");
            return;
        }

        Vector3 offset = Vector3.up * 2f;
        Vector2 screenPoint = mainCam.WorldToScreenPoint(transform.position + offset);

        // 嘗試使用畫布本地坐標
        if (healthBarsCanvas.renderMode == RenderMode.ScreenSpaceCamera &&
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                healthBarsCanvasRect, screenPoint, healthBarsCanvas.worldCamera ?? mainCam, out Vector2 localPoint))
        {
            healthBar.GetComponent<RectTransform>().anchoredPosition = localPoint;
            Debug.Log($"血條本地坐標：{localPoint}");
        }
        else
        {
            // 回退到原始螢幕坐標（若本地坐標失敗）
            healthBar.transform.position = screenPoint;
            Debug.Log($"血條螢幕坐標：{screenPoint}");
        }
    }

    private void UpdateHealthTextPosition()
    {
        if (HealthValueText == null || mainCam == null || castleHealthCanvas == null)
        {
            Debug.LogWarning("文字位置更新失敗：缺少必要組件");
            return;
        }

        Vector3 offset = Vector3.up * 2.5f; // 文字略高於血條
        Vector2 screenPoint = mainCam.WorldToScreenPoint(transform.position + offset);
        RectTransform textRect = HealthValueText.GetComponent<RectTransform>();

        if (castleHealthCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Overlay 模式：考慮 CanvasScaler 縮放
            float scaleFactor = castleHealthCanvas.scaleFactor;
            if (scaleFactor != 0)
            {
                textRect.position = screenPoint / scaleFactor;
                Debug.Log($"文字 Overlay 坐標：{textRect.position}");
            }
        }
        else if (castleHealthCanvas.renderMode == RenderMode.ScreenSpaceCamera && castleHealthCanvasRect != null)
        {
            // Camera 模式：轉換為本地坐標
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                castleHealthCanvasRect, screenPoint, castleHealthCanvas.worldCamera ?? mainCam, out Vector2 localPoint))
            {
                textRect.anchoredPosition = localPoint;
                Debug.Log($"文字本地坐標：{localPoint}");
            }
        }
    }

    private void UpdateHealthText()
    {
        if (HealthValueText == null) return;

        // 使用 displayedHealth，與血條同步
        HealthValueText.text = $"{Mathf.RoundToInt(displayedHealth)} / {maxHealth}";
    }

    public void Gameover()
    {
        displayedHealth = 0;
        if (healthBar != null)
            healthBar.value = 0;
        if (HealthValueText != null)
            HealthValueText.text = $"0 / {maxHealth}";
        Debug.Log("GAMEOVER");
        gameManager.Restart();
    }

    public void Upgrade()
    {
        if (level < maxLevel)
        {
            level++;
            UpdatePrice();
            maxHealth += 100;
            currentHealth += 100;
            targetHealth = currentHealth;
            displayedHealth = currentHealth;
            if (healthBar != null)
                healthBar.maxValue = maxHealth;
            UpdateHealthText(); // 立即更新文字
            Debug.Log($"城堡升級到 Lv{level}，最大血量: {maxHealth}");
        }
    }

    public void Restart()
    {
        level = 1;
        upgradePrice = 1000;
        maxHealth = 100;
        currentHealth = maxHealth;
        targetHealth = maxHealth;
        displayedHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
        UpdateHealthText(); // 立即更新文字
    }

    private void UpdatePrice()
    {
        upgradePrice += 1000;
    }

    public void Hide()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
    }

    public void Show()
    {
        gameObject.GetComponent<Renderer>().enabled = true;
    }
}