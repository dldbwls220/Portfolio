using UnityEngine;
using UnityEngine.UI;

public class NoteDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LeftNote"))
        {
            if(!collision.transform.GetComponent<Note>()._isStop)
                IngameManager._instance.ResetCombo();

            TimingManager.Instance._boxNoteListL.Remove(collision.gameObject);
            ObjectPool._instance._leftNoteQueue.Enqueue(collision.gameObject);
            collision.gameObject.SetActive(false);

            //Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("RightNote"))
        {
            if (!collision.transform.GetComponent<Note>()._isStop)
                IngameManager._instance.ResetCombo();

            TimingManager.Instance._boxNoteListR.Remove(collision.gameObject);
            ObjectPool._instance._rightNoteQueue.Enqueue(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
    }
}
