using UnityEngine;
using System;

public class BulletBase : MonoBehaviour
{
    [SerializeField] protected float speed = 5f; // 子彈速度
    public float damage = 10f; // 傷害值
    protected GameObject target; // 目標敵人
    public Action onHitOrDestroy; // 回調事件，通知塔子彈命中或銷毀

    public virtual void SetTarget(GameObject targetEnemy)
    {
        target = targetEnemy;
        UpdateRotation();
    }

    protected virtual void Update()
    {
        if (target == null)
        {
            onHitOrDestroy?.Invoke();
            Destroy(gameObject);
            return;
        }

        // 更新旋轉
        UpdateRotation();

        Vector3 direction = (target.transform.position - transform.position).normalized;
        float effectiveDeltaTime = Time.unscaledDeltaTime * Time.timeScale; //DeltaTime
        transform.position += direction * speed * effectiveDeltaTime;

        // 檢查是否接近目標
        if (Vector2.Distance(transform.position, target.transform.position) < 0.2f)
        {
            HitTarget();
        }
    }

    protected virtual void UpdateRotation()
    {
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    protected virtual void HitTarget()
    {
        BaseEnemy enemy = target?.GetComponent<BaseEnemy>();
        if (enemy != null && enemy.GetCurrentHealth() > 0)
        {
            enemy.TakeDamage(damage);
        }
        onHitOrDestroy?.Invoke();
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == target)
        {
            HitTarget();
        }
    }

    protected virtual void OnDestroy()
    {
        onHitOrDestroy?.Invoke();
    }
}