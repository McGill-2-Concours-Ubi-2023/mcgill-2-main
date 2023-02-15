using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DungeonLayoutDrawer
{
    public static void DrawFloor(List<Vector3> floorData, GameObject mono)
    {
        foreach (var position in floorData)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = position;
            cube.transform.parent = FindDungeonDrawer(mono).transform;
        }
    }

    public static void DrawWalls(List<Vector3> wallsData, GameObject mono)
    {
        foreach (var position in wallsData) 
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = position;
            sphere.transform.parent = FindDungeonDrawer(mono).transform;
        }
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
