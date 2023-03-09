using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomCollider : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        DungeonRoom.activeRoom = GetComponentInParent<DungeonRoom>();
    }

    private void OnCollisionStay(Collision collision)
    {
        DungeonRoom.activeRoom = GetComponentInParent<DungeonRoom>();
    }
}
