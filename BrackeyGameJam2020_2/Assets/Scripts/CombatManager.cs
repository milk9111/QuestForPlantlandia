using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public Character currentCharacter;
    public Enemy enemy;

    public Button attackButton1;
    public Button attackButton2;
    public Button defenseButton;
    public Button itemButton;

    public string currentTurn;

    void Start()
    {
        attackButton1.GetComponentInChildren<Text>().text = currentCharacter.offensiveMove1.GetName();
        attackButton2.GetComponentInChildren<Text>().text = currentCharacter.offensiveMove2.GetName();
        defenseButton.GetComponentInChildren<Text>().text = currentCharacter.defensiveMove.GetName();

        attackButton1.onClick.AddListener(currentCharacter.ExecuteOffensiveMove1);
        attackButton2.onClick.AddListener(currentCharacter.ExecuteOffensiveMove2);
        defenseButton.onClick.AddListener(currentCharacter.ExecuteDefensiveMove);

        currentTurn = currentCharacter.characterName;
    }

    public void EndTurn()
    {
        currentTurn = string.Equals(currentTurn, currentCharacter.characterName) ? "Enemy" : currentCharacter.characterName;
        if (!string.Equals(currentTurn, currentCharacter.characterName))
        {
            DeactivateButtons();
        }
    }

    public void DeactivateButtons()
    {
        attackButton1.interactable = false;
        attackButton2.interactable = false;
        defenseButton.interactable = false;
        itemButton.interactable = false;
    }
}
