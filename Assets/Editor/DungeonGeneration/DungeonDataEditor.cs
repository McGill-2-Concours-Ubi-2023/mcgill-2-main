using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(DungeonData))]
public class DungeonDataEditor : Editor
{
    private DungeonData dungeonData;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        dungeonData = ((DungeonData)target);
        if (dungeonData.GetMonoInstance() == null)
        {
            dungeonData.SetMonoInstance(FindObjectOfType<DungeonGenerator>().gameObject);   
        }

        if (GUILayout.Button("Generate new dungeon"))
        {
            dungeonData.GenerateDungeon();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        if (GUILayout.Button("Clear dungeon"))
        {
            dungeonData.ClearDungeon();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        if (dungeonData.GetActiveLayout() != null)
        {
            if (GUILayout.Button("Save data"))
            {
                bool result = EditorUtility.DisplayDialog("Confirmation Dialog",
                    "The selected dungeon's layout will be overwritten, do you still wish to continue?",
                    "OK", "Cancel");
                if (result)
                {
                    dungeonData.SaveData();
                    EditorUtility.DisplayDialog("Success Dialog", "Layout successfully saved!", "OK");
                    EditorUtility.SetDirty(dungeonData.GetActiveLayout());
                }
            }
        } 

        if(GUILayout.Button("Load layout data"))
        {
            bool result = EditorUtility.DisplayDialog("Confirmation Dialog",
                   "Any new changes will be overwritten, do you still wish to continue?",
                   "Load data", "Cancel");
            if (result)
            {
                dungeonData.LoadData();
                EditorUtility.DisplayDialog("Success Dialog", "Layout successfully loaded!", "OK");
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }
}
