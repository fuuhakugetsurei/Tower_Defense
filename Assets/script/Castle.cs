using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Castle : MonoBehaviour
{
    public static Castle Instance { get; private set;}
    public int level {get;private set;} = 1;
    public int maxLevel {get;private set;} = 20;
    public int upgradePrice {get;private set;} = 1000;
    public int maxHealth {get;private set;} = 100;
    private int currentHealth;
    public GameObject healthBarPrefab;
    private Slider healthBar;
    private Canvas healthBarsCanvas;
    private GameManager gameManager;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        currentHealth = maxHealth;
        GameObject canvasObj = GameObject.Find("HealthBarsCanvas");
        if (canvasObj != null)
        {
            healthBarsCanvas = canvasObj.GetComponent<Canvas>();
        }
        GameObject healthBarObj = Instantiate(healthBarPrefab, healthBarsCanvas.transform);
        healthBar = healthBarObj.GetComponent<Slider>();
        Vector3 offset = Vector3.up * 2f;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(transform.position + offset);
        healthBar.transform.position = screenPoint;
        healthBar.maxValue = maxHealth;
        healthBar.minValue = 0f;
        healthBar.value = currentHealth;
    }

    public void hurt(int damage)
    {
        if (damage < currentHealth)
        {
            currentHealth -= damage;    
            StartCoroutine(UpdateHealthBar());
        }
        else
        {
            currentHealth -= damage;    
            Gameover();
        }
    }
    
    protected IEnumerator UpdateHealthBar()
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
    public void Gameover()
    {
        healthBar.value = 0;
        //Time.timeScale = 0f;
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
        }
    }
    public void Restart()
    {
        level = 1;  
        upgradePrice = 1000;
        maxHealth = 100;
        currentHealth = maxHealth;
        healthBar.value = currentHealth;
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
