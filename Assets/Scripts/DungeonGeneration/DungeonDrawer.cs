using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class DungeonDrawer
{
    public static void DrawWithPrimitive(List<Vector3> data, GameObject parentObj, PrimitiveType type)
    {
        foreach (var position in data)
        {
            var instance = GameObject.CreatePrimitive(type);
            instance.transform.position = position;
            instance.transform.parent = FindDungeonDrawer(parentObj).transform;
        }
    }

    //return the drawn GameObject
    public static GameObject DrawSingleObject(Vector3 position, GameObject prefab, GameObject parentObj)
    {
        var obj = GameObject.Instantiate(prefab);
        obj.transform.position = position;
        obj.transform.parent = parentObj.transform;
        return obj;
    }

    public static GameObject DrawRandomRoom(Vector3 position, RoomTypes.RoomType roomType, DungeonData data)
    {
        string roomTypeName = Enum.GetName(typeof(RoomTypes.RoomType), roomType);
        MethodInfo method = typeof(DungeonData).GetMethod("Get" + roomTypeName +"RoomPrefabs", BindingFlags.Public | BindingFlags.Instance);
        GameObject[] roomPrefabs = (GameObject[])method.Invoke(data, null);
        int randIndex = UnityEngine.Random.Range(0, roomPrefabs.Length);
        var obj = GameObject.Instantiate(roomPrefabs[randIndex]);
        obj.transform.position = position;
        obj.transform.parent = FindDungeonDrawer(data.GetMonoInstance()).transform;
        RoomData newRoomData = new RoomData(position, roomType, randIndex);
        data.AddRoomData(newRoomData);
        return obj;
    }

    //Runtime function, won't work in Editor
    public static GameObject ReplaceRoom(DungeonRoom room, DungeonData dungeonData,
        GameObject roomPrefab, RoomTypes.RoomType type, bool isolate)
    {
        room.MoveDoorsUp();
        room.GetWalls().transform.parent = room.transform; //Move walls upwards
        DungeonData.SafeDestroy(room.transform.Find("RoomRoot").gameObject);//delete the root
        RoomData roomData = dungeonData.GetActiveLayout().GetRoomData(room);
        GameObject go = new GameObject("RoomRoot");
        go.transform.position = room.GetPosition();
        go.transform.parent = room.transform;
        GameObject obj = GameObject.Instantiate(roomPrefab);
        roomData.SetRoomType(type);
        roomData.SetIsolated(true);//Serialize it
        GameObject[] roomOptions = dungeonData.GetRoomOverrides();
        int newPrefabIndex = 0;
        for(int i = 0; i<roomOptions.Length; i++)
        {
            if (roomOptions[i] == roomPrefab)
            {
                newPrefabIndex = i;
                break;
            }
        }
        roomData.SetOverride(true, newPrefabIndex);
        obj.transform.position = go.transform.position;       
        room.ReassignRoom( type);
        obj.transform.parent = go.transform;
        if(isolate) room.Isolate();
        foreach (var door in room.GetDoors())
        {
            door.transform.parent = go.transform; //move doors upwards
        }
        room.GetWalls().transform.parent = go.transform;
        return room.gameObject;
    }

    public static GameObject DrawRoomFromData(RoomData roomData, DungeonData dungeonData)
    {      
        GameObject obj;
        if (roomData.IsOverride())
        {
            GameObject[] roomPrefabs = dungeonData.GetRoomOverrides();
            obj = GameObject.Instantiate(roomPrefabs[roomData.GetPrefabIndex()]);
        } else
        {
            string roomTypeName = roomData.GetRoomType().ToString();
            MethodInfo method = typeof(DungeonData).GetMethod("Get" + roomTypeName + "RoomPrefabs", BindingFlags.Public | BindingFlags.Instance);
            GameObject[] roomPrefabs = (GameObject[])method.Invoke(dungeonData, null);
            obj = GameObject.Instantiate(roomPrefabs[roomData.GetPrefabIndex()]);
        }
        obj.transform.position = roomData.GetPosition();
        obj.transform.parent = FindDungeonDrawer(dungeonData.GetMonoInstance()).transform;
        dungeonData.AddRoomData(roomData);
        return obj;
    }

    public static List<GameObject> DrawObjects(List<Vector3> positions, GameObject prefab, GameObject parentObj)
    {
        var objList = new List<GameObject>();
        foreach(var position in positions)
        {
            var obj = GameObject.Instantiate(prefab);
            obj.transform.position = position;
            obj.transform.parent = FindDungeonDrawer(parentObj).transform;
            objList.Add(obj);
        }
        return objList;
    }

    public static void EraseDungeon(GameObject mono)  
    {
        GameObject.DestroyImmediate(FindDungeonDrawer(mono));
    }

    private static GameObject FindDungeonDrawer(GameObject mono) //Find the dungeon "objects" under the current mono behaviour
    {
        GameObject dungeonDrawer;
        if (GameObject.Find("Dungeon_drawer") == null) 
        {
            dungeonDrawer = new GameObject("Dungeon_drawer");
            dungeonDrawer.transform.position = mono.transform.position;
            dungeonDrawer.transform.parent = mono.transform;
        } else
        {
            dungeonDrawer = GameObject.Find("Dungeon_drawer");  
        }
        return dungeonDrawer;
    }
}
