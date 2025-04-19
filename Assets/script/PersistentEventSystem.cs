using UnityEngine;
using UnityEngine.EventSystems;

public class PersistentEventSystem : MonoBehaviour
{
    private static PersistentEventSystem instance;

    void Awake()
    {
        // 確保只有一個 PersistentEventSystem 存在
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 使物件在場景切換時不被銷毀
        }
        else
        {
            Destroy(gameObject); // 如果已經存在一個實例，銷毀後來的
        }
    }
}