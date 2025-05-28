using UnityEngine;
using System;

public class Tower1Bullet : MonoBehaviour
{
    public float speed = 5f;
    public float damage = 10f; // 由塔傳遞的傷害值
    private GameObject target;
    public Action onHitOrDestroy; // 回調事件，通知塔子彈命中或銷毀

    public void SetTarget(GameObject targetEnemy)
    {
        target = targetEnemy;
        UpdateRotation(); // 初始旋轉
    }

    void Update()
    {
        if (target == null)
        {
            onHitOrDestroy?.Invoke();
            Destroy(gameObject);
            return;
        }

        // 更新旋轉
        UpdateRotation();

        // 移動，使用 Time.unscaledDeltaTime 並考慮 timeScale
        Vector3 direction = (target.transform.position - transform.position).normalized;
        float effectiveDeltaTime = Time.unscaledDeltaTime * Time.timeScale; // 確保移動距離與時間縮放一致
        transform.position += direction * speed * effectiveDeltaTime;

        // 如果接近目標，執行命中
        if (Vector2.Distance(transform.position, target.transform.position) < 0.2f)
        {
            HitTarget();
        }
    }

    private void UpdateRotation()
    {
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void HitTarget()
    {
        BaseEnemy enemy = target.GetComponent<BaseEnemy>();
        if (enemy != null && enemy.GetCurrentHealth() > 0)
        {
            enemy.TakeDamage(damage);
        }
        onHitOrDestroy?.Invoke(); // 通知塔子彈已命中
        Destroy(gameObject); // 命中後銷毀子彈
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == target)
        {
            HitTarget();
        }
    }

    void OnDestroy()
    {
        onHitOrDestroy?.Invoke(); // 確保銷毀時通知塔
    }
}