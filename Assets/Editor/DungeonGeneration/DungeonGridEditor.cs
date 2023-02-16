using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGrid))]
public class DungeonGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DungeonGrid grid = ((DungeonGrid)target);

        if (GUILayout.Button("Generate Grid"))
        {
            //grid.GenerateGrid();
        }
    }
}

