using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public string characterName;
    public int health;
    public int abilityPower;
    public int level;
    public int experience;
    public int experienceToNextLevel;
    public string offensiveMove1Name;
    public string offensiveMove2Name;
    public string defensiveMoveName;
    public Move offensiveMove1;
    public Move offensiveMove2;
    public Move defensiveMove;

    private CombatManager _combatManager;

    // Start is called before the first frame update
    void Start()
    {
        _combatManager = FindObjectOfType<CombatManager>();
    }

    public void SetMoves(Move oM1, Move oM2, Move dM)
    {
        offensiveMove1 = oM1;
        offensiveMove2 = oM2;
        defensiveMove = dM;
    }

    public void ExecuteOffensiveMove1()
    {
        //offensiveMove1.Execute();
    }

    public void ExecuteOffensiveMove2()
    {
        //offensiveMove2.Execute();
    }

    public void ExecuteDefensiveMove()
    {
        //defensiveMove.Execute();
    }

    public void FinishedMove()
    {
        Debug.Log("Ending turn");
        _combatManager.EndTurn();
    }
}
