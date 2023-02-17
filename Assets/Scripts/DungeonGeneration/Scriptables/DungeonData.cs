using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New dungeon", menuName = "Dungeon asset")]
public class DungeonData :DataContainer, IDungeonTrigger
{
    [SerializeField]
    private ScriptableObject activeLayout;
    [SerializeField]
    private ScriptableObject dungeonGrid;
    [SerializeField]
    [Range(0.1f, 1f)]
    private float roomPercent = 0.5f;
    [SerializeField]
    private List<DungeonRoom> rooms = new List<DungeonRoom>();
    [SerializeField]
    private DungeonRoom startingRoom;

    public void GenerateDungeon()
    {
        ClearDungeon();
        rooms.Clear();
        GetGrid().SetMonoInstance(mono);
        GetGrid().GenerateGrid();
        GetGrid().GenerateRooms(this);
        GetGrid().ConnectMissingRooms(this);
        GetGrid().DeleteRandomRooms(this);       
    }

    public void AddRoom(DungeonRoom room)
    {
        rooms.Add(room);
    }

    public void ReceiveMessage(string message)
    {
        Debug.Log(message);
    }

    public void SetStartingRoom(DungeonRoom room)
    {
        startingRoom = room;
    }

    public DungeonRoom GetStartingRoom()
    {
        return startingRoom;
    }

    public float RoomPercent()
    {
        return roomPercent;
    }

    public DungeonLayout GetActiveLayout()
    {
        return activeLayout as DungeonLayout;
    }

    public DungeonGrid GetGrid()
    {
        return dungeonGrid as DungeonGrid;
    }

    public List<DungeonRoom> AllRooms()
    {
        return rooms;
    }

    public void ClearDungeon()
    {
        rooms.Clear();
        GetGrid().ClearBuffer();
        DungeonDrawer.EraseDungeon(mono);
    }

    public void SaveData()
    {
        var roomsPositions = new List<Vector3>();
        rooms.ForEach(room =>
        {
            roomsPositions.Add(room.transform.position);
        });
        GetActiveLayout().SaveFloorData(roomsPositions);
        GetActiveLayout().SaveStartPosition(startingRoom.transform.position);
    }

    public void LoadData()
    {
        ClearDungeon();
        GetGrid().LoadRooms(GetActiveLayout().GetFloorData(), this);
        startingRoom = rooms.Where(room => room.transform.position == GetActiveLayout().GetStartPosition()).First();
    }

    public object GetWallsData()
    {
        return null;
    }
}
