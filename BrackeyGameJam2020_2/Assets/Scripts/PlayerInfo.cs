using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public Text nameLabel;
    public Text level;
    public Text totalXP;
    public Text xpToNextLvl;
    public Text damage;
    public Text hp;
    public Text move1;
    public Text move2;
    public Text move3;

    // Start is called before the first frame update
    void Start()
    {
        SetText();
    }

    public void SetText()
    {
        nameLabel.text = GlobalControl.instance.playerSave.unitName;
        level.text = $"Level: {GlobalControl.instance.playerSave.unitLevel}";
        totalXP.text = $"Total XP: {GlobalControl.instance.playerSave.experience}";
        xpToNextLvl.text = $"XP to Next Lvl: {GlobalControl.instance.playerSave.experienceToNextLevel}";
        damage.text = $"Added Damage: {GlobalControl.instance.playerSave.damage}";
        hp.text = $"HP: {GlobalControl.instance.playerSave.maxHP}";
        move1.text = $"Move 1: {GlobalControl.instance.playerSave.offensiveMove1Name}";
        move2.text = $"Move 2: {GlobalControl.instance.playerSave.offensiveMove2Name}";
        move3.text = $"Move 3: {GlobalControl.instance.playerSave.defensiveMoveName}";
    }
}
