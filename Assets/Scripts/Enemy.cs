using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Node curNode, nextNode;
    public bool dead = false;
    public bool eating = false;
    public bool readyToRespawn = false;
    public bool justSpawned = false;
    
    public EnemySprite graphic;
}
