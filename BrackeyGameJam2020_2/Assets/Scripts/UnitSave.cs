using System;
using System.Linq;

[Serializable]
public class UnitSave
{
    public string unitName;
    public int unitLevel;
    public int damage;
    public int maxHP;
    public int currentHP;
    public string offensiveMove1Name;
    public string offensiveMove2Name;
    public string defensiveMoveName;
    public string[] enemyMoves;
    public bool isNew = true;
    public bool isDead = false;
    public int experience;
    public int experienceToNextLevel;
    public Type enemyType;

    public UnitSave()
    {

    }

    public UnitSave(Unit u, bool isNew)
    {
        this.isNew = isNew;

        if (u == null)
        {
            return;
        }

        unitName = u.unitName;
        unitLevel = u.unitLevel;
        damage = u.damage;
        maxHP = u.maxHP;
        currentHP = u.currentHP;
        offensiveMove1Name = u.offensiveMove1Name;
        offensiveMove2Name = u.offensiveMove2Name;
        defensiveMoveName = u.defensiveMoveName;
        experience = u.experience;
        experienceToNextLevel = u.experienceToNextLevel;
        isDead = u.isDead;

        if (u.enemyMoves != null && u.enemyMoves.Any())
        {
            enemyMoves = new string[u.enemyMoves.Count];
            for (var i = 0; i < u.enemyMoves.Count; i++)
            {
                enemyMoves[i] = u.enemyMoves[i];
            }
        }
    }

    public UnitSave(UnitSave u)
    {
        isNew = u.isNew;
        unitName = u.unitName;
        unitLevel = u.unitLevel;
        damage = u.damage;
        maxHP = u.maxHP;
        currentHP = u.currentHP;
        offensiveMove1Name = u.offensiveMove1Name;
        offensiveMove2Name = u.offensiveMove2Name;
        defensiveMoveName = u.defensiveMoveName;
        experience = u.experience;
        experienceToNextLevel = u.experienceToNextLevel;
        isDead = u.isDead;

        if (u.enemyMoves != null && u.enemyMoves.Length > 0)
        {
            enemyMoves = new string[u.enemyMoves.Length];
            for (var i = 0; i < u.enemyMoves.Length; i++)
            {
                enemyMoves[i] = u.enemyMoves[i];
            }
        }
    }

    public UnitSave Copy()
    {
        return new UnitSave(this);
    }
}
