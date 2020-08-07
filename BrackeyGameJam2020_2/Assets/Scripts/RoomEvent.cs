using System;
using UnityEngine;

public abstract class RoomEvent : MonoBehaviour
{
    public bool isExecuted = false;

    public abstract void Execute();
}
