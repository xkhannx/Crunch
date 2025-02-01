using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] Enemy enemyPrefab;
    [SerializeField] Text enemyText;
    Pathfinding atlas;
    GridManager grid;
    LevelManager levelManager;

    public int numEnemies = 1;
    public int totalEnemies = 60;

    List<Enemy> enemies = new List<Enemy>();
    
    private void Awake()
    {
        atlas = FindObjectOfType<Pathfinding>();
        grid = atlas.GetComponent<GridManager>();
        levelManager = FindObjectOfType<LevelManager>();
    }

    public void SpawnEnemies()  // run once
    {
        List<Node> spawnNodesTemp = new List<Node>(grid.spawnNodes);
        
        int curEnemies = 0;

        while (curEnemies < numEnemies && spawnNodesTemp.Count > 0)
        {
            int randInd = Random.Range(0, spawnNodesTemp.Count);

            spawnNodesTemp[randInd].walkable = false;

            enemies.Add(Instantiate(enemyPrefab, spawnNodesTemp[randInd].spawnPoint.position, Quaternion.identity, transform));
            enemies[curEnemies].curNode = spawnNodesTemp[randInd];
            enemies[curEnemies].nextNode = null;
            enemies[curEnemies].graphic = enemies[curEnemies].GetComponentInChildren<EnemySprite>();
            enemies[curEnemies].justSpawned = true;

            spawnNodesTemp.RemoveAt(randInd);
            curEnemies++;
        }
        enemyText.text = "Enemies left: " + totalEnemies.ToString();
        StartCoroutine(EnemySchedule());
    }

    float timer = 0;
    public float stepTime = 1;
    IEnumerator EnemySchedule()
    {
        List<Enemy> shotEnemies = new List<Enemy>();
        List<Enemy> deadEnemies = new List<Enemy>();
        
        while (totalEnemies > 0)
        {
            timer += Time.deltaTime;
            if (timer >= stepTime)
            {
                timer = 0;
                // Plan the next move
                Planning();
                Respawn(deadEnemies);
            }

            // Move
            for (int i = 0; i < enemies.Count; i++)
            {
                // if dead then cleanup
                if (enemies[i].dead)
                {
                    totalEnemies--;
                    enemyText.text = "Enemies left: " + totalEnemies.ToString();

                    shotEnemies.Add(enemies[i]);
                    enemies[i].curNode.walkable = true;
                    if (enemies[i].nextNode != null)
                        enemies[i].nextNode.walkable = true;

                    enemies[i].GetComponent<CircleCollider2D>().enabled = false;
                    continue;
                }

                // if eating dont move
                if (enemies[i].eating) continue;

                // if just spawned then change transparency
                ReenterEnemy(enemies[i]);

                // if alive then move
                if (enemies[i].nextNode != null)
                {
                    Vector3 nextPos = enemies[i].nextNode.worldPosition - enemies[i].curNode.worldPosition;
                    nextPos *= timer / stepTime;
                    nextPos += enemies[i].curNode.worldPosition;

                    enemies[i].transform.position = nextPos;
                }
            }

            // remove the dead ones to a separate list 
            foreach (Enemy enemy in shotEnemies)
            {
                enemies.Remove(enemy);
                deadEnemies.Add(enemy);
            }
            shotEnemies.Clear();

            yield return null;
        }
        // Game Won
        foreach (Enemy enemy in enemies)
        {
            enemy.graphic.Death();
            levelManager.GameWon();

        }
    }

    private void ReenterEnemy(Enemy enemy)
    {
        if (enemy.justSpawned)
        {
            Color color = enemy.graphic.GetComponent<SpriteRenderer>().color;
            color.a = timer / stepTime;
            enemy.graphic.GetComponent<SpriteRenderer>().color = color;

            Vector3 nextPos = enemy.curNode.worldPosition - enemy.curNode.spawnPoint.position;
            nextPos *= timer / stepTime;
            nextPos += enemy.curNode.spawnPoint.position;

            enemy.transform.position = nextPos;
        }
    }

    void Respawn(List<Enemy> dead)
    {
        if (dead.Count == 0) return;
        foreach (Enemy enemy in dead)
        {
            if (!enemy.readyToRespawn) continue;

            // find an empty edge node and spawn there
            List<Node> spawnNodesTemp = new List<Node>(grid.spawnNodes);

            while (spawnNodesTemp.Count > 0)
            {
                int randInd = Random.Range(0, spawnNodesTemp.Count);
                
                if (spawnNodesTemp[randInd].walkable)
                {
                    spawnNodesTemp[randInd].walkable = false;
                    enemy.curNode = spawnNodesTemp[randInd];
                    enemy.nextNode = null;
                    enemy.transform.position = spawnNodesTemp[randInd].spawnPoint.position;
                    
                    enemy.graphic.Rebirth();
                     
                    enemy.dead = false;
                    enemy.eating = false;
                    enemy.readyToRespawn = false;
                    enemy.justSpawned = true;

                    enemies.Add(enemy);
                    enemy.GetComponent<CircleCollider2D>().enabled = true;
                    break;
                } else
                {
                    spawnNodesTemp.RemoveAt(randInd);
                }
            }
        }

        foreach (Enemy enemy in enemies)
        {
            if (dead.Contains(enemy))
            {
                dead.Remove(enemy);
            }
        }
    }

    private void Planning()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].dead) continue;

            if (enemies[i].justSpawned) enemies[i].justSpawned = false;

            // If moved Finish previous movement
            if (enemies[i].nextNode != null)
            {
                enemies[i].curNode = enemies[i].nextNode;
                enemies[i].nextNode = null;
                enemies[i].transform.position = enemies[i].curNode.worldPosition;
            }
            // If coin found start or finish eating
            if (enemies[i].curNode.point != null)
            {
                if (!enemies[i].eating)
                {
                    enemies[i].eating = true;
                    enemies[i].graphic.Eat(true);
                }
                else
                {
                    enemies[i].eating = false;
                    enemies[i].graphic.Eat(false);

                    Destroy(enemies[i].curNode.point);
                    enemies[i].curNode.point = null;
                    FindObjectOfType<PointsManager>().RemovePoint();
                }
            }

            if (!enemies[i].eating)
            {
                // Calculate next paths 
                List<Node> path = atlas.FindPath(enemies[i].transform.position);

                if (path != null)
                {
                    enemies[i].curNode.walkable = true;
                    enemies[i].nextNode = path[0];
                    enemies[i].nextNode.walkable = false;
                }
            }
        }
    }
}
