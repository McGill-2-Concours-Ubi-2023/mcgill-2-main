using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New grid", menuName = "Dungeon grid")]
public class DungeonGridData : DataContainer
{
    [SerializeField][Range(0, 20)]
    private int cellsSpacing;
    [SerializeField]
    [Range(1, 500)]
    private int gridSize;

    public void GenerateGrid()
    {
        Debug.Log(mono.name);
        var currentPosition = mono.transform.position;
        var offsetx = cellsSpacing * Vector3.right;
        var offsetz = cellsSpacing * Vector3.forward;

        for (int i = 0; i < gridSize * gridSize; i++) //O(n)
        {
            var x = i % gridSize;
            var z = i / gridSize;
            Debug.Log(z);
            var position = currentPosition + x * offsetx + z * offsetz ;

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = position;
            cube.transform.parent = mono.transform;
        }
    }
}
