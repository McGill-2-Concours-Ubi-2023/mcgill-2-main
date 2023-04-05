using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

[CreateAssetMenu(fileName = "New grid", menuName = "Dungeon grid")]
public class DungeonGrid : DataContainer<RoomData>
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
    private Dictionary<Vector3, GameObject> wallsPositions = new Dictionary<Vector3, GameObject>();

    public async Task GenerateGrid(DungeonData data)
    {
        using Stopwatch sw = new Stopwatch(nameof(GenerateGrid));
        await ClearGrid();
        this.data = data;
        gridMap = new Dictionary<Vector3, Vector2Int>();        
        var currentPosition = monoObject.transform.position;
        var offsetx = (roomSize + cellsSpacing) * Vector3.forward;
        var offsetz = (roomSize + cellsSpacing) * Vector3.right;

        for (int i = 0; i < gridSize; i++) // loop over rows
        {
            for (int j = 0; j < gridSize; j++) // loop over columns
            {
                var position = currentPosition + i * offsetx + j * offsetz;
                var gridPosition = new Vector2Int(i, j);
                gridMap.Add(position, gridPosition);
                positionsBuffer.Add(position);           
            }
        }
        mapM = FindObjectOfType<MapManager>();
        mapM.MapGridGeneration(); //start map grid generation after the dungeon grid is done generating
        //DungeonDrawer.DrawWithPrimitive(positionsBuffer, mono, PrimitiveType.Cube);
    }

    public Dictionary<Vector3, GameObject> GetWallsLayout()
    {
        return wallsPositions;
    }

    public void AddWallPosition(Vector3 position, GameObject obj)
    {
        wallsPositions.Add(position, obj);
    }

    public async Task GenerateRooms(DungeonData data)
    {
        using Stopwatch sw = new Stopwatch(nameof(GenerateRooms));
        this.data = data;
        Vector3 startingPosition = positionsBuffer.ElementAt(UnityEngine.Random.Range(0, gridSize)); //start at a random position on the grid
        DungeonRoom startingRoom = await DungeonRoom.CreateRandomRoom(data, startingPosition, gridMap,
            data.GetActiveLayout().GetName(), RoomTypes.RoomType.Start); //create starting room
        data.SetStartingRoom(startingRoom);
        mapM.GenerateMapRoom(startingRoom.GridPosition(), RoomTypes.RoomType.Start);
        //Keep track of already populated grid points
        HashSet<Vector3> visitedPoints = new HashSet<Vector3>();
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
                    var roomType = RoomTypes.GetRandomRoomType();
                    var newRoom = await DungeonRoom.CreateRandomRoom(data, selectedPoint, gridMap, data.GetActiveLayout().GetName(), roomType);
                    await Task.Yield();
                    unvisistedRooms.Push(newRoom);
                    mapM.GenerateMapRoom(newRoom.GridPosition(), RoomTypes.RoomType.Normal);//setting to normal roomtype for all rooms for now, change this when roomtype is implemented 
                }
            }         
        }
        await Cleanup(visitedPoints);
        await Task.Yield();
    }

    private async Task Cleanup(HashSet<Vector3> visitedPoints)
    {
        using Stopwatch sw = new Stopwatch(nameof(Cleanup));
        ConnectRooms();
        while(data.AllRooms().Count() < data.RoomCount())
        {
            var randomRoom = data.AllRooms().Where(room =>
                    GetAvailableNeighbours(room, visitedPoints).Any())
            .OrderBy(room => room.Id())
            .First();
            var spawnPoint = GetAvailableNeighbours(randomRoom, visitedPoints).First();
            visitedPoints.Add(spawnPoint);
            //add rooms while below the threshold
            await DungeonRoom.CreateRandomRoom(data, spawnPoint, gridMap, data.GetActiveLayout().GetName(), RoomTypes.GetRandomRoomType());           
        }
        ConnectRooms();
        data.AllRooms().ForEach(room => room.GenerateDoors(data));
    }

    public void PlaceMerchant ()
    {
        int randomIndex = UnityEngine.Random.Range(0, data.AllRooms().Count);
        var randomRoom = data.AllRooms()[randomIndex];
        var merchant = DungeonDrawer.ReplaceRoom(randomRoom,data,
            data.GetRoomOverrides()[0], RoomTypes.RoomType.Special,false);
        //merchant.SetActive(false);
    }

    public int RoomSize()
    {
        return roomSize;
    }

    public int Size()
    {
        return gridSize;
    }

    public int CellsSpacing()
    {
        return cellsSpacing;
    }

    private List<Vector3> GetAvailableNeighbours(DungeonRoom room, HashSet<Vector3> visitedPoints)
    {
        var points = new List<Vector3>();
        GetNeighbouringPoints(room).ForEach(point =>
        {
            if (!visitedPoints.Contains(point)) points.Add(point);
        });
        return points.Intersect(positionsBuffer).ToList();
    }

    private List<Vector3> GetNeighbouringPoints(DungeonRoom room)
    {
        var points = new List<Vector3>();
        Direction2D.cardinals.ForEach(cardinal =>
        {
            Vector3 samplePoint = room.transform.position + new Vector3(cardinal.x, 0, cardinal.y) * (roomSize + cellsSpacing);
            if (positionsBuffer.Contains(samplePoint))
            {
                points.Add(samplePoint);
            }
        });
        return points;
    }

    public Dictionary<Vector3, Vector2Int> GetGridMap()
    {
        return gridMap;
    }

    private List<DungeonRoom> GetNeighbouringRooms(DungeonRoom room)
    {
        var neighbours = new List<DungeonRoom>();
        Direction2D.cardinals.ForEach(cardinal =>
        {
            Vector3 samplePoint = room.transform.position + new Vector3(cardinal.x, 0, cardinal.y) * (roomSize + cellsSpacing);
            if (positionsBuffer.Contains(samplePoint))
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

    public async Task ClearGrid()
    {
        ClearData();
        wallsPositions = new Dictionary<Vector3, GameObject>();
        await DungeonDrawer.EraseDungeon(monoObject);
    }

    public async Task LoadRooms(DungeonData data)
    {
        await GenerateGrid(data);
        foreach(var roomData in data.GetActiveLayout().GetRoomsData())
        {
            DungeonRoom.CreateRoomFromData(roomData, data, gridMap, data.GetActiveLayout().GetName());
        }
        ConnectRooms();
        data.AllRooms().ForEach(room => room.GenerateDoors(data));
    }

    public async Task ReloadMiniMap(DungeonData data)
    {
        foreach (var room in data.AllRooms())
        {
            if (data.GetStartingRoom().GetPosition() == room.GetPosition())
            {
                mapM.GenerateMapRoom(room.GridPosition(), RoomTypes.RoomType.Start);
            }
            else
            {
                mapM.GenerateMapRoom(room.GridPosition(), RoomTypes.RoomType.Normal);
            }
            await Task.Yield();
        }
    }

    public int GridSize() {
        return gridSize;
    }
}
