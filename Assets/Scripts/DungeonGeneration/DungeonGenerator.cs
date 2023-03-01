using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField]
    public DungeonData data;

    private void Awake()
    {
        data.SetMonoInstance(this.gameObject);
        data.TryQuickLoad();
    }
}
