using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TowerInfo", menuName = "Scriptable Objects/TowerInfo")]
public class TowerInfo : ScriptableObject
{
    public string towerName;
    public int maxlevel;
    public float attackSpeed;
    public float attackRange;
    public float damage;
    public int cost;
    public GameObject prefab; // 塔的預製體

    // 用於儲存不同塔的額外屬性
    [System.Serializable]
    public struct TowerAttribute
    {
        public string attributeName;
        public string value;          
    }
    public List<TowerAttribute> additionalAttributes = new List<TowerAttribute>();
}
