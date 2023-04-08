#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MerchantData))]
public class MerchantDataEditor : Editor
{
    private MerchantData data;
    private GUIStyle title = new GUIStyle();
    private GUIStyle subTitle = new GUIStyle();
    private string description = "";
    private int price = -1;
    private string method = "";
    private int entryIndexToRemove = -1;
    private GameObject hologram; 


    public override void OnInspectorGUI() {
        title.fontSize = 15;
        subTitle.fontSize = 14;
        subTitle.alignment = TextAnchor.UpperCenter;
        data = (MerchantData)target;
        SerializableDict<string, int> prices = data.merchantPrices;
        SerializableDict<string, string> methods = data.merchantMethods;
        SerializableDict<int, string> descriptions = data.descriptions;
        SerializableDict<string, GameObject> holograms = data.holograms;
        serializedObject.Update();
        GUILayout.Label("Hashtable:", title);
        foreach (var kvp in descriptions)
        {
            GUILayout.Label(descriptions[kvp.Key] + ":");
            GUILayout.Label("- " + "price: " + prices[kvp.Value] + " pts");
            GUILayout.Label("- " + "method: " + methods[kvp.Value]);

            if (holograms.ContainsKey(kvp.Value))
            {
                GUILayout.Label("- " + "hologram: " + holograms[kvp.Value].ToString());
            }
            else GUILayout.Label("- " + "hologram: NONE");
            GUILayout.Label("- " + "index: " + kvp.Key);

        }
        GuiLine();

        GUILayout.Label("Add new entry:", subTitle);
        GUILayout.Label("description:"); 
        description = GUILayout.TextField(description);
        GUILayout.Label("price:");
        price = EditorGUILayout.IntField(price);
        GUILayout.Label("method name:");
        method = GUILayout.TextField(method);
        hologram = (GameObject)EditorGUILayout.ObjectField("hologram:", hologram, typeof(GameObject), true);
        if (GUILayout.Button("Add entry") && description != "" && method != "") {
            if (!prices.ContainsKey(description))
            {
                for (int i = 0; i < 100; i++) {
                    if (!descriptions.ContainsKey(i)) {
                        descriptions.Add(i, description);
                        prices.Add(description, price);
                        methods.Add(description, method);
                        if (hologram != null) {
                            holograms.Add(description, hologram);
                        }
                        
                        break;
                    }
                }
                
            }
            else Debug.Log("already has this description, delete first");

            hologram = null;
            description = "";
            price = -1;
            method = "";
            EditorUtility.SetDirty(target); // mark the scriptable object as dirty to save the changes
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        GuiLine();
        GUILayout.Label("Delete entry:", subTitle);
        GUILayout.Label("index:");
        entryIndexToRemove = EditorGUILayout.IntField(entryIndexToRemove);
        
        if (GUILayout.Button("Delete entry"))
        {
            if (prices.ContainsKey(descriptions[entryIndexToRemove]))
            {
                prices.Remove(descriptions[entryIndexToRemove]);
                methods.Remove(descriptions[entryIndexToRemove]);
                holograms.Remove(descriptions[entryIndexToRemove]);
                descriptions.Remove(entryIndexToRemove);

            }

            entryIndexToRemove = -1;
            EditorUtility.SetDirty(target); // mark the scriptable object as dirty to save the changes
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        serializedObject.ApplyModifiedProperties();

        GuiLine();
        GUILayout.Label("Edit entry:", subTitle);
        GUILayout.Label("index:");
        entryIndexToRemove = EditorGUILayout.IntField(entryIndexToRemove);
        GUILayout.Label("description:");
        description = GUILayout.TextField(description);
        GUILayout.Label("price:");
        price = EditorGUILayout.IntField(price);
        GUILayout.Label("method name:");
        method = GUILayout.TextField(method);
        hologram = (GameObject)EditorGUILayout.ObjectField("hologram:", hologram, typeof(GameObject), true);

        if (GUILayout.Button("Edit entry"))
        {
            if (prices.ContainsKey(descriptions[entryIndexToRemove]))
            {
                if (description != "")
                {
                    string oldDescription = descriptions[entryIndexToRemove];
                    descriptions[entryIndexToRemove] = description;
                    int price = prices[oldDescription];
                    prices.Remove(oldDescription);
                    prices.Add(description, price);
                    string methodName = methods[oldDescription];
                    methods.Remove(oldDescription);
                    methods.Add(description, methodName);
                    GameObject hologram;
                    if (holograms.ContainsKey(oldDescription))
                    {
                        hologram = holograms[oldDescription];
                        holograms.Remove(oldDescription);
                        holograms.Add(description, hologram);
                    }
                }
                if (price != -1)
                {
                    prices[descriptions[entryIndexToRemove]] = price;
                }
                if (method != "")
                {
                    methods[descriptions[entryIndexToRemove]] = method;
                }
                if (hologram != null)
                {
                    holograms[descriptions[entryIndexToRemove]] = hologram;
                }

            }

            entryIndexToRemove = -1;
            hologram = null;
            description = "";
            price = -1;
            method = "";
            EditorUtility.SetDirty(target); // mark the scriptable object as dirty to save the changes
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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