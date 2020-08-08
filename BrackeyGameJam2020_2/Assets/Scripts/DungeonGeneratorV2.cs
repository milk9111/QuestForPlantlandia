using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DungeonGeneratorV2 : MonoBehaviour
{
    public Vector2 worldSize = new Vector2(4, 4);
    public DungeonRoom[,] dungeonRooms;

	public List<Vector2> eventProbabilities;

    public IList<Vector2> takenPositions = new List<Vector2>();

    public int gridSizeX, gridSizeY, numberOfRooms = 20;
	public int level;

	public float randomCompare = 0.2f, randomCompareStart = 0.2f, randomCompareEnd = 0.01f;

	public IDictionary<Vector2, DungeonRoomManager> drawnRooms = new Dictionary<Vector2, DungeonRoomManager>();

	public GameObject roomWhiteObj;
	public GameObject doorObj;
	public GameObject enemyObj;
	public GameObject treasureObj;
	public GameObject partyObj;

	public Text floorText;

	public Transform mapRoot;

    void Start()
    {
		BuildDungeon(PlayerPrefs.HasKey("CurrentLevel") ? PlayerPrefs.GetInt("CurrentLevel") : 1);
    }

	public void BuildDungeon(int level)
    {
		if (GlobalControl.instance.rewindStack.Any())
        {
			GlobalControl.instance.playerSave = GlobalControl.instance.rewindStack.Peek().playerUnit.Copy();
        }

		GlobalControl.instance.enemySave.isNew = true;

		this.level = level;

		floorText.text = $"Floor {this.level}";
		if (this.level == 7)
        {
			floorText.text = "Final Floor!";
        }

		var lm = FindObjectOfType<LevelManager>();

		if (!GlobalControl.instance.dungeonSave.isNew)
		{
			worldSize = GlobalControl.instance.dungeonSave.worldSize;
			dungeonRooms = GlobalControl.instance.dungeonSave.dungeonRooms;
			eventProbabilities = GlobalControl.instance.dungeonSave.eventProbabilities;
			takenPositions = GlobalControl.instance.dungeonSave.takenPositions;
			gridSizeX = GlobalControl.instance.dungeonSave.gridSizeX;
			gridSizeY = GlobalControl.instance.dungeonSave.gridSizeY;
			numberOfRooms = GlobalControl.instance.dungeonSave.numberOfRooms;

			DrawMap();
			CalculateAdjacentRooms();

			foreach (var room in GlobalControl.instance.dungeonSave.drawnRooms.Values)
			{
				drawnRooms[room.gridPos].SetRoom(room);
				drawnRooms[room.gridPos].HideRoom();

				if (room.roomEvent.type == typeof(EnemyRoomEvent))
				{
					var enemy = Instantiate(enemyObj, drawnRooms[room.gridPos].transform);
					var e = enemy.GetComponent<EnemyRoomEvent>();
					e.generator = this;
					e.isExecuted = room.roomEvent.isExecuted;
					drawnRooms[room.gridPos].AddRoomEvent(e);
				}
				else if (room.roomEvent.type == typeof(TreasureRoomEvent))
				{
					var treasure = Instantiate(treasureObj, drawnRooms[room.gridPos].transform);
					var t = treasure.GetComponent<TreasureRoomEvent>();
					t.isExecuted = room.roomEvent.isExecuted;
					drawnRooms[room.gridPos].AddRoomEvent(t);
				}
			}

			var fr = drawnRooms.First().Value;
			fr.SetBaseColor(Color.green);
			fr.SetRoomType(RoomTypes.Start);
			fr.HideRoom();

			var lr = drawnRooms.Last().Value;
			lr.SetBaseColor(Color.red);
			lr.SetRoomType(RoomTypes.End);
			lr.HideRoom();

			var p = Instantiate(partyObj, GlobalControl.instance.dungeonSave.partyPosition, Quaternion.identity);
			lm.party = p.GetComponent<PartyMover>();

			var rm = FindObjectOfType<RewindManager>();
			if (GlobalControl.instance.rewindStack.Any())
			{
				rm.InitFromGlobal();
			}

			if (GlobalControl.instance.playerSave.isDead)
            {
				lm.DeactivateMovement();
            }

			var current = drawnRooms.First(kv => kv.Value.isCurrent).Value;

			Camera.main.transform.position = new Vector3(current.transform.position.x + 10, current.transform.position.y + 10, -10);

			lm.SetCurrentRoom(current, false);

			return;
		}

		numberOfRooms += 5 * (level - 1);

		if (numberOfRooms >= (worldSize.x * 2) * (worldSize.y * 2))
		{
			numberOfRooms = Mathf.RoundToInt((worldSize.x * 2) * (worldSize.y * 2));
		}

		gridSizeX = Mathf.RoundToInt(worldSize.x);
		gridSizeY = Mathf.RoundToInt(worldSize.y);

		CreateRooms();
		SetRoomDoors();
		DrawMap();
		CalculateAdjacentRooms();

		var firstRoom = drawnRooms.First().Value;
		firstRoom.SetBaseColor(Color.green);
		firstRoom.SetRoomType(RoomTypes.Start);
		firstRoom.ShowRoom();

		var lastRoom = drawnRooms.Last().Value;
		lastRoom.SetBaseColor(Color.red);
		lastRoom.SetRoomType(RoomTypes.End);
		lastRoom.HideRoom();

		GenerateEvents(drawnRooms.Select(kv => kv.Value).Where(r => r != firstRoom && r != lastRoom));

		var party = Instantiate(partyObj);
		party.transform.position = firstRoom.transform.position;

		Camera.main.transform.position = new Vector3(firstRoom.transform.position.x - 10, firstRoom.transform.position.y + 10, -10);

		lm.party = party.GetComponent<PartyMover>();

		lm.SetCurrentRoom(firstRoom);
	}

    void CreateRooms()
    {
		dungeonRooms = new DungeonRoom[gridSizeX * 2, gridSizeY * 2];

		dungeonRooms[gridSizeX, gridSizeY] = new DungeonRoom(Vector2.zero, RoomTypes.Start);
		takenPositions.Insert(0, Vector2.zero);
		var checkPos = Vector2.zero;
		
		for (int i = 0; i < numberOfRooms - 1; i++)
		{
			float randomPerc = i / (float)numberOfRooms - 1;
			randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc);

			checkPos = NewPosition();

			if (NumberOfNeighbors(checkPos, takenPositions) > 1 && Random.value > randomCompare)
			{
				int iterations = 0;
				do
				{
					checkPos = SelectiveNewPosition();
					iterations++;
				} while (NumberOfNeighbors(checkPos, takenPositions) > 1 && iterations < 100);
				if (iterations >= 50)
					Debug.LogError("error: could not create with fewer neighbors than : " + NumberOfNeighbors(checkPos, takenPositions));
			}

			dungeonRooms[(int)checkPos.x + gridSizeX, (int)checkPos.y + gridSizeY] = new DungeonRoom(checkPos, 0);
			takenPositions.Insert(0, checkPos);
		}
	}

	public void SetDrawnRooms(IDictionary<Vector2, DungeonRoomManagerSave> rooms)
	{
		foreach (var kv in rooms)
        {
			drawnRooms[kv.Key].SetRoom(kv.Value);
			drawnRooms[kv.Key].HideRoom();
        }
	}

	public IDictionary<Vector2, DungeonRoomManagerSave> GetDrawnRooms()
    {
		return drawnRooms.ToDictionary(kv => kv.Key, kv => new DungeonRoomManagerSave
		{
			room = kv.Value.room,
			gridPos = kv.Value.gridPos,
			color = new Color(kv.Value.color.r, kv.Value.color.g, kv.Value.color.b, kv.Value.color.a),
			isShowing = kv.Value.isShowing,
			isFound = kv.Value.isFound,
			isCurrent = kv.Value.isCurrent,
			roomEvent = new RoomEventSave
			{
				isExecuted = kv.Value.roomEvent?.isExecuted ?? false,
				type = kv.Value.roomEvent?.GetType() ?? null
			},
			position = new Vector3(kv.Value.transform.position.x, kv.Value.transform.position.y, kv.Value.transform.position.z),
			roomType = kv.Value.roomType
	});
    }

	private void GenerateEvents(IEnumerable<DungeonRoomManager> rooms)
    {
		foreach (var drawnRoom in rooms)
		{
			var prob = eventProbabilities[Random.Range(0, eventProbabilities.Count)];
			var type = (EventTypes)(int)prob.x;

			if (Random.value > prob.y)
            {
				drawnRoom.roomEvent = null;
				continue;
            }

			switch (type)
            {
				case EventTypes.Enemy:
					var enemy = Instantiate(enemyObj, drawnRoom.transform);
					var e = enemy.GetComponent<EnemyRoomEvent>();
					e.generator = this;
					drawnRoom.AddRoomEvent(e);
					break;
				case EventTypes.Treasure:
					var treasure = Instantiate(treasureObj, drawnRoom.transform);
					drawnRoom.AddRoomEvent(treasure.GetComponent<TreasureRoomEvent>());
					break;
            }
        }
    }

	public void DeactivateMovement()
    {
		foreach(var room in drawnRooms.Values)
        {
			room.canMove = false;
        }
    }

	public void ActivateMovement()
	{
		foreach (var room in drawnRooms.Values)
		{
			room.canMove = true;
		}
	}

	private Vector2 NewPosition()
	{
		int x = 0, y = 0;
		Vector2 checkingPos = Vector2.zero;
		do
		{
			int index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1)); 
			x = (int)takenPositions[index].x;
			y = (int)takenPositions[index].y;
			bool upDown = (Random.value < 0.5f);
			bool positive = (Random.value < 0.5f);

			if (upDown)
			{ 
				if (positive)
				{
					y += 1;
				}
				else
				{
					y -= 1;
				}
			}
			else
			{
				if (positive)
				{
					x += 1;
				}
				else
				{
					x -= 1;
				}
			}

			checkingPos = new Vector2(x, y);
		} while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY); 

		return checkingPos;
	}

	private void DrawMap()
	{
		for (var x = 0; x < gridSizeX * 2; x++)
		{
			for (var y = 0; y < gridSizeY * 2; y++)
            {
				var room = dungeonRooms[x, y];

				if (room == null)
				{
					continue;
				}

				var drawPos = new Vector2((x * 2) + (y * 2), y * -1 + x);

				var r = Instantiate(roomWhiteObj, drawPos, Quaternion.identity);
				r.transform.parent = mapRoot;
				var rMan = r.GetComponent<DungeonRoomManager>();

				if (room.doorUL)
				{
					var door = Instantiate(doorObj, r.transform);
					door.transform.localPosition = new Vector2(-1f, 0.5f);
					rMan.AddDoor(door);
				}

				if (room.doorUR)
				{
					var door = Instantiate(doorObj, r.transform);
					door.transform.localPosition = new Vector2(1f, 0.5f);
					rMan.AddDoor(door);
				}

				if (room.doorBL)
				{
					var door = Instantiate(doorObj, r.transform);
					door.transform.localPosition = new Vector2(-1f, -0.5f);
					rMan.AddDoor(door);
				}

				if (room.doorBR)
				{
					var door = Instantiate(doorObj, r.transform);
					door.transform.localPosition = new Vector2(1f, -0.5f);
					rMan.AddDoor(door);
				}

				rMan.gridPos = room.gridPos;
				rMan.room = room;
				rMan.HideRoom();
				drawnRooms.Add(room.gridPos, rMan);
			}
		}
	}

	private void CalculateAdjacentRooms()
    {
		for (var x = 0; x < gridSizeX * 2; x++)
		{
			for (var y = 0; y < gridSizeY * 2; y++)
			{
				var room = dungeonRooms[x, y];
				if (room == null)
				{
					continue;
				}

				var rMan = drawnRooms.ContainsKey(room.gridPos) ? drawnRooms[room.gridPos] : null;
				if (rMan == null)
				{
					continue;
				}

				var vx = (int)room.gridPos.x;
				var vy = (int)room.gridPos.y;

				var lGridPos = new Vector2(vx - 1, vy);
				// left
				if (vx - 1 < gridSizeX && drawnRooms.ContainsKey(lGridPos))
				{
					var l = drawnRooms.ContainsKey(lGridPos) ? drawnRooms[lGridPos] : null;
					if (l != null)
					{
						rMan.AddAdjacentRoom(l);
					}
				}

				var rGridPos = new Vector2(vx + 1, vy);
				// right
				if (vx + 1 > -gridSizeX && drawnRooms.ContainsKey(rGridPos))
				{
					var r = drawnRooms.ContainsKey(rGridPos) ? drawnRooms[rGridPos] : null;
					if (r != null)
					{
						rMan.AddAdjacentRoom(r);
					}
				}

				var tGridPos = new Vector2(vx, vy - 1);
				// top
				if (vy - 1 < gridSizeY && drawnRooms.ContainsKey(tGridPos))
				{
					var t = drawnRooms.ContainsKey(tGridPos) ? drawnRooms[tGridPos] : null;
					if (t != null)
					{
						rMan.AddAdjacentRoom(t);
					}
				}

				var bGridPos = new Vector2(vx, vy + 1);
				// bottom
				if (vy + 1 > -gridSizeY && drawnRooms.ContainsKey(bGridPos))
				{
					var b = drawnRooms.ContainsKey(bGridPos) ? drawnRooms[bGridPos] : null;
					if (b != null)
					{
						rMan.AddAdjacentRoom(b);
					}
				}
			}
		}
	}

	private Vector2 SelectiveNewPosition()
	{ 
		int index = 0, inc = 0;
		int x = 0, y = 0;
		Vector2 checkingPos = Vector2.zero;
		do
		{
			inc = 0;
			do
			{
				index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1));
				inc++;
			} while (NumberOfNeighbors(takenPositions[index], takenPositions) > 1 && inc < 100);
			x = (int)takenPositions[index].x;
			y = (int)takenPositions[index].y;
			bool UpDown = (Random.value < 0.5f);
			bool positive = (Random.value < 0.5f);
			if (UpDown)
			{
				if (positive)
				{
					y += 1;
				}
				else
				{
					y -= 1;
				}
			}
			else
			{
				if (positive)
				{
					x += 1;
				}
				else
				{
					x -= 1;
				}
			}
			checkingPos = new Vector2(x, y);
		} while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY);
		if (inc >= 100)
		{ 
			Debug.LogError("Error: could not find position with only one neighbor");
		}
		return checkingPos;
	}

	private int NumberOfNeighbors(Vector2 checkingPos, IList<Vector2> usedPositions)
	{
		int ret = 0; // start at zero, add 1 for each side there is already a room
		if (usedPositions.Contains(checkingPos + Vector2.right))
		{ //using Vector.[direction] as short hands, for simplicity
			ret++;
		}
		if (usedPositions.Contains(checkingPos + Vector2.left))
		{
			ret++;
		}
		if (usedPositions.Contains(checkingPos + Vector2.up))
		{
			ret++;
		}
		if (usedPositions.Contains(checkingPos + Vector2.down))
		{
			ret++;
		}
		return ret;
	}

	private void SetRoomDoors()
	{
		for (int x = 0; x < gridSizeX * 2; x++)
		{
			for (int y = 0; y < gridSizeY * 2; y++)
			{
				if (dungeonRooms[x, y] == null)
				{
					continue;
				}

				if (y - 1 < 0)
				{ //check above
					dungeonRooms[x, y].doorUL = false;
				}
				else
				{
					dungeonRooms[x, y].doorUL = dungeonRooms[x, y - 1] != null;
				}

				if (y + 1 >= gridSizeY * 2)
				{ //check below
					dungeonRooms[x, y].doorBR = false;
				}
				else
				{
					dungeonRooms[x, y].doorBR = dungeonRooms[x, y + 1] != null;
				}

				if (x - 1 < 0)
				{ //check left
					dungeonRooms[x, y].doorBL = false;
				}
				else
				{
					dungeonRooms[x, y].doorBL = dungeonRooms[x - 1, y] != null;
				}

				if (x + 1 >= gridSizeX * 2)
				{ //check right
					dungeonRooms[x, y].doorUR = false;
				}
				else
				{
					dungeonRooms[x, y].doorUR = dungeonRooms[x + 1, y] != null;
				}
			}
		}
	}
}
