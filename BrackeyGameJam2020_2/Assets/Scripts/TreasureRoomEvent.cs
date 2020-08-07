using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRoomEvent : RoomEvent
{
    public override void Execute()
    {
        isExecuted = true;
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
