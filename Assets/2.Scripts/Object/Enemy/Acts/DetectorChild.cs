using UnityEngine;

public class DetectorChild : MonoBehaviour
{
    private DetectorParent _DParent;
    [SerializeField] private string _directionName;

    void Start()
    {
        _DParent = GetComponentInParent<DetectorParent>();

        if (string.IsNullOrEmpty(_directionName))
            _directionName = gameObject.name;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            if (_DParent != null)
            {
                _DParent.OnMonsterDetected(_directionName, collision.gameObject);
            }
        }

        
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            if (_DParent != null)
            {
                _DParent.OnMonsterLost(_directionName, collision.gameObject);
            }
        }

        
    }
}
