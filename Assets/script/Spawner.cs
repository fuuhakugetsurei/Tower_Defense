using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get; private set; }
    public GameSettings gameSettings;
    public Button startButton;
    public TMP_Text levelText;
    public Button gotoWorkHouse;

    [SerializeField]
    private GameObject healthBarPrefab;  // 在 Inspector 中設置血條預製體

    public Transform[] waypoints;
    public float spawnInterval = 2f;

    private float timer = 0f;
    private bool isSpawning = false;
    private List<GameSettings.EnemyConfig> spawnList = new List<GameSettings.EnemyConfig>();
    private int level;
    private GameManager gameManager;
    private int maxEnemies;
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameSettings != null)
        {
            gameSettings.ResetLevel(); // 初始設為 0
        }
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartNextLevel);
            startButton.gameObject.SetActive(true);
            gotoWorkHouse.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("StartButton 未在 Inspector 中設置！");
        }

        LevelUpdate();
    }

    void Update()
    {
        if (!isSpawning || gameSettings.currentLevel == 0) return;

        timer += Time.deltaTime;

        maxEnemies = gameSettings.GetMaxEnemiesForLevel(gameSettings.currentLevel);
        if (timer >= spawnInterval && gameSettings.currentEnemyCount < maxEnemies && spawnList.Count > 0)
        {
            SpawnEnemy();
            timer = 0f;
        }

        if (gameSettings.currentEnemyCount == maxEnemies && GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            EndCurrentLevel();
        }
    }


    public void StartNextLevel()
    {
        if (gameManager.isUIShowing) return;

        if (gameSettings.currentLevel >= gameSettings.enemiesPerLevel.Count)
        {
            Debug.Log("所有關卡已完成！");
            if (startButton != null)
            {
                startButton.gameObject.SetActive(false);
                gotoWorkHouse.gameObject.SetActive(false);
            }
            return;
        }

        if (gameSettings.currentLevel == 0)
        {
            gameSettings.currentLevel = 1;
        }
        else
        {
            gameSettings.currentLevel++;
        }

        LevelUpdate();
        gameSettings.currentEnemyCount = 0;
        isSpawning = true;
        if (startButton != null)
        {
            startButton.gameObject.SetActive(false);
            gotoWorkHouse.gameObject.SetActive(false);
        }

        spawnList.Clear();
        var enemies = gameSettings.GetEnemiesForLevel(gameSettings.currentLevel);
        foreach (var enemyConfig in enemies)
        {
            for (int i = 0; i < enemyConfig.count; i++)
            {
                spawnList.Add(enemyConfig);
            }
        }

        Debug.Log("進入第 " + gameSettings.currentLevel + " 關，敵人數量: " + spawnList.Count);
    }
    void SpawnEnemy()
    {
        if (spawnList.Count == 0) return;

        int randomIndex = Random.Range(0, spawnList.Count);
        var enemyConfig = spawnList[randomIndex];
        spawnList.RemoveAt(randomIndex);

        GameObject enemy = Instantiate(enemyConfig.enemyPrefab, transform.position, Quaternion.identity);
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        SpeedBoostEnemy speedBoostEnemyScript = enemy.GetComponent<SpeedBoostEnemy>();
        if (enemyScript != null)
        {
            enemyScript.healthBarPrefab = healthBarPrefab;
            enemyScript.SetWaypoints(waypoints);
        }
        if (speedBoostEnemyScript != null)
        {
            speedBoostEnemyScript.healthBarPrefab = healthBarPrefab;
            speedBoostEnemyScript.SetWaypoints(waypoints);
        }

        gameSettings.currentEnemyCount++;
        Debug.Log("生成敵人: " + enemy.name + "，剩餘敵人數: " + spawnList.Count);
    }

    void EndCurrentLevel()
    {
        Debug.Log("第 " + gameSettings.currentLevel + " 關完成！");
        isSpawning = false;
        if (startButton != null)
        {
            if (gameSettings.currentLevel < gameSettings.enemiesPerLevel.Count)
            {
                startButton.gameObject.SetActive(true);
                gotoWorkHouse.gameObject.SetActive(true);
            }
            else
            {
                startButton.gameObject.SetActive(false);
                gotoWorkHouse.gameObject.SetActive(false);
            }
        }
    }

    public void LevelUpdate()
    {
        level = gameSettings.currentLevel;
        GameDataManager.Instance.level = level;
        if (levelText != null)
        {

            levelText.text = "Level: " + level.ToString();

        }
    }
    public void ResetLevel()
    {
        isSpawning = false;
        timer = 0f;
        spawnList.Clear();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        GameObject[] speedUpEnemies = GameObject.FindGameObjectsWithTag("SpeedUpEnemy");
        foreach (GameObject speedUpEnemy in speedUpEnemies)
        {
            Destroy(speedUpEnemy);
        }

        gameSettings.ResetLevel();
        LevelUpdate();

        if (startButton != null)
        {
            startButton.gameObject.SetActive(true);
            gotoWorkHouse.gameObject.SetActive(true);
        }

        GameDataManager.Instance.level = 0;
        Debug.Log("關卡已重置到初始狀態！");
    }
    public void BackToMain()
    {
        startButton.onClick.AddListener(StartNextLevel);
    }

    public bool IsSpawning() => isSpawning;
    public int GetLevel() => level;

}

