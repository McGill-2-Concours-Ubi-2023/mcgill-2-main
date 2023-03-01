using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonDoorPlaceholder : MonoBehaviour
{
    private bool isEmpty = true;   
    
    public void SetEmpty(bool empty)
    {
        isEmpty = empty; //set to false when populated
    }

    public Vector3 GetWorldPosition()
    {
        Debug.Log(transform.TransformPoint(transform.position));
        return transform.TransformPoint(transform.position);
    }
}
