using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New grid", menuName = "Dungeon grid")]
public class DungeonGrid : DataContainer
{
    [SerializeField]
    [Range(1, 500)]
    private int gridSize;
    [SerializeField]
    [Range(1, 50)]
    private int roomSize = 5;
    [SerializeField]
    [Range(0, 100)]
    private int cellsSpacing = 2;
    private DungeonData data;
    private Dictionary<Vector3, Vector2Int> gridMap;
    private MapManager mapM;

    public void GenerateGrid(DungeonData data)
    {
        ClearGrid();
        this.data = data;
        gridMap = new Dictionary<Vector3, Vector2Int>();        
        var currentPosition = mono.transform.position;
        var offsetx = (roomSize + cellsSpacing) * Vector3.forward;
        var offsetz = (roomSize + cellsSpacing) * Vector3.right;

        for (int i = 0; i < gridSize; i++) // loop over rows
        {
            for (int j = 0; j < gridSize; j++) // loop over columns
            {
                var position = currentPosition + i * offsetx + j * offsetz;
                var gridPosition = new Vector2Int(i, j);
                gridMap.Add(position, gridPosition);
                dataBuffer.Add(position);              
            }
        }
        mapM = FindObjectOfType<MapManager>();
        mapM.Trigger<IDungeonMapTrigger>(nameof(IDungeonMapTrigger.MapGridGeneration)); //start map grid generation after the dungeon grid is done generating
        DungeonDrawer.Draw(dataBuffer, mono, PrimitiveType.Cube);
    }

    public void GenerateRooms(DungeonData data)
    {
        this.data = data;
        var startingPosition = dataBuffer.ElementAt(UnityEngine.Random.Range(0, dataBuffer.Count())); //start at a random position on the grid
        var startingRoom = DungeonRoom.Create(data, startingPosition, gridMap); //create starting room
        data.SetStartingRoom(startingRoom);
        mapM.Trigger<IDungeonMapTrigger>(nameof(IDungeonMapTrigger.GenerateMapRoom), startingRoom.GridPosition(), RoomTypes.RoomType.Start);
        //Keep track of already populated grid points
        var visitedPoints = new HashSet<Vector3>();
        visitedPoints.Add(startingPosition);

        Stack<DungeonRoom> unvisistedRooms = new Stack<DungeonRoom>(); //Stack for depth first traversal
        unvisistedRooms.Push(startingRoom);
        
        for(int i = Mathf.FloorToInt(data.RoomDensity() * gridSize * gridSize); i > 0; i--)
        {
            if(unvisistedRooms.Count > 0)
            {
                var currentRoom = unvisistedRooms.Pop();
                var neighbourPositions = GetAvailableNeighbours(currentRoom, visitedPoints);
                foreach (var selectedPoint in SelectRandomPoints(neighbourPositions, visitedPoints))
                {
                    var newRoom = DungeonRoom.Create(data, selectedPoint, gridMap);
                    unvisistedRooms.Push(newRoom);
                    mapM.Trigger<IDungeonMapTrigger>(nameof(IDungeonMapTrigger.GenerateMapRoom),newRoom.GridPosition(), RoomTypes.RoomType.Normal);//setting to normal roomtype for all rooms for now, change this when roomtype is implemented 
                }
            }         
        }
        Cleanup(visitedPoints);
    }

    private void Cleanup(HashSet<Vector3> visitedPoints)
    {
        ConnectRooms();
        while(data.AllRooms().Count() < data.RoomCount())
        {
            var randomRoom = data.AllRooms().Where(room =>
            GetAvailableNeighbours(room, visitedPoints).Count() > 0)
            .OrderBy(room => room.Id())
            .First();
            var spawnPoint = GetAvailableNeighbours(randomRoom, visitedPoints).First();
            visitedPoints.Add(spawnPoint);
            DungeonRoom.Create(data, spawnPoint, gridMap);           
        }
        ConnectRooms();
    }

    public int RoomSize()
    {
        return roomSize;
    }

    public int Size()
    {
        return gridSize;
    }

    private List<Vector3> GetAvailableNeighbours(DungeonRoom room, HashSet<Vector3> visitedPoints)
    {
        var points = new List<Vector3>();
        GetNeighbouringPoints(room).ForEach(point =>
        {
            if (!visitedPoints.Contains(point)) points.Add(point);
        });
        return points.Intersect(dataBuffer).ToList();
    }

    private List<Vector3> GetNeighbouringPoints(DungeonRoom room)
    {
        var points = new List<Vector3>();
        Direction2D.cardinals.ForEach(cardinal =>
        {
            Vector3 samplePoint = room.transform.position + new Vector3(cardinal.x, 0, cardinal.y) * (roomSize + cellsSpacing);
            if (dataBuffer.Contains(samplePoint))
            {
                points.Add(samplePoint);
            }
        });
        return points;
    }

    private List<DungeonRoom> GetNeighbouringRooms(DungeonRoom room)
    {
        var neighbours = new List<DungeonRoom>();
        Direction2D.cardinals.ForEach(cardinal =>
        {
            Vector3 samplePoint = room.transform.position + new Vector3(cardinal.x, 0, cardinal.y) * (roomSize + cellsSpacing);
            if (dataBuffer.Contains(samplePoint))
            {
                neighbours.AddRange(data.AllRooms().Where(room => room.transform.position == samplePoint));
            }
        });
        return neighbours;
    }

    private List<Vector3> SelectRandomPoints(List<Vector3> points, HashSet<Vector3> visitedPoints)
    {
        int randomNumPoints = UnityEngine.Random.Range(1, points.Count);
        var selectedPoints = points.OrderBy(x => Guid.NewGuid()).Take(randomNumPoints).ToList();
        selectedPoints.ForEach(point => visitedPoints.Add(point));
        return selectedPoints;
    }

    public void ConnectRooms()
    {
        data.AllRooms().ForEach(room =>
        {
            GetNeighbouringRooms(room).ForEach(neighbour =>
            {
                if (!room.GetConnectedRooms().Contains(neighbour))
                    room.ConnectRoom(neighbour);
            });
        });
    }

    public void ClearGrid()
    {
        ClearBuffer();
        DungeonDrawer.EraseDungeon(mono);
    }

    public void LoadRooms(List<Vector3> rooms, DungeonData data)
    {       
        GenerateGrid(data);
        dataBuffer = rooms;
        var roomsObj = DungeonDrawer.DrawRooms(rooms, data.GetRoomPrefab(), mono);
        roomsObj.ForEach(roomObj => 
        {
            var loadedRoom = roomObj.AddComponent<DungeonRoom>();
            loadedRoom.Initialize(gridMap, roomObj.transform.position);
            data.AddRoom(loadedRoom);
        });
        ConnectRooms();
    }

    public void ReloadMiniMap(DungeonData data)
    {      
        data.AllRooms().ForEach(room =>
        {
            if (data.GetStartingRoom().GetPosition() == room.GetPosition())
            {
                mapM.Trigger<IDungeonMapTrigger>(nameof(IDungeonMapTrigger.GenerateMapRoom), room.GridPosition(), RoomTypes.RoomType.Start);
            } else
            {
                mapM.Trigger<IDungeonMapTrigger>(nameof(IDungeonMapTrigger.GenerateMapRoom), room.GridPosition(), RoomTypes.RoomType.Normal);
            }
        });
    }

    public int GridSize() {
        return gridSize;
    }
}
