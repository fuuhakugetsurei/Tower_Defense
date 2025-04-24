using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class SpeedBoostEnemy : BaseEnemy
{
    private float speedBoostInterval = 3f;
    private float speedBoostTimer = 0f;
    private float speedBoostPercent = 0.5f;
    private bool isPaused = false;
    public int magicPerLife = 3;//每個生命最多施幾次魔法

    protected override void Start()
    {
        maxHealth *= (float)Math.Pow(1.5f, gameSettings.currentLevel - 3);
        base.Start();
    }
    protected override void Update()
    {
        // 處理速度提升計時
        speedBoostTimer += Time.deltaTime;
        if (speedBoostTimer >= speedBoostInterval && magicPerLife > 0)
        {
            StartCoroutine(BoostSpeedAndPause());
            speedBoostTimer = 0f;
        }

        // 根據暫停狀態決定是否移動
        if (!isPaused)
        {
            base.Move(); // 只在沒暫停時移動
        }

        // 更新血條位置和值（與 BaseEnemy 一致）
        UpdateHealthBarPosition();
        UpdateHealthBar(); // 新增：確保血條值平滑更新
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
        magicPerLife--;
        yield return new WaitForSeconds(1f);
        isPaused = false;
    }
}