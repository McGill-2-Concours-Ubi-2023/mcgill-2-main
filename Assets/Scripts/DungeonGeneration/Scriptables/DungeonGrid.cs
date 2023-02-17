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
    [SerializeField][Range(.1f, 1)]
    private float roomDensity = 1;
    
    public void GenerateGrid()
    {
        ClearGrid();
        var currentPosition = mono.transform.position;
        var offsetx = (roomSize + cellsSpacing) * Vector3.right;
        var offsetz = (roomSize + cellsSpacing) * Vector3.forward;

        for (int i = 0; i < gridSize * gridSize; i++) //O(n)
        {
            var x = i % gridSize;
            var z = i / gridSize;
            var position = currentPosition + x * offsetx + z * offsetz;
            dataBuffer.Add(position);
        }
        DungeonDrawer.Draw(dataBuffer, mono, PrimitiveType.Cube);
    }

    public void GenerateRooms(DungeonData data)
    {
        var startingPosition = dataBuffer.ElementAt(UnityEngine.Random.Range(0, dataBuffer.Count())); //start at a random position on the grid
        var startingRoom = DungeonRoom.Create(data, startingPosition); //create starting room
        data.SetStartingRoom(startingRoom);

        var visitedPoints = new HashSet<Vector3>(); //Keep track of already populated grid points
        visitedPoints.Add(startingPosition); //Add the starting position to the visited points
        
        Stack<DungeonRoom> unvisistedRooms = new Stack<DungeonRoom>(); //Stack for depth first traversal
        unvisistedRooms.Push(startingRoom);

        for(int i = Mathf.FloorToInt(data.RoomPercent() * gridSize * gridSize); i > 0; i--)
        {
            if(unvisistedRooms.Count > 0)
            {
                var currentRoom = unvisistedRooms.Pop();
                var neighbourPositions = GetNeighbouringPoints(currentRoom, visitedPoints);
                foreach (var selectedPoint in SelectRandomNeighbours(neighbourPositions, visitedPoints))
                {
                    var newRoom = DungeonRoom.Create(data, selectedPoint);
                    currentRoom.ConnectRoom(newRoom);
                    unvisistedRooms.Push(newRoom);
                }
            }         
        }     
    }

    public int RoomSize()
    {
        return roomSize;
    }

    private List<Vector3> GetNeighbouringPoints(DungeonRoom room, HashSet<Vector3> visitedPoints)
    {
        var neightbours = new List<Vector3>();
        Vector3 roomPosition = room.GetPosition();

        Direction2D.cardinals.ForEach(cardinal =>
        {
            Vector3 samplePoint = roomPosition + new Vector3(cardinal.x, 0, cardinal.y) * (roomSize + cellsSpacing);
            if (!visitedPoints.Contains(samplePoint)) neightbours.Add(samplePoint);
        });
        return neightbours.Intersect(dataBuffer).ToList();
    }

    private List<DungeonRoom> GetNeighbouringRooms(DungeonRoom room, DungeonData data)
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

    private List<Vector3> SelectRandomNeighbours(List<Vector3> points, HashSet<Vector3> visitedPoints)
    {
        int randomNumPoints = UnityEngine.Random.Range(1, points.Count);
        var selectedPoints = points.OrderBy(x => Guid.NewGuid()).Take(randomNumPoints).ToList();
        selectedPoints.ForEach(point => visitedPoints.Add(point));
        return selectedPoints;
    }

    public void ConnectMissingRooms(DungeonData data)
    {
        data.AllRooms().ForEach(room =>
        {
            GetNeighbouringRooms(room, data).ForEach(neighbour =>
            {
                if (!room.GetConnectedRooms().Contains(neighbour))
                    room.ConnectRoom(neighbour);
            });
        });
    }

    public void DeleteRandomRooms(DungeonData data)
    {
        int range = data.AllRooms().Count();
        int randomNumRooms = UnityEngine.Random.Range(Mathf.FloorToInt(range * (1-roomDensity)), range);
        data.AllRooms().OrderBy(room => room.Id())
            .Take(randomNumRooms)
            .ToList()
            .ForEach(room => 
            {
                var adjacencyList = room.GetConnectedRooms();
                bool canDelete = true;
                adjacencyList.ForEach(adjacentRoom =>
                {
                    if(!DungeonRoom.PathExists(room, adjacentRoom, data.GetStartingRoom()))
                    {
                        canDelete = false;
                    }
                });
                if (canDelete && room != data.GetStartingRoom()) 
                {
                    data.AllRooms().Remove(room);
                    room.Delete();
                } 
            });        
    }

    public void ClearGrid()
    {
        ClearBuffer();
        DungeonDrawer.EraseDungeon(mono);
    }

    public void LoadRooms(List<Vector3> rooms, DungeonData data)
    {
        dataBuffer = rooms;
        GenerateGrid();
        var roomsObj = DungeonDrawer.DrawRooms(rooms, PrimitiveType.Cube, mono, new Vector3(roomSize, 1, roomSize));
        roomsObj.ForEach(roomObj => 
        {
            roomObj.AddComponent<DungeonRoom>();
            data.AddRoom(roomObj.GetComponent<DungeonRoom>());
        });
        ConnectMissingRooms(data);
    }
}
