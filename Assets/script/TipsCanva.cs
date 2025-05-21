using UnityEngine;

public class TipsCanvas : MonoBehaviour
{
    private void Awake()
    {
        // 檢查是否已有相同 Canvas 存在（避免重複）
        if (FindObjectsByType<TipsCanvas>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}