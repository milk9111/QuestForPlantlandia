using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonRoomManager : MonoBehaviour
{
    public DungeonRoom room;
    public Vector2 gridPos;
    public Color color;
    public List<GameObject> doors = new List<GameObject>();
    public List<DungeonRoomManager> adjacentRooms = new List<DungeonRoomManager>();
    public bool isShowing = false;
    public bool isFound = false;
    public bool isCurrent = false;
    public bool canMove = true;
    public RoomTypes roomType = RoomTypes.Normal;
    public RoomEvent roomEvent;

    private SpriteRenderer _renderer;
    private LevelManager _levelManager;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _levelManager = FindObjectOfType<LevelManager>();
        gridPos = room.gridPos;
    }

    void OnMouseEnter()
    {
        if (!isShowing || !canMove || !_levelManager.CanMove())
        {
            return;
        }

        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 0.75f);
    }

    void OnMouseExit()
    {
        if (!isShowing || !canMove || !_levelManager.CanMove())
        {
            return;
        }

        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 1f);
    }

    void OnMouseUp()
    {
        if (!isShowing || !canMove || !_levelManager.CanMove())
        {
            return;
        }

        _levelManager.SetCurrentRoom(this);
    }

    public void SetBaseColor(Color nColor)
    {
        color = nColor;
        SetColor(color);
    }

    public void AddAdjacentRoom(DungeonRoomManager rMan)
    {
        if (adjacentRooms.Contains(rMan))
        {
            return;
        }

        adjacentRooms.Add(rMan);
    }

    public void SetColor(Color nColor)
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        _renderer.color = nColor;
    }

    public void Reset()
    {
        SetColor(color);
    }

    public void HideRoom()
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        var a = isFound ? 0.4f : 0f;

        foreach(var door in doors)
        {
            door.GetComponent<SpriteRenderer>().color = new Color(Color.white.r, Color.white.g, Color.white.b, a);
        }

        if (roomEvent != null)
        {
            roomEvent.GetComponent<SpriteRenderer>().enabled = false;
        }

        isShowing = false;
        SetColor(new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, a));
    }

    public void ShowRoom()
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        foreach(var door in doors)
        {
            door.GetComponent<SpriteRenderer>().color = Color.white;
        }

        if (roomEvent != null && !roomEvent.isExecuted)
        {
            roomEvent.GetComponent<SpriteRenderer>().enabled = true;
        }

        isFound = true;
        isShowing = true;
        SetColor(new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 1f));
    }

    public void AddRoomEvent(RoomEvent nRoomEvent)
    {
        roomEvent = nRoomEvent;
        roomEvent.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void AddDoor(GameObject door)
    {
        if (doors == null || !doors.Any())
        {
            doors = new List<GameObject>();
        }

        doors.Add(door);
    }

    public void SetRoom(DungeonRoomManagerSave man)
    {
        room = man.room;
        gridPos = man.gridPos;
        isShowing = man.isShowing;
        isFound = man.isFound;
        isCurrent = man.isCurrent;
        roomType = man.roomType;
        if (roomEvent != null)
        {
            roomEvent.isExecuted = man.roomEvent.isExecuted;
            roomEvent.GetComponent<SpriteRenderer>().enabled = !roomEvent.isExecuted;
        }
    }

    public void SetRoomType(RoomTypes type)
    {
        roomType = type;
    } 
}
