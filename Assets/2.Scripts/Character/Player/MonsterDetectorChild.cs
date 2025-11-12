using UnityEngine;

public class MonsterDetectorChild : MonoBehaviour
{
    private MonsterDetectorParent _monsterDParent;
    [SerializeField] private string _directionName;

    void Start()
    {
        _monsterDParent = GetComponentInParent<MonsterDetectorParent>();

        if (string.IsNullOrEmpty(_directionName))
            _directionName = gameObject.name;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            if (_monsterDParent != null)
            {
                _monsterDParent.OnMonsterDetected(_directionName, collision.gameObject);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            if (_monsterDParent != null)
            {
                _monsterDParent.OnMonsterLost(_directionName, collision.gameObject);
            }
        }
    }
}
