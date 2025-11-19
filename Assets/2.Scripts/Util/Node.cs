using UnityEngine;

//수정
public class Node : IHeapItem<Node>
//수정(end)
{
    #region[MonsterReserves]
    public bool _BatNode { get; set; } = false;
    public bool _SkeletonNode { get; set; } = false;
    public bool _SlimeNode { get; set; } = false;
    public bool _GolemNode { get; set; } = false;
    #endregion[MonsterReserves]
    public bool _walkable { get; set; }
    public Vector3 _worldPosition { get; set; }

    Vector2Int _grideIndex;
    public Node _parent { get; set; }
    // == 추가
    public int _movementCost { get; set; }
    // == 추가(end)
    public int _gCost { get; set; }
    public int _hCost { get; set; }
    public int _fCost { get => _gCost + _hCost; }
    public int _grideX
    {
        get => _grideIndex.x;
        set => _grideIndex.x = value;
    }
    public int _grideY
    {
        get => _grideIndex.y;
        set => _grideIndex.y = value;
    }
    //==추가
    int _index;
    public int _heapIndex { get => _index; set => _index = value; }
    //==추가(end)
    public Node(bool walkable, Vector3 worldPos, int idxX, int idxY, int cost)
    {
        _walkable = walkable;
        _worldPosition = worldPos;
        _grideX = idxX;
        _grideY = idxY;
        _movementCost = cost;
    }
    //==추가
    public int CompareTo(Node nodeToCompare)
    {
        int compare = _fCost.CompareTo(nodeToCompare._fCost);

        if (compare == 0)
            compare = _hCost.CompareTo(nodeToCompare._hCost);

        return -compare;
    }
    //==추가(end)
}
