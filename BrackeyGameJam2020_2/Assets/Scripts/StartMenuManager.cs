using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    public Button continueGameButton;
    public Button deleteSaveButton;

    void Start()
    {
        continueGameButton.interactable = SaveGame.Exists();
        deleteSaveButton.interactable = SaveGame.Exists();
        Debug.Log(SaveGame.GetPath());
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
        PlayerPrefs.SetInt("CurrentLevel", 1);
        GlobalControl.instance.dungeonSave.isNew = true;
        GlobalControl.instance.playerSave.isNew = true;
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
