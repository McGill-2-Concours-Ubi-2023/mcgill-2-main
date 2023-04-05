using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataContainer<T> : ScriptableObject
{
    [SerializeField]
    protected List<Vector3> positionsBuffer = new List<Vector3>(); //use this buffer to store spacial data in your script
    [SerializeField]
    protected List<T> customMapBuffer = new List<T>();
    protected GameObject monoObject;

    public void SetMonoInstance(MonoBehaviour mono)
    {
        this.monoObject = mono.gameObject;
    }

    public GameObject GetMonoInstance()
    {
        return monoObject;
    }

    public List<T> GetCustomMapBuffer()
    {
        return customMapBuffer;
    }

    public void AddData(T item)
    {
        customMapBuffer.Add(item);
    }

    public void ClearData()
    {
        //Garbage collector is your friend, let him work a little bit
        customMapBuffer = new List<T>();
        positionsBuffer = new List<Vector3>();
    }
}
