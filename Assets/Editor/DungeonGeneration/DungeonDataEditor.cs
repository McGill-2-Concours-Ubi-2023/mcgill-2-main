using System;
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
        OnNullInitializeBehaviour(FindObjectOfType<DungeonGenerator>().gameObject);

        var mapManager = dungeonData.GetMapManager();
        var startingRoom = dungeonData.GetStartingRoom();

        if (mapManager == null)
        {
            dungeonData.FindMapManager();
        }

        if (GUILayout.Button("Track start room"))
        {            
            if (startingRoom == null && dungeonData.GetActiveLayout() != null)
            {
                dungeonData.LoadData();
                startingRoom = dungeonData.GetStartingRoom();
                EditorGUIUtility.PingObject(startingRoom);
                Selection.activeGameObject = startingRoom.gameObject;
            } else if(startingRoom != null)
            {
                EditorGUIUtility.PingObject(startingRoom);
                Selection.activeGameObject = startingRoom.gameObject;
            } else
            {
                EditorUtility.DisplayDialog("Null reference", "Error: room could not be tracked", "OK");
            }
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
                if(dungeonData.AllRooms() == null || dungeonData.GetStartingRoom() == null)
                {
                    EditorUtility.DisplayDialog("Save data empty", "No data to save!", "OK");
                } else
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

    private void OnNullInitializeBehaviour(GameObject dungeonGenerator)
    {
        if (dungeonData.GetMonoInstance() == null)
        {
            dungeonData.SetMonoInstance(dungeonGenerator);
            dungeonData.GetGrid().SetMonoInstance(dungeonGenerator);
        }
        else if (dungeonData.GetGrid().GetMonoInstance() == null)
        {
            dungeonData.GetGrid().SetMonoInstance(dungeonGenerator);
        }
    }
}
