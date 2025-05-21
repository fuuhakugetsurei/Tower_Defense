using UnityEngine;


public class Demon_boss : BaseEnemy
{
    private float timer = 0f; // 計時器
    protected override void Update()
    {
        timer += Time.deltaTime; // 每秒更新一次
        if (timer >= 5f)
        {
            currentHealth += 1000;//每三秒回30%的血量
            targetHealth = currentHealth;
            timer -= 3f; 
        }
        base.Update();
    }
}
