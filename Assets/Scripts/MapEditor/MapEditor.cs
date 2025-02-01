using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapEditor : MonoBehaviour
{
    Camera cam;
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
    void Start()
    {
        cam = Camera.main;
        LoadLevel();
    }

    List<Node> usedNodes = new List<Node>();
    enum BrushAction {Brushing, Erasing};
    BrushAction brAct = BrushAction.Brushing;

    void Update()
    {
        if (brush == BrushType.Empty) return;

        if (Input.GetMouseButtonDown(0))
        {
            FirstClick();
        }
        else if (Input.GetMouseButton(0))
        {
            if (strokeStarted)
            {
                Brushing();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            strokeStarted = false;
        }
    }

    private void Brushing()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Node pressedNode = NodeFromWorldPoint(mousePos);

        if (pressedNode != null)
        {
            switch (brush)
            {
                case BrushType.Floor:
                    if (!usedNodes.Contains(pressedNode))
                    {
                        switch (brAct)
                        {
                            case BrushAction.Brushing:
                                if (pressedNode.floorPrefab == null)
                                {
                                    usedNodes.Add(pressedNode);

                                    pressedNode.floorPrefab = Instantiate(floorPrefab, pressedNode.worldPosition, Quaternion.identity, transform);
                                    pressedNode.walkable = true;
                                }
                                break;
                            case BrushAction.Erasing:
                                usedNodes.Add(pressedNode);
                                if (pressedNode.floorPrefab != null)
                                {
                                    Destroy(pressedNode.floorPrefab);
                                    if (pressedNode.hasPoint)
                                    {
                                        pressedNode.hasPoint = false;
                                        Destroy(pressedNode.point);
                                    }

                                    if (pressedNode.spawnDir > 0)
                                    {
                                        pressedNode.spawnDir = 0;
                                        Destroy(pressedNode.spawnPoint.gameObject);
                                    }

                                    pressedNode.walkable = false;
                                }
                                break;
                        }
                    }
                    break;
                case BrushType.Point:
                    
                    if (!usedNodes.Contains(pressedNode))
                    {
                        if (pressedNode.walkable)
                        {
                            switch (brAct)
                            {
                                case BrushAction.Brushing:

                                    if (!pressedNode.hasPoint)
                                    {
                                        usedNodes.Add(pressedNode);

                                        pressedNode.hasPoint = true;
                                        pressedNode.point = Instantiate(pointPrefab, pressedNode.worldPosition, Quaternion.identity, transform);
                                    }
                                    break;
                                case BrushAction.Erasing:
                                    usedNodes.Add(pressedNode);
                                    if (pressedNode.hasPoint)
                                    {
                                        pressedNode.hasPoint = false;
                                        Destroy(pressedNode.point);
                                    }
                                    break;
                            }
                        }
                    }
                    break;
            }
        }
    }

    bool strokeStarted = false;

    private void FirstClick()
    {
        usedNodes.Clear();

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Node pressedNode = NodeFromWorldPoint(mousePos);

        if (pressedNode != null)
        {
            switch (brush)
            {
                case BrushType.Floor:
                    if (!pressedNode.walkable)
                    {
                        brAct = BrushAction.Brushing;
                        strokeStarted = true;
                        usedNodes.Add(pressedNode);

                        pressedNode.floorPrefab = Instantiate(floorPrefab, pressedNode.worldPosition, Quaternion.identity, transform);
                        pressedNode.walkable = true;
                    }
                    else
                    {
                        brAct = BrushAction.Erasing;
                        strokeStarted = true;

                        usedNodes.Add(pressedNode);
                        Destroy(pressedNode.floorPrefab);
                        if (pressedNode.hasPoint)
                        {
                            pressedNode.hasPoint = false;
                            Destroy(pressedNode.point);
                        }

                        if (pressedNode.spawnDir > 0)
                        {
                            pressedNode.spawnDir = 0;
                            Destroy(pressedNode.spawnPoint.gameObject);
                        }

                        pressedNode.walkable = false;
                    }
                    break;

                case BrushType.Point:
                    if (pressedNode.walkable)
                    {
                        if (!pressedNode.hasPoint)
                        {
                            brAct = BrushAction.Brushing;
                            strokeStarted = true;

                            usedNodes.Add(pressedNode);

                            pressedNode.hasPoint = true;
                            pressedNode.point = Instantiate(pointPrefab, pressedNode.worldPosition, Quaternion.identity, transform);
                        }
                        else
                        {
                            brAct = BrushAction.Erasing;
                            strokeStarted = true;

                            usedNodes.Add(pressedNode);
                            pressedNode.hasPoint = false;
                            Destroy(pressedNode.point);
                        }
                    }
                    break;

                case BrushType.Spawn:
                    if (pressedNode.walkable)
                    {
                        if (pressedNode.spawnPoint == null) // if no spawnPoints attached yet
                        {
                            bool spawned = false;
                            while (!spawned && pressedNode.spawnDir < 4)
                            {
                                pressedNode.spawnDir++;
                                
                                spawned = PlaceSpawnPoint(pressedNode);
                            }
                            if (!spawned)
                            {
                                pressedNode.spawnDir = 0;
                            }
                        } else // spawn point exists
                        {
                            Destroy(pressedNode.spawnPoint.gameObject);

                            bool spawned = false;
                            while (!spawned && pressedNode.spawnDir < 4)
                            {
                                pressedNode.spawnDir++;

                                spawned = PlaceSpawnPoint(pressedNode);
                            }
                            if (!spawned)
                            {
                                pressedNode.spawnDir = 0;
                            }
                        }
                    }
                    break;
            }
        }
    }
    
    bool PlaceSpawnPoint(Node curNode)
    {
        Node tempNode = NodeFromWorldPoint(curNode.worldPosition + spawnDirs[curNode.spawnDir - 1]);
        if (tempNode == null || !tempNode.walkable)
        {
            GameObject pointer = Instantiate(spawnPrefab, curNode.worldPosition + spawnDirs[curNode.spawnDir - 1], Quaternion.identity, transform);
            Vector3 rot = spawnRots[curNode.spawnDir - 1];
            pointer.transform.eulerAngles = rot;
            curNode.spawnPoint = pointer.transform;
            return true;
        }
        return false;
    }

    [Header("Prefabs")]
    [SerializeField] GameObject floorPrefab;
    [SerializeField] GameObject pointPrefab;
    [SerializeField] GameObject spawnPrefab;

    [Header("Brushes")]
    [SerializeField] SelectTileButton floorButton;
    [SerializeField] SelectTileButton pointButton;
    [SerializeField] SelectTileButton spawnButton;

    public enum BrushType { Empty, Floor, Point, Spawn}
    public BrushType brush = BrushType.Empty;

    public void SetBrush(BrushType newBrush)
    {
        brush = newBrush;
        floorButton.transform.localScale = Vector3.one * 10;
        floorButton.selected = false;
        pointButton.transform.localScale = Vector3.one * 10;
        pointButton.selected = false;
        spawnButton.transform.localScale = Vector3.one * 10;
        spawnButton.selected = false;

        switch (brush)
        {
            case BrushType.Empty:
                break;
            case BrushType.Floor:
                floorButton.selected = true;
                floorButton.transform.localScale = Vector3.one * 12;
                break;
            case BrushType.Point:
                pointButton.selected = true;
                pointButton.transform.localScale = Vector3.one * 12;
                break;
            case BrushType.Spawn:
                spawnButton.selected = true;
                spawnButton.transform.localScale = Vector3.one * 12;
                break;

        }
    }

    // Grid generation
    [Header("Grid gen parameters:")]
    public Vector2 gridWorldSize;
    public float nodeRadius;

    public Node[,] grid;

    public int gridSizeX, gridSizeY;
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
    
    Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;

        if (percentX != Mathf.Clamp01(percentX) || percentY != Mathf.Clamp01(percentY))
            return null;

        if (percentX == 1) percentX = 0.99f;
        if (percentY == 1) percentY = 0.99f;

        int x = Mathf.FloorToInt(gridSizeX * percentX);
        int y = Mathf.FloorToInt(gridSizeY * percentY);

        return grid[x, y];
    }

    public void ClearMap()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        CreateGrid();
    }

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
                }
                if (grid[i, j].spawnDir > 0)
                {
                    GameObject pointer = Instantiate(spawnPrefab, grid[i, j].worldPosition + spawnDirs[grid[i, j].spawnDir - 1], Quaternion.identity, transform);
                    Vector3 rot = spawnRots[grid[i, j].spawnDir - 1];
                    pointer.transform.eulerAngles = rot;
                    grid[i, j].spawnPoint = pointer.transform;
                }
            }
        }
    }

    [Header("Titles")]
    [SerializeField] Text title;
    [SerializeField] Text pathTitle;
    void LoadLevel() // run only once
    {
        CreateGrid();

        CurrentLevel cur = FindObjectOfType<CurrentLevel>();

        GetComponent<MapSaver>().path = cur.levelToLoad;
        
        string rootPath = Application.persistentDataPath + "/maps";
        string temp = cur.levelToLoad.Substring(rootPath.Length + 1, cur.levelToLoad.Length - rootPath.Length - 1);

        GetComponent<MapSaver>().mapName = temp;
        title.text = temp;
        pathTitle.text = cur.levelToLoad;

        if (!cur.newLevel)
        {
            GetComponent<MapSaver>().LoadMap();
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));
    }
}
