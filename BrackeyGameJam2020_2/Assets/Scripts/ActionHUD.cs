using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionHUD : MonoBehaviour
{
    public Button attackButton1;
    public Button attackButton2;
    public Button defenseButton;
    public Button itemButton;

    public void Disable()
    {
        attackButton1.interactable = false;
        attackButton2.interactable = false;
        defenseButton.interactable = false;
        itemButton.interactable = false;
    }

    public void Enable()
    {
        attackButton1.interactable = true;
        attackButton2.interactable = true;
        defenseButton.interactable = true;
        itemButton.interactable = true;
    }

    public void SetText(string aB1, string aB2, string dM)
    {
        attackButton1.GetComponentInChildren<Text>().text = aB1;
        attackButton2.GetComponentInChildren<Text>().text = aB2;
        defenseButton.GetComponentInChildren<Text>().text = dM;
    }
}
