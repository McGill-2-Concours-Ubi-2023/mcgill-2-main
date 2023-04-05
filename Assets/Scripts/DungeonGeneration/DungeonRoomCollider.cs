using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DungeonRoomCollider : MonoBehaviour
{
   
    // Start is called before the first frame update
    private async void OnTriggerEnter(Collider other)
    {
        while (GameManager.isLoading)
        {
            await Task.Yield();
        }
        
        DungeonRoom.activeRoom = transform.parent.GetComponentInParent<DungeonRoom>();
        DungeonRoom.activeRoom.UpdateRoomsLayout();
        if (DungeonRoom.lastEnteredDoor != null) DungeonRoom.lastEnteredDoor.ShowWalls();
        
    }


    private void OnTriggerStay(Collider other)
    {
        DungeonRoom.activeRoom = GetComponentInParent<DungeonRoom>();
    }
}
