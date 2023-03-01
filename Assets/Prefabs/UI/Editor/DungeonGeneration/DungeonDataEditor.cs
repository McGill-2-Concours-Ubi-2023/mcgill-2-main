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
            if (startingRoom == null && !dungeonData.GetActiveLayout().IsEmpty())
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
                EditorUtility.DisplayDialog("Null reference", "Data is empty", "OK");
            }
        }

        if (GUILayout.Button("Generate new dungeon"))
        {
            bool dataEmpty = dungeonData.AllRooms().Count() == 0;
            if (dataEmpty)
            {
                dungeonData.GenerateDungeon();
                EditorUtility.SetDirty(dungeonData.GetActiveLayout());
            } 
            else
            {
                bool result = EditorUtility.DisplayDialog("Confirmation Dialog",
                                            "The selected dungeon's layout will be overwritten, do you still wish to continue?",
                                        "OK", "Cancel");
                if (result)
                {
                    dungeonData.GenerateDungeon();
                    EditorUtility.SetDirty(dungeonData.GetActiveLayout());
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
            }       
        }

        if (GUILayout.Button("Clear dungeon"))
        {
            bool result = EditorUtility.DisplayDialog("Confirmation Dialog",
                                            "The current layout will be erased, continue ?",
                                        "OK", "Cancel");
            if (result)
            {              
                dungeonData.ClearDungeon();
                dungeonData.GetActiveLayout().ClearData();
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
