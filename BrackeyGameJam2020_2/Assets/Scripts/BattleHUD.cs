using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text nameText;
    public Text levelText;
    public Text hpCounter;
    public Slider hpSlider;
    public GameObject fill;

    public void SetHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        levelText.text = "Lvl " + unit.unitLevel;
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
        hpCounter.text = $"{unit.currentHP} / {unit.maxHP}";
    }

    public void SetHP(int hp)
    {
        hpSlider.value = hp;

        var hpText = hp < 0 ? 0 : hp;

        hpCounter.text = $"{hpText} / {hpSlider.maxValue}";

        if (hpSlider.value <= hpSlider.minValue)
        {
            fill.SetActive(false);
        }
    }
}
