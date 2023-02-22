using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDungeonMapTrigger : ITrigger
{
    public abstract void MapGridGeneration();
    public abstract void Test();
    public abstract void GenerateMapRoom(Vector2Int pos, RoomTypes.RoomType rt);
    public abstract void ClearMap();
    //public abstract void GenerateMapRooms(Dictionary<Vector3, Vector2Int> position);
}
