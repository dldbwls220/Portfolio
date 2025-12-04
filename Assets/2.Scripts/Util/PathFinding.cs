using DefineEnum;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public TileMapGridManager grid;

    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos, MonsterBase requester = null)
    {
        Node startNode = grid.NodeFromWorldPos(startPos);
        Node targetNode = grid.NodeFromWorldPos(targetPos);

        // 경로가 불가능한 경우
        if (!targetNode._walkable)
        {
            return null;
        }

        Heap<Node> openSet = new Heap<Node>(grid._gridSizeMax);
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode._gCost = 0;
        startNode._hCost = GetDistance(startNode, targetNode);
        startNode._parent = null;

        openSet.Add(startNode);

        while (openSet._Count > 0)
        {
            Node current = openSet.RemoveFirst();
            closedSet.Add(current);

            // 목표 도착
            if (current == targetNode)
            {
                List<Node> finalPath = RetracePath(startNode, targetNode);                

                return finalPath;
            }

            foreach (Node neighbor in grid.GetNeighbors(current))
            {
                if (!neighbor._walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                if (requester != null && neighbor._reservedBy != null && neighbor._reservedBy != requester)
                {
                    // 만약 requester가 더 우선 순위가 높으면 허용
                    if ((int)requester._monsterP <= (int)neighbor._reservedBy._monsterP)
                        continue; 
                }

                int dirPenalty = 0;

                if (requester != null && requester._monsterP == MonsterPriority.RedDragon)
                {
                    // 세로 이동 → 보너스
                    if (neighbor._grideX == current._grideX)
                        dirPenalty = -5;     // 세로 우선
                    else
                        dirPenalty = 20;     // 가로 패널티
                }

                // 실제 비용 계산
                int newCost = current._gCost
                            + GetDistance(current, neighbor)
                            + neighbor._movementCost
                            + dirPenalty;


                if (newCost < neighbor._gCost || !openSet.Contains(neighbor))
                {
                    neighbor._gCost = newCost;
                    neighbor._hCost = GetDistance(neighbor, targetNode);
                    neighbor._parent = current;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                    else
                        openSet.UpdateItem(neighbor);
                }
            }
        }

        return null; // 경로 없음
    }

    // 경로 재구성
    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node current = endNode;

        while (current != startNode)
        {
            path.Add(current);
            current = current._parent;
        }

        path.Add(startNode);
        path.Reverse();

        return path;
    }

    // 이동 거리 계산
    int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a._grideX - b._grideX);
        int dstY = Mathf.Abs(a._grideY - b._grideY);

        return 10 * (dstX + dstY);
    }
}
