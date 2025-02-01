using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    GridManager grid;

    private void Start()
    {
        grid = GetComponent<GridManager>();
    }

    public List<Node> FindPath(Vector3 startPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node endNode = null;
        Queue<Node> nodeQ = new Queue<Node>();
        nodeQ.Enqueue(startNode);

        while (nodeQ.Count > 0)
        {
            Node currentNode = nodeQ.Dequeue();

            if (currentNode.point != null)
            {
                endNode = currentNode;
                break;
            }

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable) continue;

                if (!neighbor.observed)
                {
                    neighbor.observed = true;
                    neighbor.cost = currentNode.cost + 1;
                    neighbor.parent = currentNode;
                    nodeQ.Enqueue(neighbor);
                }
            }
        }

        ResetNodes();

        if (endNode != null)
        {
            return RetracePath(startNode, endNode);
        }

        return null;
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();

        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        
        return path;
    }

    void ResetNodes()
    {
        foreach (Node n in grid.grid)
        {
            n.cost = 0;
            n.observed = false;
        }
    }
}
