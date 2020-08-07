using System;
using UnityEngine.UI;

public class BattleDataModel
{
    public BattleSystem battleSystem;
    public Unit targetUnit;
    public Unit sourceUnit;
    public BattleHUD targetHUD;
    public BattleHUD sourceHUD;
    public Text dialogueText;

    public BattleDataModel(BattleSystem battleSystem, Unit targetUnit, Unit sourceUnit,
        BattleHUD targetHUD, BattleHUD sourceHUD, Text dialogueText)
    {
        this.battleSystem = battleSystem;
        this.targetUnit = targetUnit;
        this.sourceUnit = sourceUnit;
        this.targetHUD = targetHUD;
        this.sourceHUD = sourceHUD;
        this.dialogueText = dialogueText;
    }
}
