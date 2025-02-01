using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject floorPrefab;
    [SerializeField] GameObject spawnPrefab;
    [SerializeField] GameObject pointPrefab;

    [Header("Grid gen parameters:")]
    public Vector2 gridWorldSize;
    public float nodeRadius;

    public Node[,] grid;
    public List<Node> spawnNodes = new List<Node>();

    public int gridSizeX, gridSizeY;

    Vector3[] spawnDirs;
    Vector3[] spawnRots;
    private void Awake()
    {
        spawnDirs = new Vector3[4];
        spawnDirs[0] = new Vector3(0, 0.67f, 0);
        spawnDirs[1] = new Vector3(0.67f, 0, 0);
        spawnDirs[2] = new Vector3(0, -0.67f, 0);
        spawnDirs[3] = new Vector3(-0.67f, 0, 0);

        spawnRots = new Vector3[4];
        spawnRots[0] = new Vector3(0, 0, 180);
        spawnRots[1] = new Vector3(0, 0, 90);
        spawnRots[2] = new Vector3(0, 0, 0);
        spawnRots[3] = new Vector3(0, 0, -90);
    }

    public void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - new Vector3(gridSizeX / 2, gridSizeY / 2);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + new Vector3(x * nodeRadius * 2 + nodeRadius, y * nodeRadius * 2 + nodeRadius, 0);

                grid[x, y] = new Node(false, worldPoint, x, y);
            }
        }
    }

    public int numPoints = 0;
    public void ConstructMap()
    {
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                if (grid[i, j].walkable)
                {
                    grid[i, j].floorPrefab = Instantiate(floorPrefab, grid[i, j].worldPosition, Quaternion.identity, transform);
                }
                if (grid[i, j].hasPoint)
                {
                    grid[i, j].point = Instantiate(pointPrefab, grid[i, j].worldPosition, Quaternion.identity, transform);
                    numPoints++;
                }
                if (grid[i, j].spawnDir > 0)
                {
                    GameObject pointer = Instantiate(spawnPrefab, grid[i, j].worldPosition + spawnDirs[grid[i, j].spawnDir - 1], Quaternion.identity, transform);
                    Vector3 rot = spawnRots[grid[i, j].spawnDir - 1];
                    pointer.transform.eulerAngles = rot;
                    grid[i, j].spawnPoint = pointer.transform;

                    spawnNodes.Add(grid[i, j]);
                }
            }
        }
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if ((x + y) % 2 == 0) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkY >= 0 && checkX < gridSizeX && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        if (percentX == 1) percentX = 0.99f;
        if (percentY == 1) percentY = 0.99f;

        int x = Mathf.FloorToInt(gridSizeX * percentX);
        int y = Mathf.FloorToInt(gridSizeY * percentY);

        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));
        if (grid != null)
        {
            for (int i = 0; i < gridSizeX; i++)
            {
                for (int j = 0; j < gridSizeY; j++)
                {
                    if (grid[i, j].walkable)
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawWireSphere(grid[i, j].worldPosition, nodeRadius);
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireSphere(grid[i, j].worldPosition, nodeRadius);
                    }
                }
            }
        }
    }
}
