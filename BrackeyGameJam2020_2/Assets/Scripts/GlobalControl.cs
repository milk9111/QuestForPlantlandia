using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControl : MonoBehaviour
{
    public static GlobalControl instance;

    public DungeonSave dungeonSave = null;
    public UnitSave playerSave = null;
    public UnitSave enemySave = null;
    public Stack<DungeonRoomStackState> rewindStack = new Stack<DungeonRoomStackState>();
    public string currentScene;

    void Awake()
    {
        if (instance == null)
        {
            PlayerPrefs.SetInt("CurrentLevel", 1);
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else if (instance != null)
        {
            Destroy(gameObject);
        }
    }
}
