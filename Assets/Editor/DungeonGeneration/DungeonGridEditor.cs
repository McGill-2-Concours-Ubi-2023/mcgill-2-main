using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGridData))]
public class DungeonGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DungeonGridData grid = ((DungeonGridData)target);

        if (GUILayout.Button("Generate Grid"))
        {
            grid.GenerateGrid();
        }
    }
}

