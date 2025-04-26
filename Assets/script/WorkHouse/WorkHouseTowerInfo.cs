using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WorkHouseTowerInfo", menuName = "Scriptable Objects/WorkHouseTowerInfo")]
public class WorkHouseTowerInfo : ScriptableObject
{
    public string towerName;
    public int maxlevel;
    public int cost;

    // 用於儲存不同塔的額外屬性
    [System.Serializable]
    public struct TowerAttribute
    {
        public string attributeName;
        public string value;          
    }
    public List<TowerAttribute> additionalAttributes = new List<TowerAttribute>();
}
