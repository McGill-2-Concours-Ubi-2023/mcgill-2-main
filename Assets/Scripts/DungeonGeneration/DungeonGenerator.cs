using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.AI.Navigation;

[ExecuteInEditMode]
public class DungeonGenerator : MonoBehaviour
{
    [SerializeField]
    public List<DungeonRoom> activeRooms;
    [SerializeField]
    public DungeonData data;
    [SerializeField]
    private bool newDungeonOnPlay = false;
    public DungeonRoom roomToReplace;

    private void Awake()
    {
        data.SetMonoInstance(this);
        if (newDungeonOnPlay)
        {
            data.GenerateDungeon();
        } else
        {
            data.LoadData();
        }
        StartCoroutine(PlaceRandomMerchant());
    }

    public void AddActiveRoom(DungeonRoom room)
    {
        if (!activeRooms.Contains(room))
        {
            activeRooms.Add(room);
        }
    }

    IEnumerator PlaceRandomMerchant()
    {
        yield return new WaitForEndOfFrame();
        data.PlaceMerchant();
    }
}
