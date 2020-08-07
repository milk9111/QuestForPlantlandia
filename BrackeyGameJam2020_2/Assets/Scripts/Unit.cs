using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;
    public int experience;
    public int experienceToNextLevel = 100;
    public int damage;
    public int maxHP;
    public int currentHP;
    public string offensiveMove1Name;
    public string offensiveMove2Name;
    public string defensiveMoveName;

    public Transform backTarget;
    public Transform launcher;

    private float _distanceToBackTarget;

    void Start()
    {
        _distanceToBackTarget = Math.Abs(transform.position.x - backTarget.position.x);
    }

    public bool TakeDamage(int damage)
    {
        currentHP -= damage;

        return currentHP <= 0;
    }

    public bool LevelUp(int xp)
    {
        experience += xp;

        if (experience >= experienceToNextLevel)
        {
            experienceToNextLevel += (unitLevel * 100) + 100;
            unitLevel++;
            return true;
        }

        return false;
    }

    void Update()
    {
        var point = Camera.main.WorldToScreenPoint(backTarget.position);

        if (point.x < 0)
        {
            var pos = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.WorldToScreenPoint(transform.position).x + Math.Abs(point.x) + _distanceToBackTarget, 0, 0));
            transform.position = new Vector3(pos.x, transform.position.y, 0);
        } else if (point.x > Screen.width)
        {
            var pos = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.WorldToScreenPoint(transform.position).x - Math.Abs(point.x - Screen.width) - _distanceToBackTarget, 0, 0));
            transform.position = new Vector3(pos.x, transform.position.y, 0);
        }
    }
}
