using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int health;
    public int experience;
    public int level;

    public IMove offensiveMove1;
    public IMove offensiveMove2;
    public IMove defensiveMove;
}
