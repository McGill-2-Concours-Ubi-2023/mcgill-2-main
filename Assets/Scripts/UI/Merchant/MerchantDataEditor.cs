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
    private string descriptionToAdd = "";
    private int priceToAdd = -1;
    private string methodToAdd = "";
    private int entryIndexToRemove = -1;


    public override void OnInspectorGUI() {
        title.fontSize = 15;
        subTitle.fontSize = 14;
        subTitle.alignment = TextAnchor.UpperCenter;
        data = (MerchantData)target;
        SerializableDict<string, int> prices = data.merchantPrices;
        SerializableDict<string, string> methods = data.merchantMethods;
        SerializableDict<int, string> descriptions = data.descriptions;
        serializedObject.Update();
        GUILayout.Label("Hashtable:", title);
        int index = 0;
        foreach (var kvp in descriptions)
        {
            GUILayout.Label(descriptions[kvp.Key] + ":");
            GUILayout.Label("- " + "price: " + prices[kvp.Value] + " pts");
            GUILayout.Label("- " + "method: " + methods[descriptions[kvp.Key]]);
            GUILayout.Label("- " + "index: " + kvp.Key);
            index++; 
        }
        GuiLine();

        GUILayout.Label("Add new entry:", subTitle);
        GUILayout.Label("description:"); 
        descriptionToAdd = GUILayout.TextField(descriptionToAdd);
        GUILayout.Label("price:");
        priceToAdd = EditorGUILayout.IntField(priceToAdd);
        GUILayout.Label("method name:");
        methodToAdd = GUILayout.TextField(methodToAdd);
        if (GUILayout.Button("Add entry") && descriptionToAdd != "" && methodToAdd != "") {
            if (!prices.ContainsKey(descriptionToAdd))
            {
                for (int i = 0; i < 100; i++) {
                    if (!descriptions.ContainsKey(i)) {
                        descriptions.Add(i, descriptionToAdd);
                        prices.Add(descriptionToAdd, priceToAdd);
                        methods.Add(descriptionToAdd, methodToAdd);
                        break;
                    }
                }
                
            }
            else Debug.Log("already has this description, delete first");

            descriptionToAdd = "";
            priceToAdd = -1;
            methodToAdd = "";
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
                descriptions.Remove(entryIndexToRemove);
                
            }
            else Debug.Log("given description does not exist");

            entryIndexToRemove = -1;
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