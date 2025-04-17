using UnityEngine;

public class Tower1Bullet : MonoBehaviour
{
    public float speed = 5f;
    public float damage = 10f;
    private GameObject target;

    public void SetTarget(GameObject targetEnemy)
    {
        target = targetEnemy;
        UpdateRotation(); // 初始旋轉
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // 更新旋轉
        UpdateRotation();

        // 移動
        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

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
        Enemy enemyScript = target.GetComponent<Enemy>();
        SpeedBoostEnemy specialEnemyScript = target.GetComponent<SpeedBoostEnemy>();

        if (enemyScript != null)
        {
            enemyScript.TakeDamage(damage);
            Debug.Log($"{gameObject.name} 命中 {target.name}，造成 {damage} 傷害");
        }
        else if (specialEnemyScript != null)
        {
            specialEnemyScript.TakeDamage(damage);
            Debug.Log($"{gameObject.name} 命中 {target.name}，造成 {damage} 傷害");
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == target)
        {
            HitTarget();
        }
    }
}