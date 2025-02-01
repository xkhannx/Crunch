using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData 
{
    public string mapName;
    public bool playable;

    public int[] floorX;
    public int[] floorY;
    public int[] pointX;
    public int[] pointY;
    public int[] spawnX;
    public int[] spawnY;
    public int[] spawnDir;

    public MapData(string _mapName, int[] _floorX, int[] _floorY, int[] _pointX, int[] _pointY, int[] _spawnX, int[] _spawnY, int[] _spawnDir)
    {
        mapName = _mapName;
        playable = false;

        floorX = _floorX;
        floorY = _floorY;
        pointX = _pointX;
        pointY = _pointY;
        spawnX = _spawnX;
        spawnY = _spawnY;
        spawnDir = _spawnDir;
    }
}
