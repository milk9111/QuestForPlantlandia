using System;
using UnityEngine;

public class DungeonRoom
{
	public Vector2 gridPos;
	public RoomTypes type;
	public bool doorUL, doorUR, doorBL, doorBR;
	public DungeonRoom(Vector2 _gridPos, RoomTypes _type)
	{
		gridPos = _gridPos;
		type = _type;
	}
}
