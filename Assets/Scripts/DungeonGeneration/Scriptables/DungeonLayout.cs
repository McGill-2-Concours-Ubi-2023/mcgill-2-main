using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New dungeon layout", menuName = "Dungeon layout")]
public class DungeonLayout : ScriptableObject
{
    [SerializeField]
    private List<Vector3> wallsData;
    [SerializeField]
    private List<Vector3> floorData;
    [SerializeField]
    private Vector3 startRoomPosition;

    public string GetName()
    {
        return name;
    }

    public List<Vector3> GetWallsData()
    {
        return wallsData;
    }

    public List<Vector3> GetFloorData()
    {
        return floorData;
    }

    public void SaveWallsData(List<Vector3> data)
    {
        wallsData = data;
    }

    public void SaveFloorData(List<Vector3> data)
    {
        floorData = data;
    }

    public void SaveStartPosition(Vector3 position)
    {
        startRoomPosition = position;
    }
    public Vector3 GetStartPosition()
    {
        return startRoomPosition;
    }
}
