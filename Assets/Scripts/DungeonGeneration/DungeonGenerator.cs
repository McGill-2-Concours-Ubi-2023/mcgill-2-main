using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DungeonGenerator : MonoBehaviour
{
    [SerializeField]
    DungeonGridData grid;
    [SerializeField]
    DungeonData data;

    private void Awake()
    {
        grid.SetMonoInstance(this.gameObject);
        data.SetMonoInstance(this.gameObject);
    }

    private void Update()
    {
        if(grid.GetMonoInstance() == null)
        {
            grid.SetMonoInstance(this.gameObject);
        }
    }
}
