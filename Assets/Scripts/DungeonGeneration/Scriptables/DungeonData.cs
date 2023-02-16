using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New dungeon", menuName = "Dungeon asset")]
public class DungeonData :DataContainer
{
    [SerializeField]
    private ScriptableObject activeLayout;
    [SerializeField]
    private ScriptableObject dungeonGrid;
    [SerializeField]
    [Range(0.1f, 1f)]
    private float roomPercent = 0.5f;
    [SerializeField]
    private int roomSize = 10;
    [SerializeField]
    private List<DungeonRoom> rooms = new List<DungeonRoom>();

    public void GenerateDungeon()
    {
        GetGrid().SetMonoInstance(mono);
        ((DungeonGrid)dungeonGrid).GenerateGrid(this);
    }

    public void AddRoom(DungeonRoom room)
    {
        rooms.Add(room);
    }

    public float RoomPercent()
    {
        return roomPercent;
    }

    public int RoomSize()
    {
        return roomSize;
    }

    public DungeonLayout GetActiveLayout()
    {
        return activeLayout as DungeonLayout;
    }

    public DungeonGrid GetGrid()
    {
        return dungeonGrid as DungeonGrid;
    }

    public void ClearDungeon()
    {
        GetGrid().ClearBuffer();
    }

    public void SaveData()
    {
        //throw new NotImplementedException();
    }

    public void LoadData()
    {
        //throw new NotImplementedException();
    }

    public object GetWallsData()
    {
        return null;
        //throw new NotImplementedException();
    }
}
