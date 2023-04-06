#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Configuration))]
public class ConfigurationEditor : Editor
{
    public Configuration config;
    public string descriptionToAdd = "";
    public GameObject RoomToAdd = null;
    public string descriptionToRemove = "";
    public GameObject RoomToRemove = null;
    private GUIStyle title = new GUIStyle();
    private GUIStyle subTitle = new GUIStyle();

    public override void OnInspectorGUI()
    {
        title.fontSize = 15;
        subTitle.fontSize = 14;
        subTitle.alignment = TextAnchor.UpperCenter;
        //base.OnInspectorGUI();
        config = (Configuration)target;
        serializedObject.Update();
        SerializedProperty sp2 = serializedObject.FindProperty("configurations");
        EditorGUILayout.PropertyField(sp2);
        GUILayout.Label("Hashtable:", title);
        foreach (var kvp in config.configurations)
        {
            GUILayout.Label(kvp.Key + ":");
            foreach (var gameObject in kvp.Value)
            {
                GUILayout.Label("- " + gameObject.name);
            }
        }
        GuiLine();
        GUILayout.Label("Add new entry:", subTitle);
        GUILayout.Label("description:");

        descriptionToAdd = GUILayout.TextField(descriptionToAdd);
        RoomToAdd = (GameObject)EditorGUILayout.ObjectField("room:", RoomToAdd, typeof(GameObject), true);
        if (GUILayout.Button("Add entry") && descriptionToAdd != "" && RoomToAdd != null)
        {
            if (!config.configurations.ContainsKey(descriptionToAdd))
            {
                config.configurations.Add(descriptionToAdd, new SerializableList<GameObject>());
            }

            config.configurations[descriptionToAdd].Add(RoomToAdd);
            descriptionToAdd = ""; // clear the input field
            RoomToAdd = null; // clear the game object field
            EditorUtility.SetDirty(target); // mark the scriptable object as dirty to save the changes
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        GuiLine();
        GUILayout.Label("Remove config:", subTitle);
        GUILayout.Label("description:");
        descriptionToRemove = GUILayout.TextField(descriptionToRemove);
        RoomToRemove = (GameObject)EditorGUILayout.ObjectField("room:", RoomToRemove, typeof(GameObject), true);
        if (GUILayout.Button("Remove config") && descriptionToRemove != "" && RoomToRemove != null)
        {
            if (config.configurations.ContainsKey(descriptionToRemove))
            {
                config.configurations[descriptionToRemove].Remove(RoomToRemove);
                if (config.configurations[descriptionToRemove].Count == 0) config.configurations.Remove(descriptionToRemove);
                descriptionToRemove = ""; // clear the input field
                RoomToRemove = null; // clear the game object field
                EditorUtility.SetDirty(target); // mark the scriptable object as dirty to save the changes
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        GuiLine();
        GUILayout.Label("Remove description", subTitle);
        GUILayout.Label("description:");
        descriptionToRemove = GUILayout.TextField(descriptionToRemove);
        if (GUILayout.Button("Remove description") && descriptionToRemove != "") {
            if (config.configurations.ContainsKey(descriptionToRemove)) {
                config.configurations.Remove(descriptionToRemove);
                descriptionToRemove = ""; // clear the input field
                EditorUtility.SetDirty(target); // mark the scriptable object as dirty to save the changes
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
    void GuiLine(int i_height = 5)

    {

        Rect rect = EditorGUILayout.GetControlRect(false, i_height);

        rect.height = i_height;

        EditorGUI.DrawRect(rect, new Color(0f, 0f, 0f, 1));

    }


}
#endif