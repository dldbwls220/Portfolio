using System.Collections.Generic;
using UnityEngine;

public class TimingManager : MonoBehaviour
{
    public static TimingManager Instance;

    public List<GameObject> _boxNoteListL;
    public List<GameObject> _boxNoteListR;

    [SerializeField] Transform _center;
    [SerializeField] RectTransform[] timingRect;
    Vector2[] _timingBoxes;

    bool _canJudge;
    float _judgeCooldown;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initManager();
    }

    void initManager()
    {
        Debug.Log(_center.localPosition.x);

        _canJudge = true;
        _judgeCooldown = 0.15f;

        //타이밍 박스 설정
        _timingBoxes = new Vector2[timingRect.Length];
        _boxNoteListL = new List<GameObject>();
        _boxNoteListR = new List<GameObject>();
        for (int i = 0; i < timingRect.Length; i++)
        {
            _timingBoxes[i].Set(_center.localPosition.x - timingRect[i].rect.width / 2, _center.localPosition.x + timingRect[i].rect.width / 2);
            Debug.Log(_timingBoxes[i]);
        }
    }

    public bool CheckTiming()
    {
        if (!_canJudge) return false;

        for (int i = 0; i < _boxNoteListL.Count; i++)
        {
            float notePosX = _boxNoteListL[i].transform.localPosition.x;

            for (int n = 0; n < _timingBoxes.Length; n++)
            {

                if (_timingBoxes[n].x <= notePosX && notePosX <= _timingBoxes[n].y)
                {

                    if (_timingBoxes[n] == _timingBoxes[2])
                    {
                        _canJudge = false;
                        Invoke(nameof(ResetJudge), _judgeCooldown);

                        _boxNoteListL[i].GetComponent<Note>().StopSprite();
                        _boxNoteListL.RemoveAt(i);
                        _boxNoteListR[i].GetComponent<Note>().StopSprite();
                        _boxNoteListR.RemoveAt(i);
                        Debug.Log("Miss");

                        return false;
                    }
                    else
                    {
                        _canJudge = false;
                        Invoke(nameof(ResetJudge), _judgeCooldown);

                        _boxNoteListL[i].GetComponent<Note>().StopSprite();
                        _boxNoteListL.RemoveAt(i);
                        _boxNoteListR[i].GetComponent<Note>().StopSprite();
                        _boxNoteListR.RemoveAt(i);

                        Debug.Log("Hit" + n);

                        return true;
                    }                  
                }
            }
        }

        return false;
        
    }

    void ResetJudge()
    {
        _canJudge = true;
    }
}
