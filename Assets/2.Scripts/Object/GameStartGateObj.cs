using System.Collections;
using UnityEngine;

public class GameStartGateObj : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(MoveSceneDelay());
        }

        IEnumerator MoveSceneDelay()
        {
            yield return new WaitForSeconds(0.3f);
            SoundManager._instance._lobbyDESC._stop();
            SceneControlManager._instance.StartGame();
        }
    }
}
