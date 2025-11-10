using UnityEngine;

public class NoteDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("RightNote") || collision.CompareTag("LeftNote"))
        {
            TimingManager.Instance._boxNoteListL.Remove(collision.gameObject);
            TimingManager.Instance._boxNoteListR.Remove(collision.gameObject);
            Destroy(collision.gameObject);
        }
    }
}
