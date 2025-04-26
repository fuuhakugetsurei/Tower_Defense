using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Button tenSpeedButton;
    public GameSettings gameSettings;
    public TextAsset levelConfigFile;
    public bool isUIShowing = false;
    private int speed = 1;
    private TMP_Text buttonText;
    [SerializeField]
    private List<GameObject> enemyPrefabs = new List<GameObject>();
    private Dictionary<string, GameObject> enemyPrefabMap = new Dictionary<string, GameObject>();
    void Awake()
    {
        Application.runInBackground = true;
    }

    void Start()
    {
        tenSpeedButton.onClick.AddListener(() => speedup());
        buttonText = tenSpeedButton.GetComponentInChildren<TMP_Text>();
        buttonText.text = "1x";
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
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("退出遊戲");
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
    private void speedup()
    {
        if (buttonText != null)
        {
            if (speed == 1)
            {
                speed = 10;
                buttonText.text = "10x";
            }
            else
            {
                speed = 1;
                buttonText.text = "1x";
            }
        }
        else
        {
            Debug.LogError("tenSpeedButton is not assigned in the Inspector.");
        }
        Time.timeScale = speed;
    }
}