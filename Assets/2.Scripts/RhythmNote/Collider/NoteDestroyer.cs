using UnityEngine;
using UnityEngine.UI;

public class NoteDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("RightNote") || collision.CompareTag("LeftNote"))
        {
            if(!collision.transform.GetComponent<Note>()._isStop)
                IngameManager._instance.ResetCombo();

            TimingManager.Instance._boxNoteListL.Remove(collision.gameObject);
<<<<<<< HEAD
            ObjectPool._instance._leftNoteQueue.Enqueue(collision.gameObject);
            collision.gameObject.SetActive(false);

            //Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("RightNote"))
        {
            if (!collision.transform.GetComponent<Note>()._isStop)
                IngameManager._instance.ResetCombo();

=======
>>>>>>> parent of 1eae76b (Note Sync and Pool)
            TimingManager.Instance._boxNoteListR.Remove(collision.gameObject);
            Destroy(collision.gameObject);
        }
    }
}
