using System.Collections.Generic;
using UnityEngine;

public class DetectorParent : MonoBehaviour
{
    private Dictionary<string, List<GameObject>> _monsterDetected = new Dictionary<string, List<GameObject>>();

    public void OnMonsterDetected(string direction, GameObject monster)
    {
        if (!_monsterDetected.ContainsKey(direction))
            _monsterDetected[direction] = new List<GameObject>();

        if (!_monsterDetected[direction].Contains(monster))
        {
            _monsterDetected[direction].Add(monster);
            Debug.Log($"[{direction}] 방향에서 몬스터 감지됨");
        }
    }

    public void OnMonsterLost(string direction, GameObject monster)
    {
        if (_monsterDetected.ContainsKey(direction))
        {
            _monsterDetected[direction].Remove(monster);
            Debug.Log($"[{direction}] 방향에서 몬스터 사라짐");
        }
    }

    public bool IsMonsterInDirection(string direction)
    {
        return _monsterDetected.ContainsKey(direction) && _monsterDetected[direction].Count > 0;
    }

    
}
