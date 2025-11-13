using UnityEngine;

public class CenterFLame : MonoBehaviour
{
    AudioSource _audioS;

    bool _isPlaying;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioS = GetComponent<AudioSource>();
        _isPlaying = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LeftNote"))
        {
            if (!_isPlaying)
            {
                _audioS.Play();
                _isPlaying = true;
            }
        }
    }
}
