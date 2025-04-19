using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class SceneController : MonoBehaviour
{
    private string currentActiveScene; // 記錄當前顯示的場景
    private WorkHouseGameManager workHouseGameManager;
    private GameManager gameManager;

    void Start()
    {
        // 初始化時加載場景 A 和 B
        LoadScenes();
    }

    private void LoadScenes()
    {
        // 假設場景 A 是初始場景，已經在編輯器中加載
        currentActiveScene = "SampleScene";
        gameManager = FindFirstObjectByType<GameManager>();

        // 以 Additive 模式加載場景 B
        SceneManager.LoadSceneAsync("WorkHouse", LoadSceneMode.Additive);

        // 等場景 B 加載完成後，隱藏它
        StartCoroutine(SetSceneActiveAfterLoad("WorkHouse", false));
    }

    // 切換場景
    public void SwitchScene(string targetSceneName)
    {
        if (gameManager.isUIShowing) return;
        if (targetSceneName == currentActiveScene) return;
        if (targetSceneName == "WorkHouse") Castle.Instance.Hide();
        if (targetSceneName == "SampleScene") Castle.Instance.Show();

        // 隱藏當前場景
        SetSceneActive(currentActiveScene, false);

        // 顯示目標場景
        SetSceneActive(targetSceneName, true);

        // 更新當前場景
        currentActiveScene = targetSceneName;
    }

    // 設置場景的啟用/禁用狀態
    private void SetSceneActive(string sceneName, bool isActive)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (scene.isLoaded)
        {
            // 遍歷場景中的所有根物件
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                // 排除 GlobalEventSystem，防止被禁用
                if (rootObject.name != "GlobalEventSystem")
                {
                    rootObject.SetActive(isActive);
                }
            }

            if (isActive && sceneName == "WorkHouse")
            {
                UpdateSceneBUI(scene);
            }
        }
    }

    private void UpdateSceneBUI(Scene scene)
    {
        workHouseGameManager = FindFirstObjectByType<WorkHouseGameManager>();
        if (workHouseGameManager == null)
        {
            Debug.LogError("WorkHouseGameManager not found in scene: " + scene.name);
            return;
        }
        Debug.Log("Updating UI for WorkHouse");
        workHouseGameManager.UpdateUI();
    }

    // 協程：等待場景加載完成後設置啟用/禁用
    private IEnumerator SetSceneActiveAfterLoad(string sceneName, bool isActive)
    {
        // 等待場景加載完成
        while (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            yield return null;
        }
        SetSceneActive(sceneName, isActive);
    }
}