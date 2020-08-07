using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewindManager : MonoBehaviour
{
    
    public GameObject rewindButtonObj;
    public RectTransform content;
    public LevelManager levelManager;
    public DungeonGeneratorV2 generator;
    public Scrollbar scrollbar;

    public int move;

    public List<Button> rewindButtons;

    void Start()
    {
        rewindButtons = new List<Button>();
        move = 0;
    }

    public void InitFromGlobal()
    {
        var s = new Stack<DungeonRoomStackState>();
        while (GlobalControl.instance.rewindStack.Any())
        {
            var stack = GlobalControl.instance.rewindStack.Pop();
            s.Push(stack);
        }

        while (s.Any())
        {
            var stack = s.Pop();
            generator.SetDrawnRooms(stack.rooms);
            foreach (var room in stack.rooms)
            {
                if (room.Value.isCurrent)
                {
                    var current = generator.drawnRooms[room.Key];
                    current.SetRoom(room.Value);
                    levelManager.SetCurrentRoom(current, true, stack.playerUnit);
                    break;
                }
            } 
        }
    }

    public void Push(DungeonRoomManager man, UnitSave playerUnit = null)
    {
        if (rewindButtons == null)
        {
            rewindButtons = new List<Button>();
        }

        var bt = Instantiate(rewindButtonObj, content);

        var button = bt.GetComponent<Button>();
        var index = move;

        var trigger = bt.AddComponent<EventTrigger>();

        var pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener(delegate
        {
            man.SetColor(new Color(255, 223, 0, 1));
        });

        var pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener(delegate
        {
            man.Reset();

            if (!man.isShowing)
            {
                man.HideRoom();
            }

            if (man.isCurrent)
            {
                man.SetColor(Color.blue);
            }
        });

        trigger.triggers.Add(pointerEnter);
        trigger.triggers.Add(pointerExit);

        button.onClick.AddListener(delegate {
            for (var i = content.childCount - 1; i > index; i--)
            {
                var b = rewindButtons[i];
                rewindButtons.RemoveAt(i);
                Destroy(b.gameObject);
            }

            DungeonRoomStackState state = null;
            for (var i = content.childCount - 1; i >= index; i--)
            {
                state = GlobalControl.instance.rewindStack.Pop();
                generator.SetDrawnRooms(state.rooms);
                foreach (var room in state.rooms)
                {
                    if (room.Value.isCurrent)
                    {
                        break;
                    }
                }
                GlobalControl.instance.playerSave = state.playerUnit.Copy();
            }

            state.rooms.First(r => r.Key == man.gridPos).Value.isCurrent = true;

            if (state != null)
            {
                GlobalControl.instance.rewindStack.Push(state);
            }

            levelManager.currentRoom.HideRoom();

            move = index + 1;
            scrollbar.value = 1;

            levelManager.SetCurrentRoom(man, false);
        });

        rewindButtons.Add(button);
        move++;        

        content.GetChild(move - 1).SetSiblingIndex(0);

        bt.GetComponentInChildren<Text>().text = $"{move}";

        GlobalControl.instance.rewindStack.Push(new DungeonRoomStackState(generator.GetDrawnRooms(), playerUnit == null ? GlobalControl.instance.playerSave.Copy() : playerUnit.Copy()));
    }

    public void DeactivateButtons()
    {
        foreach(var bt in rewindButtons)
        {
            bt.interactable = false;
        }
    }

    public void ActivateButtons()
    {
        foreach (var bt in rewindButtons)
        {
            bt.interactable = true;
        }
    }
}
