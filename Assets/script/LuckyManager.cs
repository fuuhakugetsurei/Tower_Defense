using UnityEngine;
using TMPro;

public class LuckyManager : MonoBehaviour
{
    public GameSettings gameSettings;  
    public TMP_Text luckyText;                 
    private int luckyPoint;

    public static LuckyManager Instance { get; private set; }
    
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        setup();
        UpdateUI();
    }
    
    public void AddLucky(int amount)
    {
        luckyPoint += amount;
        UpdateUI();
    }

    
    public bool SpendLucky(int amount)
    {
        if (luckyPoint >= amount)
        {
            luckyPoint -= amount;
            UpdateUI();
            return true;
        }
        else
        {
            Debug.Log("你太雖了");
            return false;
        }
    }

    
    void UpdateUI()
    {
        if (luckyText != null)
        {
            luckyText.text = "luckypoint: " + luckyPoint.ToString();
            GameDataManager.Instance.lucky = luckyPoint;
        }
    }
    public int GetLucky() => luckyPoint; 
    
    public bool RollLucky()
    {
        int roll = Random.Range(0, 100);         
        return roll < luckyPoint;
    }
    public void setup()
    {
        luckyPoint = gameSettings.luckyPoint;
        GameDataManager.Instance.lucky = luckyPoint;
        UpdateUI();
    }
}
