using UnityEngine;


public class Demon_boss : BaseEnemy
{
    private float timer = 0f;
    protected override void Update()
    {
        timer += Time.unscaledDeltaTime * Time.timeScale;
        if (timer >= 5f)
        {
            currentHealth += 750;
            targetHealth = currentHealth;
            timer -= 5f; 
        }
        base.Update();
    }
}
