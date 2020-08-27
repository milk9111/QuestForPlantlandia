using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class GameStateSaveData
{
    public UnitSave player;
    public UnitSave enemy;
    public DungeonSaveData dungeon;
    public List<DungeonRoomStackStateSaveData> stackState;
    public string currentScene;

    public GameStateSaveData(DungeonSave dungeon, List<DungeonRoomStackState> stackState,  UnitSave player, string scene, UnitSave enemy = null)
    {
        this.dungeon = new DungeonSaveData(dungeon);
        this.stackState = stackState.Select(s => new DungeonRoomStackStateSaveData(s)).ToList();
        this.player = player;
        this.currentScene = scene;
        this.enemy = enemy;
    }

    public Stack<DungeonRoomStackState> ToStack()
    {
        var s = new Stack<DungeonRoomStackState>();
        for (var i = stackState.Count - 1; i >= 0; i--)
        {
            s.Push(new DungeonRoomStackState(stackState[i]));
        }

        return s;
    }
}
