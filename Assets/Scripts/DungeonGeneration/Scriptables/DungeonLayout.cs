using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New dungeon layout", menuName = "Dungeon layout")]
public class DungeonLayout : ScriptableObject
{
    [SerializeField]
    private List<Vector3> floorData;
    [SerializeField]
    private Vector3 startRoomPosition;

    public string GetName()
    {
        return name;
    }

    public List<Vector3> GetFloorData()
    {
        return floorData;
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
    public void ClearData()
    {
        floorData = new List<Vector3>();
        startRoomPosition = Vector3.zero;
    }

    public bool IsEmpty()
    {
        return floorData.Count == 0; //if no rooms -> empty
    }
}
