using UnityEngine;
using UnityEngine.EventSystems;

public class TowerBase : MonoBehaviour
{
    [SerializeField] protected float attackSpeed = 1f; // 每秒攻擊次數
    [SerializeField] protected float damage = 10f;     // 每次攻擊傷害
    [SerializeField] protected float attackRange = 2f; // 攻擊範圍
    [SerializeField] protected int level = 1;          // 當前等級
    [SerializeField] protected int maxLevel = 20;      // 最大等級
    [SerializeField] protected string towerName = "塔"; // 塔名稱
    [SerializeField] protected int cost = 50;          // 總價值
    [SerializeField] protected int upgradePrice = 10;  // 升級價格

    protected float attackCooldown = 0f;
    protected GameObject currentTarget;
    public TowerPlacementPoint placementPoint;
    public int luckytimes = 0; // 記錄幸運次數
    protected LuckyManager luckyManager;
    protected CoinManager coinManager;
    protected string towerInfos; // 用於儲存塔的資訊

    protected virtual void Update()
    {
        // 初始化管理器
        luckyManager ??= FindFirstObjectByType<LuckyManager>();
        coinManager ??= FindFirstObjectByType<CoinManager>();

        // 更新冷卻時間
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.unscaledDeltaTime * Time.timeScale;
        }

        // 檢查當前目標是否有效
        if (currentTarget != null)
        {
            BaseEnemy enemy = currentTarget.GetComponent<BaseEnemy>();
            if (enemy == null || enemy.GetCurrentHealth() <= 0 ||
                Vector2.Distance(transform.position, currentTarget.transform.position) > attackRange)
            {
                currentTarget = null;
                ResetTargetState();
            }
        }

        // 尋找新目標
        if (currentTarget == null)
        {
            FindTarget();
        }

        // 執行攻擊
        if (currentTarget != null && attackCooldown <= 0)
        {
            Attack();
        }
    }

    protected virtual void FindTarget()
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

        currentTarget = closestEnemy;
        if (currentTarget != null)
        {
            InitializeTargetState();
            Debug.Log($"{gameObject.name} 選擇新目標 {currentTarget.name}");
        }
        else
        {
            Debug.Log($"{gameObject.name} 無目標可攻擊");
        }
    }

    protected virtual void Attack()
    {
        // 預設攻擊邏輯，子類可覆蓋
        Debug.Log($"{gameObject.name} 執行預設攻擊");
    }

    protected virtual void InitializeTargetState()
    {
        // 子類可覆蓋以初始化目標相關狀態
    }

    protected virtual void ResetTargetState()
    {
        // 子類可覆蓋以重置目標相關狀態
    }

    protected virtual void OnDrawGizmosSelected()
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
    public string GetTowerName() => towerName;
    public int GetCost() => cost;

    public virtual string GetTowerInfos()
    {
        return $"等級: {GetLevel()}\n" +
               $"攻擊傷害: {GetDamage()}\n" +
               $"攻擊速度: {GetAttackSpeed()}\n" +
               $"攻擊範圍: {GetAttackRange()}";
    }

    public virtual void Upgrade()
    {
        if (level < maxLevel)
        {
            bool isLucky = luckyManager.RollLucky();
            level++;
            attackRange = Mathf.Min(attackRange + 0.3f, 6f);
            attackRange = Mathf.Round(attackRange * 10f) / 10f;

            if (isLucky)
            {
                attackSpeed = Mathf.Round((attackSpeed + 0.5f) * 10f) / 10f;
                damage += 5f;
                luckyManager.SpendLucky(3);
                luckytimes++;
                TooltipManager.Instance.ShowTooltip("幸運值加成！");
            }
            else
            {
                attackSpeed = Mathf.Round((attackSpeed + 0.3f) * 10f) / 10f;
                damage += 3f;
                luckytimes--;
                luckyManager.AddLucky(5);
            }

            cost += upgradePrice;
            UpdatePrice();
            Debug.Log($"{gameObject.name} 升級為 Lv{level}，傷害: {damage}，攻速: {attackSpeed}，範圍: {attackRange}");
        }
        else
        {
            Debug.Log($"{gameObject.name} 已達最大等級 Lv{level}");
        }
    }

    public virtual void ApplyLuckyBonus(float multiplier)
    {
        damage *= multiplier;
        attackSpeed *= multiplier;
    }

    protected virtual void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        TowerUIManager uiManager = FindFirstObjectByType<TowerUIManager>();
        if (uiManager != null)
        {
            uiManager.ShowTowerInfo(this);
        }
    }

    protected virtual void UpdatePrice()
    {
        upgradePrice += 40 * (level - 1);
    }

    public virtual void DestroyTower()
    {
        if (coinManager != null)
        {
            coinManager.AddGold((int)(cost * 0.8f)); // 返還80%金幣
        }
        if (placementPoint != null)
        {
            placementPoint.RemoveTower();
        }
        Destroy(gameObject);
    }
}