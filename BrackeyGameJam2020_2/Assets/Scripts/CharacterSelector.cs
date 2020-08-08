using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    public Image image;
    public Text nameLabel;
    public Text description;
    public Button selectorButton;

    private StartMenuManager _manager;

    void Start()
    {
        _manager = FindObjectOfType<StartMenuManager>();
    }

    public void Dark()
    {
        image.color = Color.black;
        nameLabel.gameObject.SetActive(false);
        description.gameObject.SetActive(false);
    }

    public void Highlight()
    {
        image.color = Color.white;
        nameLabel.gameObject.SetActive(true);
        description.gameObject.SetActive(true);
    }

    public void Click()
    {
        image.color = Color.gray;
        nameLabel.color = Color.gray;
        description.color = Color.gray;

        _manager.StartGame(gameObject.name);
    }
}
