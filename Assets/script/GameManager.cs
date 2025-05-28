using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public Button start;
    public TMP_Text startText;
    public Button tenSpeedButton;
    public Image image;
    public GameSettings gameSettings;
    public TextAsset levelConfigFile;
    public bool isUIShowing = false;
    private int speed = 1;
    private TMP_Text buttonText;
    [SerializeField]
    private List<GameObject> enemyPrefabs = new List<GameObject>();
    private Dictionary<string, GameObject> enemyPrefabMap = new Dictionary<string, GameObject>();
    [SerializeField] private Sprite[] sprites;

    void Awake()
    {
        Application.runInBackground = true;
        QualitySettings.vSyncCount = 0; 
    }

    void Start()
    {
        isUIShowing = true;
        start.gameObject.SetActive(true);
        tenSpeedButton.onClick.AddListener(() => speedup());
        Image gameoverSprite = start.gameObject.GetComponent<Image>();
        start.onClick.AddListener(() =>
                                    {
                                        startText.gameObject.SetActive(false);
                                        gameoverSprite.DOFade(0f, 1f)
                                                        .SetEase(Ease.InOutSine)
                                                        .SetUpdate(UpdateType.Normal,true)
                                                        .OnComplete(() =>
                                                         {
                                                            start.gameObject.SetActive(false);
                                                        });
                                        
                                        isUIShowing = false;
                                    });
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
    public void quit()
    {
        Application.Quit();
    }
    private void speedup()
    {
        if (buttonText != null)
        {
            if (speed == 1)
            {
                image.sprite = sprites[0];
                speed = 10;
                buttonText.text = "10x";
            }
            else
            {
                image.sprite = sprites[1];
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