using System;

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
    public bool isNew = true;
    public int experience;
    public int experienceToNextLevel;
    public Type enemyType;

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
    }

    public UnitSave Copy()
    {
        return new UnitSave(this);
    }
}
