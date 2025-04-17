using UnityEngine;
using UnityEngine.EventSystems;

public class Tower1 : MonoBehaviour
{

    [SerializeField]
    private float attackSpeed = 1f; // 每秒攻擊次數（1 次/秒）
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
    


    void Update()
    {
        luckyManager = Object.FindFirstObjectByType<LuckyManager>();
        
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        FindTarget();
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
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hit.gameObject;
                }
            }
        }

        currentTarget = closestEnemy;
        if (currentTarget == null)
        {
            Debug.Log($"{gameObject.name} 無目標可攻擊");
        }
    }

    private void Attack()
    {
        if (currentTarget != null)
        {
            // 發射子彈
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Tower1Bullet bulletScript = bullet.GetComponent<Tower1Bullet>();
            bulletScript.SetTarget(currentTarget);
            bulletScript.damage = damage; // 傳遞傷害值
            Debug.Log($"{gameObject.name} 發射子彈攻擊 {currentTarget.name}");

            attackCooldown = 1f / attackSpeed;
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
            if (isLucky)
            {
                attackSpeed += 0.8f;
                damage += 6f;
                attackRange += 0.6f;
                luckyManager.SpendLucky(3);
            }else
            {
                attackSpeed += 0.5f;
                damage += 5f;
                attackRange += 0.5f;
                luckyManager.AddLucky(5);
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
        upgradePrice += 10;
    }
    
}