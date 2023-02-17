using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DungeonDrawer
{
    public static void Draw(List<Vector3> data, GameObject parentObj, PrimitiveType type)
    {
        foreach (var position in data)
        {
            var instance = GameObject.CreatePrimitive(type);
            instance.transform.position = position;
            instance.transform.parent = FindDungeonDrawer(parentObj).transform;
        }
    }

    //return the drawn GameObject
    public static GameObject DrawSingleRoom(Vector3 position, PrimitiveType type, GameObject parentObj, Vector3 roomScale)
    {
        var obj = GameObject.CreatePrimitive(type);
        obj.transform.position = position;
        obj.transform.localScale = roomScale;
        obj.transform.parent = FindDungeonDrawer(parentObj).transform;
        return obj;
    }

    public static List<GameObject> DrawRooms(List<Vector3> positions, PrimitiveType type, GameObject parentObj, Vector3 roomScale)
    {
        var roomsObj = new List<GameObject>();
        foreach(var position in positions)
        {
            var obj = GameObject.CreatePrimitive(type);
            obj.transform.position = position;
            obj.transform.localScale = roomScale;
            obj.transform.parent = FindDungeonDrawer(parentObj).transform;
            roomsObj.Add(obj);
        }
        return roomsObj;
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
