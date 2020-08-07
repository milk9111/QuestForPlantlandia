using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public RewindManager rewindManager;

    public DungeonGeneratorV2 generator;

    public BattleSystem battleSystem;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(false);
        generator = FindObjectOfType<DungeonGeneratorV2>();
        battleSystem = FindObjectOfType<BattleSystem>();
        rewindManager = FindObjectOfType<RewindManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            mainMenu.SetActive(!mainMenu.activeSelf);
            if (generator != null)
            {
                if (mainMenu.activeSelf)
                {
                    generator.DeactivateMovement();
                    rewindManager.DeactivateButtons();
                } else
                {
                    generator.ActivateMovement();
                    rewindManager.ActivateButtons();
                }
            }

            if (battleSystem != null)
            {
                if (mainMenu.activeSelf)
                {
                    battleSystem.DeactivateActionHUD();
                }
                else
                {
                    battleSystem.ActivateActionHUD();
                }
            }
        }
    }

    public void GoToStartMenu()
    {
        PlayerPrefs.SetString("NextScene", "start_menu");
        SceneManager.LoadScene("loading_screen");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SaveFromDungeon()
    {
        GlobalControl.instance.dungeonSave = new DungeonSave
        {
            worldSize = generator.worldSize,
            dungeonRooms = generator.dungeonRooms,
            eventProbabilities = generator.eventProbabilities,
            takenPositions = generator.takenPositions,
            gridSizeX = generator.gridSizeX,
            gridSizeY = generator.gridSizeY,
            numberOfRooms = generator.numberOfRooms,
            level = generator.level,
            drawnRooms = generator.drawnRooms.ToDictionary(kv => kv.Key, kv => new DungeonRoomManagerSave
            {
                room = kv.Value.room,
                gridPos = kv.Value.gridPos,
                color = kv.Value.color,
                isShowing = kv.Value.isShowing,
                isFound = kv.Value.isFound,
                isCurrent = kv.Value.isCurrent,
                position = kv.Value.transform.position,
                roomType = kv.Value.roomType,
                roomEvent = new RoomEventSave
                {
                    isExecuted = kv.Value.roomEvent?.isExecuted ?? false,
                    type = kv.Value.roomEvent?.GetType() ?? null
                }
            }),
            partyPosition = FindObjectOfType<LevelManager>().party.transform.position,
            isNew = false
        };

        SaveGame.Save(new GameStateSaveData(GlobalControl.instance.dungeonSave, GlobalControl.instance.rewindStack.ToList(), GlobalControl.instance.playerSave, "test_v2", new UnitSave(null, true)));
    }

    public void SaveFromCombat()
    {
        SaveGame.Save(new GameStateSaveData(GlobalControl.instance.dungeonSave, GlobalControl.instance.rewindStack.ToList(), new UnitSave(battleSystem.playerUnit, false), "test_combat", new UnitSave(battleSystem.enemyUnit, false)));
    }
}
