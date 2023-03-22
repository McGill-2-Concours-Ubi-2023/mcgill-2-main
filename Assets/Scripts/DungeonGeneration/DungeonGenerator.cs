using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.AI.Navigation;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField]
    public DungeonData data;
    [SerializeField]
    private bool newDungeonOnPlay = false;
    public DungeonRoom roomToReplace;

    private void Awake()
    {
        data.SetMonoInstance(this.gameObject);
        if (newDungeonOnPlay)
        {
            data.GenerateDungeon();
        } else
        {
            data.LoadData();
        }
    }
}
