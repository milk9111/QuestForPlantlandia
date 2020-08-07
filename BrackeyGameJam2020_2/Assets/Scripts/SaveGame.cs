using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveGame
{
    public static void Save(GameStateSaveData saveData)
    {
        var formatter = new BinaryFormatter();
        var path = Application.persistentDataPath + "/gamestate.save";

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        var stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, saveData);
        stream.Close();
    }

    public static GameStateSaveData Load()
    {
        var path = Application.persistentDataPath + "/gamestate.save";
        if (!File.Exists(path))
        {
            Debug.Log("File not found! Could not load game.");
            return null;
        }

        var formatter = new BinaryFormatter();
        var stream = new FileStream(path, FileMode.Open);

        var gameState = (GameStateSaveData)formatter.Deserialize(stream);
        stream.Close();

        return gameState;
    }

    public static bool Exists()
    {
        return File.Exists(Application.persistentDataPath + "/gamestate.save");
    }

    public static void Delete()
    {
        var path = Application.persistentDataPath + "/gamestate.save";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static string GetPath()
    {
        return Application.persistentDataPath + "/gamestate.save";
    }
}
