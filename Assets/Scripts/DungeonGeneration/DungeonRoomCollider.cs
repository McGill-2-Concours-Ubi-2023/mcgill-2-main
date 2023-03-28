using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomCollider : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        DungeonRoom.activeRoom = GetComponentInParent<DungeonRoom>();
        DungeonRoom.activeRoom.GetOccludableWalls().ForEach(wall => { if (wall != null) wall.Hide(); });
        if (DungeonRoom.lastEnteredDoor != null) DungeonRoom.lastEnteredDoor.ShowWalls();
    }


    private void OnTriggerStay(Collider other)
    {
        DungeonRoom.activeRoom = GetComponentInParent<DungeonRoom>();
    }
}
