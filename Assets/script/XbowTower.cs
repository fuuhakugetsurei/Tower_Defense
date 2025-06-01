using UnityEngine;

public class CrossbowTower : TowerBase
{
    [SerializeField] private GameObject bulletPrefab; // 子彈預製體
    private int bulletsFired = 0; // 已發射的箭數
    private int requiredArrows = 3; // 每段攻擊的箭數（初始 3 發）
    private float burstInterval = 0.15f; // 每發箭的間隔
    private float burstCooldown = 3f; // 每段攻擊後的冷卻
    private float lastFireTime = 0f; // 上次射擊的時間
    private enum TowerState { Idle, Bursting, Cooling } // 狀態機
    private TowerState state = TowerState.Idle; // 當前狀態
    void Start()
    {
        towerInfos = $"等級: {GetLevel()}\n" +
                     $"攻擊傷害: {GetDamage()}\n" +
                     $"攻擊範圍: {GetAttackRange()}\n" +
                     $"每段攻擊箭數: {requiredArrows}\n" +
                     $"冷卻時間: {burstCooldown}秒";
    }

    public override string GetTowerInfos()
    {
        return       $"等級: {GetLevel()}\n" +
                     $"攻擊傷害: {GetDamage()}\n" +
                     $"攻擊範圍: {GetAttackRange()}\n" +
                     $"每段攻擊箭數: {requiredArrows}\n" +
                     $"冷卻時間: {burstCooldown}秒";
    }

    protected override void InitializeTargetState()
    {
        bulletsFired = 0;
        state = TowerState.Idle;
        lastFireTime = 0f;
        if (currentTarget != null)
        {
            BaseEnemy enemy = currentTarget.GetComponent<BaseEnemy>();
            Debug.Log($"[{Time.frameCount}] {gameObject.name} 準備攻擊新目標 {currentTarget.name}，目標血量: {enemy.GetCurrentHealth()}");
        }
    }

    protected override void ResetTargetState()
    {
        bulletsFired = 0;
        state = TowerState.Idle;
        lastFireTime = 0f;
        Debug.Log($"[{Time.frameCount}] {gameObject.name} 重置目標狀態，進入 Idle");
    }

    protected override void Update()
    {
        // 初始化管理器
        luckyManager ??= FindFirstObjectByType<LuckyManager>();
        coinManager ??= FindFirstObjectByType<CoinManager>();

        // 更新冷卻
        if (state == TowerState.Cooling)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0)
            {
                state = TowerState.Idle;
                Debug.Log($"[{Time.frameCount}] {gameObject.name} 冷卻結束，進入 Idle 狀態");
            }
            return; // <--- 冷卻時直接結束，不做任何事
        }

        // 只有 Idle 狀態才會尋找目標與攻擊
        if (state == TowerState.Idle)
    {
    // 檢查當前目標是否有效
    if (currentTarget != null)
    {
        BaseEnemy enemy = currentTarget.GetComponent<BaseEnemy>();
        // 只在自己攻擊的敵人死亡時才重設狀態
        if (enemy == null || enemy.GetCurrentHealth() <= 0 ||
            Vector2.Distance(transform.position, currentTarget.transform.position) > attackRange)
        {
            Debug.Log($"[{Time.frameCount}] {gameObject.name} 目標 {currentTarget?.name} 無效（血量: {enemy?.GetCurrentHealth()}, 距離: {Vector2.Distance(transform.position, currentTarget.transform.position)}), 原狀態: {state}");
            currentTarget = null;
            // 不要直接呼叫 ResetTargetState()，只要把 currentTarget 設為 null，讓下方自動尋找新目標
        }
    }

    // 尋找新目標
    if (currentTarget == null)
    {
        FindTarget();
        // 這裡可以加一行：如果找到新目標，直接 Attack()
        if (currentTarget != null)
        {
            Attack();
        }
    }
    // 不要再 Attack()，避免重複攻擊
}
        else if (state == TowerState.Bursting)
        {
            // 先檢查目標是否還有效
            if (currentTarget == null || 
                currentTarget.GetComponent<BaseEnemy>()?.GetCurrentHealth() <= 0)
            {
                state = TowerState.Cooling;
                attackCooldown = burstCooldown;
                currentTarget = null;
                bulletsFired = requiredArrows;
                Debug.Log($"[{Time.frameCount}] {gameObject.name} Bursting 中目標消失，進入冷卻");
                return;
            }

            // 目標有效才繼續射擊
            float currentTime = Time.time;
            if (currentTime - lastFireTime >= burstInterval && bulletsFired < requiredArrows)
            {
                FireSingleBullet();
            }
        }
    }

    protected override void Attack()
    {
        BaseEnemy enemy = currentTarget?.GetComponent<BaseEnemy>();
        if (enemy != null && enemy.GetCurrentHealth() > 0)
        {
            state = TowerState.Bursting;
            bulletsFired = 0;
            lastFireTime = Time.time - burstInterval; // 讓第一發能立即射出
            Debug.Log($"[{Time.frameCount}] {gameObject.name} 開始快速射擊，目標: {currentTarget.name}");
            // 不要直接呼叫 FireSingleBullet()，讓 Update 控制發射
        }
    }

    private void FireSingleBullet()
    {
        BaseEnemy enemy = currentTarget?.GetComponent<BaseEnemy>();
        if (enemy != null && enemy.GetCurrentHealth() > 0)
        {
            Vector3 offset = new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y, 0f) * 0.1f;
            GameObject bullet = Instantiate(bulletPrefab, transform.position + offset, Quaternion.identity);
            ArrowBullet bulletScript = bullet.GetComponent<ArrowBullet>();
            bulletScript.SetTarget(currentTarget);
            bulletScript.damage = damage;
            bulletScript.onHitOrDestroy = OnBulletHitOrDestroy;

            bulletsFired++;
            lastFireTime = Time.time; // 記錄射擊時間
            Debug.Log($"[{Time.frameCount}] {gameObject.name} 發射第 {bulletsFired}/{requiredArrows} 箭，目標: {currentTarget.name}，目標血量: {enemy.GetCurrentHealth()}，Time.time: {Time.time}, FPS: {1f / Time.deltaTime}");

            if (bulletsFired >= requiredArrows)
            {
                state = TowerState.Cooling;
                attackCooldown = burstCooldown;
                Debug.Log($"[{Time.frameCount}] {gameObject.name} 完成 {requiredArrows} 箭射擊，進入 {burstCooldown} 秒冷卻");
            }
        }
        else
        {
            // 目標無效時，強制進入冷卻
            state = TowerState.Cooling;
            attackCooldown = burstCooldown;
            currentTarget = null;
            bulletsFired = requiredArrows; // 停止射擊
            Debug.Log($"[{Time.frameCount}] {gameObject.name} 目標無效或已死亡，發射中斷，進入 {burstCooldown} 秒冷卻");
            return;
        }
    }

    private void OnBulletHitOrDestroy()
    {
        // 只要目標死亡就強制進入冷卻
        if (state != TowerState.Bursting) return; // 只在連射狀態下處理
        if (currentTarget == null) return;
        BaseEnemy enemy = currentTarget.GetComponent<BaseEnemy>();
        if (enemy == null || enemy.GetCurrentHealth() <= 0)
        {
            state = TowerState.Cooling;
            attackCooldown = burstCooldown;
            currentTarget = null;
            bulletsFired = requiredArrows; // 阻止繼續射箭
            Debug.Log($"[{Time.frameCount}] {gameObject.name} 目標在快速射擊中死亡，進入 {burstCooldown} 秒冷卻");
            // 不要呼叫 ResetTargetState()，避免直接回到 Idle
        }
    }

    public override void Upgrade()
    {
        if (level < maxLevel)
        {
            bool isLucky = luckyManager.RollLucky();
            level++;
            attackRange = Mathf.Min(attackRange + 0.3f, 6f);
            attackRange = Mathf.Round(attackRange * 10f) / 10f;

            if (isLucky)
            {
                damage += 10f;
                luckyManager.SpendLucky(3);
                luckytimes++;
                TooltipManager.Instance.ShowTooltip("幸運值加成！");
            }
            else
            {
                damage += 8f;
                luckyManager.AddLucky(5);
                luckytimes--;
            }

            if (level % 5 == 0)
            {
                burstCooldown -= 0.3f; // 每 5 級減少冷卻時間
                requiredArrows++;
                Debug.Log($"{gameObject.name} 每段攻擊間隔減少至 {burstCooldown} 秒，箭數增加至 {requiredArrows}");
            }

            cost += upgradePrice;
            UpdatePrice();
            Debug.Log($"{gameObject.name} 升級為 Lv{level}，傷害: {damage}，範圍: {attackRange}，箭數: {requiredArrows}，冷卻: {burstCooldown}");
        }
        else
        {
            Debug.Log($"{gameObject.name} 已達最大等級 Lv{level}");
        }
    }
}