using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : DataContainer
{
    [SerializeField]
    private List<DungeonRoom> adjacentRooms;
    // Start is called before the first frame update

    public DungeonRoom(GameObject mono, int roomSize)
    {
        this.mono = mono;
    }

    public static void Create(DungeonData data, Vector3 position)
    {
        DungeonRoom room = new DungeonRoom(data.GetMonoInstance(), data.RoomSize());
        var prefab_ = GameObject.CreatePrimitive(PrimitiveType.Cube); //use the prefab parameter within function instead
        prefab_.transform.position = position;
        prefab_.transform.localScale = new Vector3(data.RoomSize(), 1, data.RoomSize());
        data.AddRoom(room);
    }
}
