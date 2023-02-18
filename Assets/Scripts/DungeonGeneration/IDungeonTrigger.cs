using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDungeonMapTrigger : ITrigger
{
    public abstract void MapGridGeneration();

    public abstract void GenerateRoom(Vector2 pos, RoomTypes.RoomType rt);

}
