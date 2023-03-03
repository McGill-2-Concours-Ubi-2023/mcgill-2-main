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
    [SerializeField]
    private List<MapRoom> rooms = new List<MapRoom>(); // all cells in map, including the nonroom ones 
    private int gridSize;
    [SerializeField]
    private List<GameObject> roomsObj = new List<GameObject>(); 
    public void GenerateMapRoom(Vector2Int pos, RoomTypes.RoomType rt) {
        int index = pos.y + gridSize * pos.x;
        rooms[index].SetRoom();
        rooms[index].SetType(rt);
    }

    public void MapGridGeneration() {
        gridSize = dungeonGrid.GridSize();
        var rectT = mapLayoutGroup.gameObject.GetComponent<RectTransform>().rect;
        Vector2 layoutSize = new Vector2(rectT.width, rectT.height);
        mapLayoutGroup.cellSize = layoutSize / gridSize;
        for (int i = 0; i < gridSize*gridSize; i++)
        {
            GameObject room = GameObject.Instantiate(roomCellPrefab);
            MapRoom mr = room.GetComponent<MapRoom>();
            rooms.Add(mr);
            room.transform.SetParent(this.transform);
            roomsObj.Add(mr.gameObject);
            mr.SetID(i);
        }
    }

    public void ClearMap() {
        GameObject[] roomsObj1 = GameObject.FindGameObjectsWithTag("MapRoom");
        foreach (var room in roomsObj1) {
            GameObject.DestroyImmediate(room);
        }
        rooms.Clear();
        roomsObj.Clear();
    }

    public void Test()
    {
        Debug.Log("test");
    }

    public void VisitRoom(int roomIndex) {
        Debug.Log(roomIndex);
        roomsObj[roomIndex].GetComponent<MapRoom>().VisitRoom();
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) {
            if (transform.localScale.x == 1)
            {
                RectTransform rect = this.gameObject.GetComponent<RectTransform>();
                rect.localScale = new Vector3(0.3f, 0.3f, 1);
               

            }
            else transform.localScale = Vector3.one;
            
        }
    }
}
