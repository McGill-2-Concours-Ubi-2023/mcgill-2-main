using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "New Configuration", menuName = "Configuration")]
public class Configuration : ScriptableObject
{
    [SerializeField]
    public SerializableDict<string, SerializableList<GameObject>> configurations = new SerializableDict<string, SerializableList<GameObject>>();
    [SerializeField]
    List<List<GameObject>> gbs = new List<List<GameObject>>();
    [SerializeField]
    List<string> descriptions = new List<string>();
    [SerializeField]
    List<GameObject> gos = new List<GameObject>();

}
