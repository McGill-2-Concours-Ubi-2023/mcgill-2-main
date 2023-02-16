using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New grid", menuName = "Dungeon grid")]
public class DungeonGrid : DataContainer
{
    [SerializeField]
    [Range(0, 100)]
    private int cellsSpacing;
    [SerializeField]
    [Range(1, 500)]
    private int gridSize;
    [SerializeField]

    public void GenerateGrid(DungeonData data)
    {
        ClearGrid();
        var currentPosition = mono.transform.position;
        var offsetx = cellsSpacing * Vector3.right;
        var offsetz = cellsSpacing * Vector3.forward;

        for (int i = 0; i < gridSize * gridSize; i++) //O(n)
        {
            var x = i % gridSize;
            var z = i / gridSize;
            var position = currentPosition + x * offsetx + z * offsetz;
            dataBuffer.Add(position);
        }
        GenerateRooms(data);
        DungeonDrawer.Draw(dataBuffer, mono, PrimitiveType.Cube);
    }

    public void GenerateRooms(DungeonData data)
    {
        List<Vector3> lockedPoints = new List<Vector3>();
        var startPosition = dataBuffer.ElementAt(Random.Range(0, dataBuffer.Count));
        DungeonRoom.Create(data, startPosition);
        lockedPoints.Add(startPosition);
        
        var currentPosition = startPosition;
        int numRooms = Mathf.FloorToInt(dataBuffer.Count * data.RoomPercent());
        for (int i = numRooms; i > 0; i--)
        {
            var availablePoints = dataBuffer.Except(lockedPoints);
            DungeonRoom.Create(data, availablePoints.ElementAt(Random.Range(0, availablePoints.Count())));
            var randomCardinal = Direction2D.GetRandomCardinal();
            currentPosition += new Vector3(randomCardinal.x, 0, randomCardinal.y);
        }
    }

    public void ClearGrid()
    {
        ClearBuffer();
        DungeonDrawer.EraseDungeon(mono);
    }
}
