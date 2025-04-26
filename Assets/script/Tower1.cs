using UnityEngine;
using UnityEngine.EventSystems;

public class Tower1 : MonoBehaviour
{
    [SerializeField]
    private float attackSpeed = 1f; // 每秒發射箭數
    [SerializeField]
    private float damage = 10f;     // 每次攻擊傷害（傳給子彈）
    [SerializeField]
    private float attackRange = 2f; // 攻擊範圍（單位：Unity 單位）
    [SerializeField]
    private GameObject bulletPrefab; // 子彈預製體

    [SerializeField]
    private int level = 1;
    [SerializeField]
    private const int maxLevel = 20;
    private float attackCooldown = 0f;
    private GameObject currentTarget;
    private int upgradePrice = 10;
    private LuckyManager luckyManager;
    private int bulletsInFlight = 0; // 追蹤飛行中的子彈數
    private int bulletsFired = 0;    // 針對當前目標已發射的箭數
    private int requiredArrows = 0;  // 新增：當前目標所需的總箭數（初始計算）
    
    public int luckytimes  = 0; 

    void Update()
    {
        luckyManager = FindFirstObjectByType<LuckyManager>();
        
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        // 檢查當前目標是否有效，若無效則重置
        if (currentTarget != null)
        {
            BaseEnemy enemy = currentTarget.GetComponent<BaseEnemy>();
            if (enemy == null || enemy.GetCurrentHealth() <= 0 || 
                Vector2.Distance(transform.position, currentTarget.transform.position) > attackRange)
            {
                currentTarget = null;
                bulletsFired = 0;
                bulletsInFlight = 0;
                requiredArrows = 0; // 重置所需箭數
            }
        }

        // 只有在沒有當前目標時才尋找新目標
        if (currentTarget == null)
        {
            FindTarget();
        }

        if (currentTarget != null && attackCooldown <= 0)
        {
            Attack();
        }
    }

    private void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy") || hit.CompareTag("SpeedUpEnemy"))
            {
                BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
                if (enemy != null && enemy.GetCurrentHealth() > 0)
                {
                    float distance = Vector2.Distance(transform.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = hit.gameObject;
                    }
                }
            }
        }

        if (closestEnemy != null)
        {
            currentTarget = closestEnemy;
            BaseEnemy enemy = currentTarget.GetComponent<BaseEnemy>();
            requiredArrows = Mathf.CeilToInt(enemy.GetCurrentHealth() / damage); // 計算初始所需箭數
            bulletsFired = 0; // 重置已發射箭數
            Debug.Log($"{gameObject.name} 選擇新目標 {currentTarget.name}，需 {requiredArrows} 箭");
        }
        else
        {
            currentTarget = null;
            Debug.Log($"{gameObject.name} 無目標可攻擊");
        }
    }

    private void Attack()
    {
        if (currentTarget != null)
        {
            BaseEnemy enemy = currentTarget.GetComponent<BaseEnemy>();
            if (enemy != null && enemy.GetCurrentHealth() > 0)
            {
                // 檢查是否需要發射更多箭
                if (bulletsFired < requiredArrows)
                {
                    // 發射單支箭
                    GameObject bullet = Instantiate(bulletPrefab, transform.position + new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y, 0f) * 0.1f, Quaternion.identity);
                    Tower1Bullet bulletScript = bullet.GetComponent<Tower1Bullet>();
                    bulletScript.SetTarget(currentTarget);
                    bulletScript.damage = damage;
                    bulletScript.onHitOrDestroy += () => bulletsInFlight--; // 子彈命中或銷毀時減少計數
                    Debug.Log($"{gameObject.name} 發射子彈 {bulletsFired + 1}/{requiredArrows} 攻擊 {currentTarget.name}，目標血量: {enemy.GetCurrentHealth()}");

                    bulletsInFlight++;
                    bulletsFired++;
                    attackCooldown = 1f / attackSpeed; // 設置冷卻時間
                }
                else
                {
                    // 如果已射夠箭但敵人還沒死，重新計算所需箭數
                    requiredArrows = Mathf.CeilToInt(enemy.GetCurrentHealth() / damage);
                    bulletsFired = 0; // 重置已發射箭數
                    Debug.Log($"{gameObject.name} 已射 {bulletsFired}/{requiredArrows} 箭，敵人仍存活，重新計算需 {requiredArrows} 箭");
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public int GetLevel() => level;
    public float GetDamage() => damage;
    public float GetAttackSpeed() => attackSpeed;
    public float GetAttackRange() => attackRange;
    public int GetPrice() => upgradePrice;
    public int GetMaxLevel() => maxLevel;

    public void Upgrade()
    {
        if (level < maxLevel)
        {
            bool isLucky = luckyManager.RollLucky();
            level++;
            if (attackRange < 6f) attackRange = Mathf.Round((attackRange + 0.3f) * 10f) / 10f;
            if (isLucky)
            {
                attackSpeed = Mathf.Round((attackSpeed + 0.5f) * 10f) / 10f;
                damage += 5f;
                luckyManager.SpendLucky(3);
                luckytimes++;
            }
            else
            {
                attackSpeed = Mathf.Round((attackSpeed + 0.3f) * 10f) / 10f;
                damage += 3f;
                luckyManager.AddLucky(5);
                luckytimes--;
            }
            UpdatePrice();
            Debug.Log($"{gameObject.name} 升級為 Lv{level}，傷害: {damage}，攻速: {attackSpeed}，範圍: {attackRange}");
        }
        else
        {
            Debug.Log($"{gameObject.name} 已達最大等級 Lv{level}");
        }
    }

    public void ApplyLuckyBonus(float multiplier)
    {
        damage *= multiplier;
        attackSpeed *= multiplier;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        TowerUIManager uiManager = FindFirstObjectByType<TowerUIManager>();
        if (uiManager != null)
        {
            uiManager.ShowTowerInfo(this);
        }
    }

    private void UpdatePrice()
    {
        upgradePrice += 10 * (level - 1);
    }
}