using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New dungeon layout", menuName = "Dungeon layout")]
public class DungeonLayout : ScriptableObject
{
    [SerializeField]
    private List<RoomData> roomsData;

    public string GetName()
    {
        return name;
    }

    public List<RoomData> GetRoomsData()
    {
        return roomsData;
    }

    public void RemoveData(RoomData data)
    {
        if (roomsData.Contains(data)) roomsData.Remove(data);
    }

    public RoomData GetRoomData(DungeonRoom room)
    {
        foreach(RoomData data in roomsData)
        {
            if(room.GetPosition() == data.GetPosition() && data.GetRoomType() == room.GetRoomType())
            {
                return data;
            }
        }
        return null;
    }

    public void SaveRoomsData(List<RoomData> data)
    {
        this.roomsData = data;
    }


    public Vector3 GetStartPosition()
    {
        return roomsData.Where(roomData => roomData.GetRoomType() == RoomTypes.RoomType.Start).ToList().First().GetPosition();
    }

    public void ClearData()
    {
        roomsData = new List<RoomData>();
    }

    public bool IsEmpty()
    {
        return roomsData.Count == 0; //if no rooms -> empty
    }
}
