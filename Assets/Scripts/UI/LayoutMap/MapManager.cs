using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour, IDungeonMapTrigger
{
    public DungeonData dungeonData;
    public DungeonGrid dungeonGrid; 
    [SerializeField]
    private GridLayoutGroup mapLayoutGroup;
    //public GameObject map;
    [SerializeField]
    private GameObject roomCellPrefab;
    private List<MapRoom> rooms; // all cells in map, including the nonroom ones 

    public void GenerateRoom(Vector2 pos, RoomTypes.RoomType rt) {
        
    }

    public void MapGridGeneration() {
        int gridSize = dungeonGrid.GridSize();
        Vector2 layoutSize = mapLayoutGroup.gameObject.transform.localScale;
        mapLayoutGroup.cellSize = layoutSize / gridSize;
        int counter = 0;
        for (int i = 0; i < gridSize; i++)
        {
            var room = GameObject.Instantiate(roomCellPrefab);
            MapRoom mr = room.GetComponent<MapRoom>();
            rooms.Add(mr);
            mr.ID = counter;
            counter++;
        }
    }

    private void Start()
    {
        
        
    }


}
