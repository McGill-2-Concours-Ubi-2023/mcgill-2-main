using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DungeonRoomCollider : MonoBehaviour
{

    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }
    // Start is called before the first frame update
    private async void OnTriggerEnter(Collider other)
    {

        while (GameManager.isLoading)
        {
            await Task.Yield();
        }

        if (this == null)
        {
            return;
        }
        
        DungeonRoom.activeRoom = transform.parent.GetComponentInParent<DungeonRoom>();
        if (DungeonRoom.activeRoom)
            await DungeonRoom.activeRoom.UpdateRoomsLayout();
        if (DungeonRoom.lastEnteredDoor)
            DungeonRoom.lastEnteredDoor.ShowWalls();
    }

    private void OnTriggerStay(Collider other)
    {
        DungeonRoom.activeRoom = GetComponentInParent<DungeonRoom>();
    }
}
