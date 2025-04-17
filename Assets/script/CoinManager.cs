using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set;}

    public GameSettings gameSettings;  // 引用 GameSettings
    public TMP_Text goldText;                 // 顯示金幣的 UI Text

    private int gold;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        // 從 GameSettings 讀取初始金幣數量
        setup();
        UpdateGoldUI();
    }

    // 獲得金幣
    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldUI();
    }

    // 花費金幣
    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            UpdateGoldUI();
            return true;
        }
        else
        {
            Debug.Log("金幣不足！");
            return false;
        }
    }

    // 更新金幣顯示
    void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = "coin: " + gold.ToString();
            GameDataManager.Instance.gold = gold;

        }
    }

    public void setup()
    {
        gold = gameSettings.initialGold;
        GameDataManager.Instance.gold = gold;
        UpdateGoldUI();
    }
    // 取得當前金幣數量
    public int GetGold() => gold;
}

