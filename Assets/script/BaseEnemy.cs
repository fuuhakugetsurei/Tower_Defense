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

    protected virtual void Start()
    {
        mainCam = Camera.main;

        maxHealth *= (float)Math.Pow(1.5f, gameSettings.currentLevel - 1);
        coinManager = FindFirstObjectByType<CoinManager>();
        castle = FindFirstObjectByType<Castle>();
        currentHealth = maxHealth;

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
    }

    public void SetWaypoints(Transform[] wp)
    {
        waypoints = wp;
    }

    protected virtual void Move()
    {
        if (waypoints == null || waypointIndex >= waypoints.Length) return;

        Vector2 targetPosition = waypoints[waypointIndex].position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

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
        if (currentHealth <= 0)
        {
            Die();
        }
        if (healthBar != null)
        {
            StartCoroutine(UpdateHealthBar());
        }
    }

    protected virtual IEnumerator UpdateHealthBar()
    {
        if (healthBar == null) yield break;

        float startValue = healthBar.value;
        float endValue = currentHealth;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            healthBar.value = Mathf.Lerp(startValue, endValue, elapsed / duration);
            yield return null;
        }
        healthBar.value = endValue;
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
}
