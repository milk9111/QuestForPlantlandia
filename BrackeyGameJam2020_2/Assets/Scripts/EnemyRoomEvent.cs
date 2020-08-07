using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyRoomEvent : RoomEvent
{
    public DungeonGeneratorV2 generator;

    public override void Execute()
    {
        isExecuted = true;
        GetComponent<SpriteRenderer>().enabled = false;
        generator.DeactivateMovement();

        SaveDungeon();

        PlayerPrefs.SetString("NextScene", "test_combat");
        GlobalControl.instance.currentScene = "test_combat";

        StartCoroutine(GoToCombat());
    }

    IEnumerator GoToCombat()
    {
        yield return new WaitForSeconds(0.6f);

        SceneManager.LoadScene("loading_screen");
    }

    private void SaveDungeon()
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
    }
}
