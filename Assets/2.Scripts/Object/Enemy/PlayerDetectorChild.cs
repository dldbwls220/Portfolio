using UnityEngine;

public class PlayerDetectorChild : MonoBehaviour
{
    private PlayerDetectorParent _playerDParent;
    [SerializeField] private string _directionName;

    void Start()
    {
        _playerDParent = GetComponentInParent<PlayerDetectorParent>();

        if (string.IsNullOrEmpty(_directionName))
            _directionName = gameObject.name;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_playerDParent != null)
            {
                _playerDParent.OnPlayerDetected(_directionName, collision.gameObject);
            }
        }

        if (collision.CompareTag("Obstacle"))
        {
            if (_playerDParent != null)
            {
                _playerDParent.OnWallDetected(_directionName, collision.gameObject);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_playerDParent != null)
            {
                _playerDParent.OnPlayerLost(_directionName, collision.gameObject);
            }
        }

        if (collision.CompareTag("Obstacle"))
        {
            if (_playerDParent != null)
            {
                _playerDParent.OnWallLost(_directionName, collision.gameObject);
            }
        }
    }
}
