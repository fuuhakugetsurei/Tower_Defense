using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
public class GameSettings : ScriptableObject
{
    [System.Serializable]
    public class EnemyConfig
    {
        public GameObject enemyPrefab;
        public int count;
    }

    [System.Serializable]
    public class JsonEnemyConfig
    {
        public string enemyType;
        public int count;
    }

    [SerializeField]
    public List<List<EnemyConfig>> enemiesPerLevel = new List<List<EnemyConfig>>();

    public int currentLevel = 1;
    public int currentEnemyCount = 0;
    public int initialGold = 100;  // 初始金幣數量
    public int luckyPoint = 50; //初始幸運值

    private void OnValidate()
    {
        if (currentLevel < 1) currentLevel = 1;
        while (enemiesPerLevel.Count < currentLevel)
            enemiesPerLevel.Add(new List<EnemyConfig>());
        while (enemiesPerLevel.Count > currentLevel)
            enemiesPerLevel.RemoveAt(enemiesPerLevel.Count - 1);
    }

    public void LoadFromJson(string json, Dictionary<string, GameObject> enemyPrefabMap)
    {
        enemiesPerLevel.Clear();
        Debug.Log("載入 JSON: " + json);

        // 使用 Newtonsoft.Json 解析完整 JSON
        var levelData = JsonConvert.DeserializeObject<LevelData>(json);
        if (levelData == null || levelData.levels == null)
        {
            Debug.LogError("JSON 解析失敗，levelData 為 null");
            return;
        }

        foreach (var jsonLevel in levelData.levels)
        {
            List<EnemyConfig> levelConfig = new List<EnemyConfig>();
            foreach (var jsonEnemy in jsonLevel)
            {
                if (enemyPrefabMap.ContainsKey(jsonEnemy.enemyType))
                {
                    levelConfig.Add(new EnemyConfig
                    {
                        enemyPrefab = enemyPrefabMap[jsonEnemy.enemyType],
                        count = jsonEnemy.count
                    });
                }
                else
                {
                    Debug.LogError($"敵人類型 {jsonEnemy.enemyType} 未找到對應的預製體！");
                }
            }
            enemiesPerLevel.Add(levelConfig);
        }
    }

    [System.Serializable]
    private class LevelData
    {
        public List<List<JsonEnemyConfig>> levels;
    }

    public void SetEnemiesForLevel(int level, List<EnemyConfig> enemyConfigs)
    {
        if (level < 1) return;
        while (enemiesPerLevel.Count < level)
        {
            enemiesPerLevel.Add(new List<EnemyConfig>());
        }
        enemiesPerLevel[level - 1] = enemyConfigs;
    }

    public List<EnemyConfig> GetEnemiesForLevel(int level)
    {
        if (level <= 0 || level > enemiesPerLevel.Count)
        {
            Debug.LogWarning($"指定的關卡 {level} 無效，返回空敵人列表！");
            return new List<EnemyConfig>();
        }
        return enemiesPerLevel[level - 1];
    }

    public int GetMaxEnemiesForLevel(int level)
    {
        var enemies = GetEnemiesForLevel(level);
        int total = 0;
        foreach (var enemy in enemies)
        {
            total += enemy.count;
        }
        return total;
    }

    public void ResetEnemyCount()
    {
        currentEnemyCount = 0;
    }

    public void ResetLevel()
    {
        currentLevel = 0;
        currentEnemyCount = 0;
    }

}

