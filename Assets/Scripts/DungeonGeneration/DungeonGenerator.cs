using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DungeonGenerator : MonoBehaviour
{
    [SerializeField]
    DungeonData data;

    private void Awake()
    {
        data.SetMonoInstance(this.gameObject);
    }

    private void Update()
    {
        if(data.GetMonoInstance() == null)
        {
            data.SetMonoInstance(this.gameObject);
        }
    }
}
