using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectorParent : MonoBehaviour
{
    private Dictionary<string, List<GameObject>> _playerDetected = new Dictionary<string, List<GameObject>>();
    private Dictionary<string, List<GameObject>> _wallDetected = new Dictionary<string, List<GameObject>>();

    public void OnPlayerDetected(string direction, GameObject monster)
    {
        if (!_playerDetected.ContainsKey(direction))
            _playerDetected[direction] = new List<GameObject>();

        if (!_playerDetected[direction].Contains(monster))
        {
            _playerDetected[direction].Add(monster);
            Debug.Log($"[{direction}] 방향에서 플레이어 감지됨");
        }
    }

    public void OnWallDetected(string direction, GameObject monster)
    {
        if (!_wallDetected.ContainsKey(direction))
            _wallDetected[direction] = new List<GameObject>();

        if (!_wallDetected[direction].Contains(monster))
        {
            _wallDetected[direction].Add(monster);
            Debug.Log($"[{direction}] 벽 감지됨");
        }
    }

    public void OnPlayerLost(string direction, GameObject monster)
    {
        if (_playerDetected.ContainsKey(direction))
        {
            _playerDetected[direction].Remove(monster);
            Debug.Log($"[{direction}] 방향에서 플레이어 사라짐");
        }
    }

    public void OnWallLost(string direction, GameObject monster)
    {
        if (_wallDetected.ContainsKey(direction))
        {
            _wallDetected[direction].Remove(monster);
            Debug.Log($"[{direction}] 방향에서 벽 사라짐");
        }
    }

    public bool IsPlayerInDirection(string direction)
    {
        return _playerDetected.ContainsKey(direction) && _playerDetected[direction].Count > 0;
    }

    public bool IsWallInDirection(string direction)
    {
        return _wallDetected.ContainsKey(direction) && _wallDetected[direction].Count > 0;
    }
}
