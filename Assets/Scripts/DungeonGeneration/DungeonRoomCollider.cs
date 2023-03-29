using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomCollider : MonoBehaviour
{
   
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        DungeonRoom.activeRoom = GetComponentInParent<DungeonRoom>();
        DungeonRoom.activeRoom.UpdateWalls();
        if (DungeonRoom.lastEnteredDoor != null) DungeonRoom.lastEnteredDoor.ShowWalls();
        DungeonRoom.activeRoom.OccludeRooms();
    }


    private void OnTriggerStay(Collider other)
    {
        DungeonRoom.activeRoom = GetComponentInParent<DungeonRoom>();
    }
}
