using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorkHouseGameManager : MonoBehaviour
{
    public static WorkHouseGameManager Instance { get; private set; }
    public TMP_Text levelText;
    public TMP_Text luckyText;
    public TMP_Text goldText;
    private SceneController sceneController;
    public bool isUIShowing = false;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        // 找到名為 "SceneManager" 的物件
        GameObject sceneManager = GameObject.Find("Spawner");
        if (sceneManager != null)
        {
            sceneController = sceneManager.GetComponent<SceneController>();
        }

        UpdateUI();

    }
    public void BackToMainScene()
    {
        if (sceneController != null && isUIShowing == false)
        {
            FindFirstObjectByType<SceneController>().SwitchScene("SampleScene");
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateUI()
    {
        switch (GameDataManager.Instance.gold)
        {
            case >= 1000000000:
                goldText.text = "coin: " + (GameDataManager.Instance.gold / 1000000000f).ToString("F1") + "B"; ;
                break;
            case >= 1000000:
                goldText.text = "coin: " + (GameDataManager.Instance.gold / 1000000f).ToString("F1") + "M"; ;
                break;
            case >= 1000:
                goldText.text = "coin: " + (GameDataManager.Instance.gold / 1000f).ToString("F1") + "K"; ;
                break;
            default:
                goldText.text = "coin: " + GameDataManager.Instance.gold.ToString("F0"); ;
                break;
        }
        luckyText.text = "luckypoint: " + GameDataManager.Instance.lucky.ToString();
        levelText.text = "Level: " + GameDataManager.Instance.level.ToString();
    }

    public void Restart()
    {
        FindFirstObjectByType<SceneController>().SwitchScene("WorkHouse");
        UpdateUI();
        GameObject[] towers = GameObject.FindGameObjectsWithTag("tower");
        foreach (var tower in towers)
        {
            Destroy(tower);
        }
        // 找出所有 TowerSet（包含 inactive）
        var towerSets = Resources.FindObjectsOfTypeAll<TowerSet>();
        foreach (var ts in towerSets)
        {
            ts.gameObject.SetActive(true);
            ts.isOccupied = false;
        }
        FindFirstObjectByType<SceneController>().SwitchScene("SampleScene");
    }
}
