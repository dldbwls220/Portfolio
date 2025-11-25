using UnityEngine;

public class BigMonsterSorter : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] int headOffset = 200;  // Decor보다 확실히 위로 오게 하는 값
    [SerializeField] float multiplier = 100f; // 기존 y기반 정렬과 동일하게 유지

    void LateUpdate()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        // 기존 feetY 기반 정렬 + 드래곤 전용 보정
        sr.sortingOrder = -(int)(transform.position.y * multiplier) + headOffset;
    }
}
