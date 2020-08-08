using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public DungeonRoomManager currentRoom;
    public PartyMover party;

    public GameObject isDeadMessage;

    public bool canMove = true;

    private CameraDrag _camera;
    private DungeonGeneratorV2 _generator;
    private RewindManager _rewind;

    void Start()
    {
        _camera = FindObjectOfType<CameraDrag>();
        _generator = FindObjectOfType<DungeonGeneratorV2>();
        _rewind = FindObjectOfType<RewindManager>();
    }

    public void DeactivateMovement()
    {
        isDeadMessage.SetActive(true);
        canMove = false;
    }

    public void ActivateMovement()
    {
        isDeadMessage.SetActive(false);
        canMove = true;
    }

    public bool CanMove()
    {
        return canMove;
    }

    public void SetCurrentRoom(DungeonRoomManager newRoom, bool withRewind = true, UnitSave playerUnit = null)
    {
        if (currentRoom != null)
        {
            foreach (var room in currentRoom.adjacentRooms)
            {
                room.HideRoom();
            }

            currentRoom.name = currentRoom.name.Substring(1);
            currentRoom.isCurrent = false;
            currentRoom.Reset();
            currentRoom.HideRoom();
        }

        currentRoom = newRoom;
        currentRoom.isCurrent = true;
        currentRoom.name = $"*{currentRoom.name}";
        currentRoom.SetColor(Color.blue);

        foreach (var room in currentRoom.adjacentRooms)
        {
            room.ShowRoom();
        }

        if (_camera == null)
        {
            _camera = FindObjectOfType<CameraDrag>();
        }

        _camera.SetTarget(currentRoom.transform);
        party.SetTarget(currentRoom.transform);

        if (_rewind == null)
        {
            _rewind = FindObjectOfType<RewindManager>();
        }

        if (currentRoom.roomEvent != null && !currentRoom.roomEvent.isExecuted)
        {
            currentRoom.roomEvent.Execute();
        }

        if (withRewind)
        {
            _rewind.Push(currentRoom, playerUnit);
        }

        if (currentRoom.roomType == RoomTypes.End)
        {
            StartCoroutine(NextDungeon());
        }
    }

    IEnumerator NextDungeon()
    {
        _generator.DeactivateMovement();
        _rewind.DeactivateButtons();

        if (_generator.level >= 7)
        {
            StartCoroutine(GetComponent<SceneFader>().FadeAndLoadScene(SceneFader.FadeDirection.In, "credits"));
            yield return null;
        } else
        {
            yield return new WaitForSeconds(0.6f);

            GlobalControl.instance.rewindStack = new Stack<DungeonRoomStackState>();
            GlobalControl.instance.dungeonSave.isNew = true;
            PlayerPrefs.SetInt("CurrentLevel", PlayerPrefs.HasKey("CurrentLevel") ? PlayerPrefs.GetInt("CurrentLevel") + 1 : 2);

            PlayerPrefs.SetString("NextScene", "test_v2");

            SceneManager.LoadScene("loading_screen");
        }
    }
}
