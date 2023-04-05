using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableList<T> : List<T>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<T> data = new List<T>();

    public void OnBeforeSerialize()
    {
        data.Clear();
        foreach (T item in this)
        {
            data.Add(item);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();
        for (int i = 0; i < data.Count; i++)
        {
            this.Add(data[i]);
        }
    }
}
