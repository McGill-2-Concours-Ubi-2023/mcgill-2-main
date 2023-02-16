using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New grid", menuName = "Dungeon grid")]
public class DungeonGrid : DataContainer
{
    [SerializeField][Range(0, 100)]
    private int cellsSpacing;
    [SerializeField]
    [Range(1, 500)]
    private int gridSize;

    public void GenerateGrid()
    {
        ClearGrid();
        var currentPosition = mono.transform.position;
        var offsetx = cellsSpacing * Vector3.right;
        var offsetz = cellsSpacing * Vector3.forward;

        for (int i = 0; i < gridSize * gridSize; i++) //O(n)
        {
            var x = i % gridSize;
            var z = i / gridSize;
            var position = currentPosition + x * offsetx + z * offsetz ;
            dataBuffer.Add(position);
        }
        DungeonDrawer.Draw(dataBuffer, mono, PrimitiveType.Cube);
    }

    public void ClearGrid()
    {
        ClearBuffer();
        DungeonDrawer.EraseDungeon(mono);
    }
}
