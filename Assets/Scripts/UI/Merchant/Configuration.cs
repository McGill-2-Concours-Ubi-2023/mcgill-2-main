using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "New Configuration", menuName = "Configuration")]
public class Configuration : ScriptableObject
{
    [SerializeField]
    public SerializableDict<string, SerializableList<GameObject>> configurations = new SerializableDict<string, SerializableList<GameObject>>();
    

}
