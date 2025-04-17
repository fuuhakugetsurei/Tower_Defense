using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpeedBoostEnemy : BaseEnemy
{
    private float speedBoostInterval = 3f;
    private float speedBoostTimer = 0f;
    private float speedBoostPercent = 0.5f;
    private bool isPaused = false;

    protected override void Update()
    {
        if (!isPaused)
        {
            base.Move(); // 只在沒暫停時移動
        }

        UpdateHealthBarPosition();

        speedBoostTimer += Time.deltaTime;
        if (speedBoostTimer >= speedBoostInterval)
        {
            StartCoroutine(BoostSpeedAndPause());
            speedBoostTimer = 0f;
        }
    }

    private IEnumerator BoostSpeedAndPause()
    {
        isPaused = true;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObj in enemies)
        {
            if (enemyObj.CompareTag("SpeedUpEnemy")) continue; // 跳過自己
            BaseEnemy enemy = enemyObj.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.speed *= (1f + speedBoostPercent);
            }
        }

        yield return new WaitForSeconds(1f);
        isPaused = false;
    }
}
