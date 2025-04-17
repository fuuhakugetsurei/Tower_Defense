using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    public int gold;
    public int lucky;
    public int level;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 場景切換不會被銷毀
        }
        else
        {
            Destroy(gameObject); // 保證只會有一個
        }
    }
}
