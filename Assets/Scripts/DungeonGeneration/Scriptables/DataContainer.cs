using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataContainer : ScriptableObject
{
    protected List<Vector3> dataBuffer = new List<Vector3>();
    protected GameObject mono;

    public void SetMonoInstance(GameObject mono)
    {
        this.mono = mono;
    }

    public GameObject GetMonoInstance()
    {
        return mono;
    }

    public void ClearBuffer()
    {
        //Garbage collector is your friend, let him work a little bit
        dataBuffer = new List<Vector3>();
    }
}
