using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class MapSaver : MonoBehaviour
{
    public string mapName;

    int[] floorX;
    int[] floorY;
    int[] pointX;
    int[] pointY;
    int[] spawnX;
    int[] spawnY;
    int[] spawnDir;

    public string path;
    public void SaveMap()
    {
        MapEditor mapEditor = FindObjectOfType<MapEditor>();

        List<Node> floorNodes = new List<Node>();
        List<Node> pointNodes = new List<Node>();
        List<Node> spawnNodes = new List<Node>();

        for (int i = 0; i < mapEditor.gridSizeX; i++)
        {
            for (int j = 0; j < mapEditor.gridSizeY; j++)
            {
                if (mapEditor.grid[i, j].walkable)
                {
                    floorNodes.Add(mapEditor.grid[i, j]);
                }
                if (mapEditor.grid[i,j].hasPoint)
                {
                    pointNodes.Add(mapEditor.grid[i, j]);
                }
                if (mapEditor.grid[i, j].spawnDir > 0)
                {
                    spawnNodes.Add(mapEditor.grid[i, j]);
                }
            }
        }

        string rootPath = Application.persistentDataPath + "/maps";
        string temp = path.Substring(rootPath.Length + 1, path.Length - rootPath.Length - 1);
        mapName = temp;

        floorX = new int[floorNodes.Count];
        floorY = new int[floorNodes.Count];
        pointX = new int[pointNodes.Count];
        pointY = new int[pointNodes.Count];
        spawnX = new int[spawnNodes.Count];
        spawnY = new int[spawnNodes.Count];
        spawnDir = new int[spawnNodes.Count];

        for (int i = 0; i < floorNodes.Count; i++)
        {
            floorX[i] = floorNodes[i].gridX;
            floorY[i] = floorNodes[i].gridY;
        }
        for (int i = 0; i < pointNodes.Count; i++)
        {
            pointX[i] = pointNodes[i].gridX;
            pointY[i] = pointNodes[i].gridY;
        }
        for (int i = 0; i < spawnNodes.Count; i++)
        {
            spawnX[i] = spawnNodes[i].gridX;
            spawnY[i] = spawnNodes[i].gridY;
            spawnDir[i] = spawnNodes[i].spawnDir;
        }

        MapData mapSave = new MapData(mapName, floorX, floorY, pointX, pointY, spawnX, spawnY, spawnDir);
        mapSave.playable = floorNodes.Count > 0 && pointNodes.Count > 0 && spawnNodes.Count > 0;
        
        SaveSystem.SaveMap(mapSave);
    }

    public void LoadMap()
    {
        MapEditor mapEditor = FindObjectOfType<MapEditor>();
        mapEditor.ClearMap();

        MapData loadedMap = SaveSystem.Load(path);
        floorX = loadedMap.floorX;
        floorY = loadedMap.floorY;
        pointX = loadedMap.pointX;
        pointY = loadedMap.pointY;
        spawnX = loadedMap.spawnX;
        spawnY = loadedMap.spawnY;
        spawnDir = loadedMap.spawnDir;

        for (int i = 0; i < floorX.Length; i++)
        {
            mapEditor.grid[floorX[i], floorY[i]].walkable = true;
        }
        for (int i = 0; i < pointX.Length; i++)
        {
            mapEditor.grid[pointX[i], pointY[i]].hasPoint = true;
        }
        for (int i = 0; i < spawnDir.Length; i++)
        {
            mapEditor.grid[spawnX[i], spawnY[i]].spawnDir = loadedMap.spawnDir[i];
        }

        mapEditor.ConstructMap();
    }
}
