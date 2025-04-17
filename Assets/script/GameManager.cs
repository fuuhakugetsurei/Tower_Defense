using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public GameSettings gameSettings;
    public TextAsset levelConfigFile;
    public bool isUIShowing = false;

    [SerializeField]
    private List<GameObject> enemyPrefabs = new List<GameObject>();
    private Dictionary<string, GameObject> enemyPrefabMap = new Dictionary<string, GameObject>();
    void Awake()
    {
        Application.runInBackground = true;
    }

    void Start()
    {
        if (gameSettings == null)
        {
            Debug.LogError("gameSettings 未正確設置！");
            return;
        }
        if (levelConfigFile == null)
        {
            Debug.LogError("levelConfigFile 未設置！請拖入 JSON 檔案！");
            return;
        }

        Debug.Log("JSON 內容: " + levelConfigFile.text);  // 輸出 JSON 內容

        foreach (var prefab in enemyPrefabs)
        {
            if (prefab != null)
            {
                enemyPrefabMap[prefab.name] = prefab;
            }
        }

        gameSettings.LoadFromJson(levelConfigFile.text, enemyPrefabMap);
        gameSettings.ResetEnemyCount();

        Debug.Log("遊戲初始化完成，關卡數：" + gameSettings.enemiesPerLevel.Count);

    }
    public void Restart()
    {
        Scene SampleScene = SceneManager.GetSceneByName("SampleScene");
        Scene WorkHouse = SceneManager.GetSceneByName("WorkHouse");
        Debug.Log($"SampleScene loaded: {SampleScene.isLoaded}");
        Debug.Log($"WorkHouse loaded: {WorkHouse.isLoaded}");
        GameObject[] towers = GameObject.FindGameObjectsWithTag("tower");
        foreach (var tower in towers)
        {
            Destroy(tower);
        }
        var towerPoints = Resources.FindObjectsOfTypeAll<TowerPlacementPoint>();
        foreach (var tp in towerPoints)
        {
            tp.gameObject.SetActive(true);
            tp.isOccupied = false;
        }
        Spawner.Instance.ResetLevel();
        Castle.Instance.Restart();
        LuckyManager.Instance.setup();
        CoinManager.Instance.setup();
        WorkHouseGameManager.Instance.Restart();
    }

}