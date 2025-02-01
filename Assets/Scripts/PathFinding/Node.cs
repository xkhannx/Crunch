using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    
    public bool walkable;
    public GameObject floorPrefab;

    public bool hasPoint;
    public GameObject point;
    
    public int spawnDir;
    public Transform spawnPoint;

    // pathfinding
    public Node parent;
    public int cost;
    public bool observed = false;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        walkable = _walkable;
    }
}
