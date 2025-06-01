using UnityEngine;

public class icewizard : TowerBase
{
    [SerializeField] private float slowDuration = 2f; // 凍結持續時間
    [SerializeField] private float slowAmount = 0.25f; // 減速比例

    void Start()
    {
        towerInfos = $"等級: {GetLevel()}\n" +
                     $"攻擊範圍: {GetAttackRange()}\n" +
                     $"冰凍冷卻時間: {GetAttackSpeed()}\n" +
                     $"冰凍冷卻倍率: {100f - slowAmount * 100}%移速\n" +
                     $"減速持續時間: {slowDuration}秒";
    }

    public override string GetTowerInfos()
    {
        return $"等級: {GetLevel()}\n" +
                     $"攻擊範圍: {GetAttackRange()}\n" +
                     $"冰凍冷卻時間: {GetAttackSpeed()}\n" +
                     $"冰凍冷卻倍率: {100f - slowAmount * 100}%移速\n" +
                     $"減速持續時間: {slowDuration}秒";
    }

    protected override void Update()
    {
        luckyManager ??= FindFirstObjectByType<LuckyManager>();
        coinManager ??= FindFirstObjectByType<CoinManager>();

        if (attackCooldown > 0)
        {
            attackCooldown -= Time.unscaledDeltaTime * Time.timeScale;
        }

        if (attackCooldown <= 0)
        {
            Attack();
        }
    }
    protected override void Attack()
    {
        // 取得攻擊範圍內所有敵人
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy") || hit.CompareTag("SpeedUpEnemy"))
            {
                BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
                if (enemy != null)
                {
                    enemy.ApplySlow(slowAmount, slowDuration);
                }
            }
        }
        attackCooldown = 1f / attackSpeed; // 重置攻擊冷卻時間
    }

    protected override void FindTarget()
    {
        // 冰法不需要鎖定目標
        currentTarget = null;
    }

    public override void ApplyLuckyBonus(float multiplier)
    {
        slowAmount *= multiplier;
        slowDuration *= multiplier;
        attackSpeed *= multiplier;
    }
    public override void Upgrade()
    {
        if (level < maxLevel)
        {
            bool isLucky = luckyManager.RollLucky();
            level++;
            attackRange = Mathf.Min(attackRange + 0.3f, 3f);
            attackRange = Mathf.Round(attackRange * 10f) / 10f;
            slowDuration = Mathf.Min(slowDuration + 0.1f, 3f);
            slowDuration = Mathf.Round(slowDuration * 10f) / 10f;
            
            if (isLucky)
            {
                attackSpeed = Mathf.Max(attackSpeed - 0.1f, 1f);
                attackSpeed = Mathf.Round(attackSpeed * 10f) / 10f; 
                slowAmount += 0.05f;
                luckyManager.SpendLucky(3);
                luckytimes++;
                TooltipManager.Instance.ShowTooltip("幸運值加成！");
            }
            else
            {
                slowAmount += 0.03f;
                luckyManager.AddLucky(5);
                luckytimes--;
            }
            slowAmount = Mathf.Round(slowAmount * 100f) / 100f;
            cost += upgradePrice;
            UpdatePrice();
        }
        else
        {
            Debug.Log($"{gameObject.name} 已達最大等級 Lv{level}");
        }
    }
    protected override void UpdatePrice()
    {
        upgradePrice += 60 * (level - 1);
    }

    
}
