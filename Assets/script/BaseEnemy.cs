using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public abstract class BaseEnemy : MonoBehaviour
{
    public float speed = 2f;
    public float maxHealth = 100f;
    protected float currentHealth;
    protected Transform[] waypoints;
    protected int waypointIndex = 0;
    protected CoinManager coinManager;
    public int coinDrop;
    public int ATK = 10;
    public GameSettings gameSettings;

    public GameObject healthBarPrefab;
    protected Slider healthBar;
    protected Canvas healthBarsCanvas;
    protected bool isInitialized = false;
    protected Castle castle;
    protected Camera mainCam;  // 快取主攝影機
    protected float targetHealth; // 新增：追蹤血條的目標值
    private float displayedHealth; // 新增：當前顯示的血條值
    private bool isSlowed = false;
    private Coroutine slowCoroutine;
    private float originalSpeed; // 新增：記錄原始速度

    protected virtual void Start()
    {
        originalSpeed = speed; // 記錄原始速度  
        mainCam = Camera.main;
        coinManager = FindFirstObjectByType<CoinManager>();
        castle = FindFirstObjectByType<Castle>();
        currentHealth = maxHealth;
        targetHealth = maxHealth; // 初始化目標血量
        displayedHealth = maxHealth; // 初始化顯示血量
        GameObject canvasObj = GameObject.Find("HealthBarsCanvas");
        if (canvasObj != null)
        {
            healthBarsCanvas = canvasObj.GetComponent<Canvas>();
        }

        if (healthBarPrefab != null && healthBarsCanvas != null)
        {
            GameObject healthBarObj = Instantiate(healthBarPrefab, healthBarsCanvas.transform);
            healthBarObj.SetActive(false); // 先隱藏
            UpdateHealthBarPosition();
            healthBar = healthBarObj.GetComponent<Slider>();
            healthBar.maxValue = maxHealth;
            healthBar.minValue = 0f;
            healthBar.value = currentHealth;
            StartCoroutine(EnableHealthBarNextFrame(healthBarObj));
        }

        isInitialized = true;
    }

    private IEnumerator EnableHealthBarNextFrame(GameObject hb)
    {
        yield return null; // 等一幀
        hb.SetActive(true);
    }

    protected virtual void Update()
    {
        Move();
        UpdateHealthBarPosition();
        UpdateHealthBar(); // 新增：每幀平滑更新血條
    }

    public void SetWaypoints(Transform[] wp)
    {
        waypoints = wp;
    }

    protected virtual void Move()
    {
        if (waypoints == null || waypointIndex >= waypoints.Length) return;

        Vector2 targetPosition = waypoints[waypointIndex].position;
        float effectiveDeltaTime = Time.unscaledDeltaTime * Time.timeScale; // 考慮 timeScale
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * effectiveDeltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            waypointIndex++;
            if (waypointIndex >= waypoints.Length)
            {
                ReachGoal();
            }
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (!isInitialized) return;

        currentHealth -= damage;
        targetHealth = currentHealth; // 更新目標血量
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void UpdateHealthBar()
    {
        if (healthBar == null) return;

        // 平滑插值血條值
        float lerpSpeed = 10f; // 控制變化速度（越大越快，相當於 0.1 秒完成）
        displayedHealth = Mathf.Lerp(displayedHealth, targetHealth, lerpSpeed * Time.deltaTime);
        healthBar.value = displayedHealth;

        // 如果血量已達目標值，確保精確
        if (Mathf.Abs(displayedHealth - targetHealth) < 0.01f)
        {
            displayedHealth = targetHealth;
            healthBar.value = targetHealth;
        }
    }

    protected virtual void UpdateHealthBarPosition()
    {
        if (healthBar != null && mainCam != null)
        {
            Vector3 offset = Vector3.up * 1f;
            Vector2 screenPoint = mainCam.WorldToScreenPoint(transform.position + offset);
            healthBar.transform.position = screenPoint;
        }
    }

    protected virtual void Die()
    {
        coinManager.AddGold(coinDrop);
        Destroy(gameObject);
    }

    protected virtual void ReachGoal()
    {
        castle.hurt(ATK);
        Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (healthBar != null && healthBar.gameObject != null)
        {
            Destroy(healthBar.gameObject);
        }
    }

    public float GetCurrentHealth() => currentHealth;

    private float slowEndTime = 0f;  // 新增：記錄減速效果結束時間
    
    public void ApplySlow(float slowAmount, float duration)
    {
        float currentTime = Time.time;
        
        // 更新結束時間（取較晚的時間）
        slowEndTime = Mathf.Max(currentTime + duration, slowEndTime);
    
        // 如果還沒被減速，才套用減速效果
        if (!isSlowed)
        {
            isSlowed = true;
            float originalSpeed = speed;
            speed = originalSpeed * (1f - slowAmount);
            
            // 改變顏色表示被冰凍
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color originalColor = spriteRenderer.color;
                spriteRenderer.color = new Color(originalColor.r * 0.5f, originalColor.g * 0.7f, originalColor.b + 0.5f, originalColor.a);
            }
        }
    
        // 確保協程在運行
        if (slowCoroutine == null)
        {
            slowCoroutine = StartCoroutine(SlowCoroutine(slowAmount));
        }
    }
    
    private IEnumerator SlowCoroutine(float slowAmount)
    {
        while (Time.time < slowEndTime)
        {
            yield return null;
        }
    
        // 解除減速效果
        isSlowed = false;
        speed = originalSpeed;  // 設定回原始速度
        
        // 恢復原始顏色
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
        
        slowCoroutine = null;
    }
}