using DefineEnum;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    public GameObject _heartPrefab;
    List<HeartManager> _hearts = new List<HeartManager>();

    public void DrawHearts(float currentHP)
    {
        for (int i = 0; i < _hearts.Count; i++)
        {
            if (i < currentHP)
            {
                _hearts[i].SetHeartImage(HeartSatus.Full);
            }
            else
                _hearts[i].SetHeartImage(HeartSatus.Empty);
        }

        Debug.Log("하트 그리기");
    }

    public void CreateEmptyHeart(float maxHP, bool isMonster = true)
    {
        if (isMonster)
        {
            for (int i = 0; i < maxHP; i++)
            {
                GameObject newHeart = Resources.Load<GameObject>("UI/MonsterHeart");
                GameObject go = Instantiate(newHeart, transform);
                HeartManager heart = go.GetComponent<HeartManager>();
                heart.SetHeartImage(HeartSatus.Empty);
                _hearts.Add(heart);

                Debug.Log(newHeart);
            }
        }
        else
        {
            for (int i = 0; i < maxHP; i++)
            {
                GameObject newHeart = Resources.Load<GameObject>("UI/PlayerHeart");
                GameObject go = Instantiate(newHeart, transform);
                HeartManager heart = go.GetComponent<HeartManager>();
                heart.SetHeartImage(HeartSatus.Empty);
                _hearts.Add(heart);

                Debug.Log(newHeart);
            }
        }
       
    }

    public void ClearHeart()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        _hearts = new List<HeartManager>();
    }

}
