using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomCollider : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        DungeonRoom.activeRoom = GetComponentInParent<DungeonRoom>();
        DungeonRoom.activeRoom.GetOccludableWalls().ForEach(wall => wall.Hide());
        DungeonRoom.lastEnteredDoor.ShowWalls();
    }

    private void OnTriggerStay(Collider other)
    {
        DungeonRoom.activeRoom = GetComponentInParent<DungeonRoom>();
    }
}
