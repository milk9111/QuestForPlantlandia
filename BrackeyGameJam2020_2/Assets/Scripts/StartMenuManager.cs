using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    public Button continueGameButton;
    public Button deleteSaveButton;

    public GameObject startMenu;
    public GameObject characterSelection;

    public GameObject geminiObj;
    public GameObject ferbObj;
    public GameObject violaObj;

    public CharacterSelector gemini;
    public CharacterSelector ferb;
    public CharacterSelector viola;

    void Start()
    {
        continueGameButton.interactable = SaveGame.Exists();
        deleteSaveButton.interactable = SaveGame.Exists();

        gemini.Dark();
        ferb.Dark();
        viola.Dark();

        startMenu.SetActive(true);
        characterSelection.SetActive(false);
    }

    public void DeleteGame()
    {
        SaveGame.Delete();
        continueGameButton.interactable = SaveGame.Exists();
        deleteSaveButton.interactable = SaveGame.Exists();
    }

    public void ContinueGame()
    {
        var gameState = SaveGame.Load();

        if (gameState != null)
        {
            GlobalControl.instance.dungeonSave = gameState.dungeon.ToDungeonSave();
            GlobalControl.instance.playerSave = gameState.player;
            GlobalControl.instance.enemySave = gameState.enemy;
            GlobalControl.instance.rewindStack = gameState.ToStack();
            GlobalControl.instance.currentScene = gameState.currentScene;
        }

        PlayerPrefs.SetInt("CurrentLevel", GlobalControl.instance.dungeonSave.level);
        PlayerPrefs.SetString("NextScene", gameState.currentScene);

        GoToScene();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        characterSelection.SetActive(true);
        startMenu.SetActive(false);
    }

    public void StartGame(string heroName)
    {
        PlayerPrefs.SetInt("CurrentLevel", 1);
        GlobalControl.instance.dungeonSave.isNew = true;

        GlobalControl.instance.playerSave.isNew = true;
        if (!string.IsNullOrEmpty(heroName))
        {
            Unit unit = null;
            switch(heroName)
            {
                case "Gemini":
                    unit = geminiObj.GetComponent<Unit>();
                    break;
                case "Ferb":
                    unit = ferbObj.GetComponent<Unit>();
                    break;
                case "Viola":
                    unit = violaObj.GetComponent<Unit>();
                    break;
            }

            GlobalControl.instance.playerSave = new UnitSave(unit, false);
        }
        
        GlobalControl.instance.enemySave.isNew = true;
        GlobalControl.instance.rewindStack = new Stack<DungeonRoomStackState>();
        GlobalControl.instance.currentScene = "test_v2";
        PlayerPrefs.SetString("NextScene", "test_v2");

        GoToScene();
    }

    public void GoToScene()
    {
        SceneManager.LoadScene("loading_screen");
    }
}
