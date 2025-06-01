using UnityEngine;

public class ArrowTower : TowerBase
{
    [SerializeField] private GameObject bulletPrefab;
    private int bulletsFired = 0;
    private int requiredArrows = 0;

    void Start()
    {
        towerInfos =    $"等級: {GetLevel()}\n" +
                        $"攻擊傷害: {GetDamage()}\n" +
                        $"攻擊速度: {GetAttackSpeed()}\n" +
                        $"攻擊範圍: {GetAttackRange()}";
    }
    protected override void InitializeTargetState()
    {
        
        if (currentTarget != null)
        {
            BaseEnemy enemy = currentTarget.GetComponent<BaseEnemy>();
            requiredArrows = Mathf.CeilToInt(enemy.GetCurrentHealth() / damage);
            bulletsFired = 0;
            Debug.Log($"{gameObject.name} 需 {requiredArrows} 箭擊殺 {currentTarget.name}");
        }
    }

    protected override void ResetTargetState()
    {
        bulletsFired = 0;
        requiredArrows = 0;
    }

    protected override void Attack()
    {
        if (currentTarget != null)
        {
            BaseEnemy enemy = currentTarget.GetComponent<BaseEnemy>();
            if (enemy != null && enemy.GetCurrentHealth() > 0)
            {
                if (bulletsFired < requiredArrows)
                {
                    Vector3 offset = new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y, 0f) * 0.1f;
                    GameObject bullet = Instantiate(bulletPrefab, transform.position + offset, Quaternion.identity);
                    ArrowBullet bulletScript = bullet.GetComponent<ArrowBullet>();
                    bulletScript.SetTarget(currentTarget);
                    bulletScript.damage = damage;

                    bulletsFired++;
                    attackCooldown = 1f / attackSpeed;
                    Debug.Log($"{gameObject.name} 發射子彈 {bulletsFired}/{requiredArrows} 攻擊 {currentTarget.name}，目標血量: {enemy.GetCurrentHealth()}");
                }
                else
                {
                    requiredArrows = Mathf.CeilToInt(enemy.GetCurrentHealth() / damage);
                    bulletsFired = 0;
                    Debug.Log($"{gameObject.name} 已射 {bulletsFired}/{requiredArrows} 箭，重新計算需 {requiredArrows} 箭");
                }
            }
        }
    }

    public override void Upgrade()
    {
        base.Upgrade(); // 使用 TowerBase 的 luckytimes
    }
}