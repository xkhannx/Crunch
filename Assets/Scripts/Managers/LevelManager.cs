using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    PointsManager points;
    GridManager grid;
    EnemyManager enemies;

    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject gun;
    [SerializeField] AudioClip song;
    [SerializeField] GameObject overlayImage;

    private void Awake()
    {
        points = GetComponent<PointsManager>();
        grid = FindObjectOfType<GridManager>();
        enemies = FindObjectOfType<EnemyManager>();
    }
    private void Start()
    {
        LoadLevel();

        gun.SetActive(true);
        if (song != null)
            AudioSource.PlayClipAtPoint(song, transform.position);
    }

    [Header("Titles")]
    [SerializeField] Text title;
    void LoadLevel() // run only once
    {
        CurrentLevel cur = FindObjectOfType<CurrentLevel>();
        
        
        LoadMap(cur.levelToLoad);

        enemies.SpawnEnemies();
        points.InitPoints(grid.numPoints);
        string rootPath = Application.persistentDataPath + "/maps";
        string temp = cur.levelToLoad.Substring(rootPath.Length + 1, cur.levelToLoad.Length - rootPath.Length - 1);
        title.text = temp;

    }

    public void LoadMap(string path)
    {
        grid.CreateGrid();

        MapData loadedMap = SaveSystem.Load(path);

        for (int i = 0; i < loadedMap.floorX.Length; i++)
        {
            grid.grid[loadedMap.floorX[i], loadedMap.floorY[i]].walkable = true;
        }
        for (int i = 0; i < loadedMap.pointX.Length; i++)
        {
            grid.grid[loadedMap.pointX[i], loadedMap.pointY[i]].hasPoint = true;
        }
        for (int i = 0; i < loadedMap.spawnDir.Length; i++)
        {
            grid.grid[loadedMap.spawnX[i], loadedMap.spawnY[i]].spawnDir = loadedMap.spawnDir[i];
        }

        grid.ConstructMap();
    }

    public void GameWon()
    {
        gun.SetActive(false);
        StartCoroutine(WinScreen());
    }

    IEnumerator WinScreen()
    {
        yield return new WaitForSeconds(1f);
        winScreen.SetActive(true);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OverlayImage()
    {
        overlayImage.SetActive(!overlayImage.activeInHierarchy);
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
