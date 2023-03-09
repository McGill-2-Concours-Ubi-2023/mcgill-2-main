using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DungeonDoor : MonoBehaviour
{
    [SerializeField]
    private DungeonRoom sharedRoom1;
    [SerializeField]
    private DungeonRoom sharedRoom2;


    public static DungeonDoor Create(GameObject doorObj, DungeonRoom originRoom, DungeonRoom targetRoom, DungeonData data)
    {
        Vector3 Y_Offset = new Vector3(0, data.GetRoomPrefab().transform.localScale.y/2
              + data.GetDoorPrefab().transform.localScale.y / 2, 0);
        Vector3 diff = originRoom.transform.position - targetRoom.transform.position;
        Vector3 doorPosition = doorObj.transform.position - diff/2 + Y_Offset;
        var doorObject = DungeonDrawer.DrawSingleObject(doorPosition, data.GetDoorPrefab(), doorObj);
        var doorComponent = doorObject.AddComponent<DungeonDoor>();
        doorComponent.sharedRoom1 = originRoom;
        doorComponent.sharedRoom2 = targetRoom;
        GameObject placeholder;
        data.GetGrid().GetWallsLayout().TryGetValue(doorPosition, out placeholder);
        if (placeholder != null)
        {
            doorComponent.transform.rotation = placeholder.transform.rotation;
            GameObject.DestroyImmediate(placeholder);
        }     
        return doorComponent;
    }

    public static void CreatePlaceholder(DungeonRoom room, Vector3 direction, DungeonData data)
    {
        Vector3 Y_Offset = new Vector3(0, data.GetRoomPrefab().transform.localScale.y / 2
              + data.GetDoorPrefab().transform.localScale.y / 2, 0);
        Vector3 phPosition = room.transform.position + data.GetRoomPrefab().transform.localScale.x / 2 *
            direction + Y_Offset;
        if (!data.GetGrid().GetWallsLayout().ContainsKey(phPosition))
        {
            var doorPhObj = DungeonDrawer.DrawSingleObject(phPosition, data.GetWallPrefab(), room.transform.gameObject);
            var doorPhRotation = Quaternion.FromToRotation(doorPhObj.transform.forward, direction);
            var adjustedRotation = new Quaternion(0, doorPhRotation.y, doorPhRotation.z, doorPhRotation.w); // no -180 degrees on x axis
            doorPhObj.transform.rotation *= adjustedRotation;
            data.GetGrid().AddWallPosition(phPosition, doorPhObj);
        }      
    }

    public List<DungeonRoom> GetSharedRooms()
    {
        return new List<DungeonRoom>() { sharedRoom1, sharedRoom2 };
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Vector3 Y_Offset = new Vector3(0, 5, 0);
            transform.position += Y_Offset;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Vector3 Y_Offset = new Vector3(0, 5, 0);
            transform.position -= Y_Offset;
        }
        if (other.CompareTag("Player")) {
            GoThorouthDoor();
        }
        
    }

    private void GoThorouthDoor() {
        MapManager map = GameObject.Find("LayoutMap").GetComponent<MapManager>();
        DungeonRoom currentRoom = map.dungeonData.GetActiveRoom();
        Vector2Int position = currentRoom.GridPosition();
        int gridSize = map.dungeonGrid.GridSize(); 
        map.VisitRoom(position.x * gridSize + position.y);

        if (sharedRoom1 == currentRoom)
        {
            Vector2Int pos = sharedRoom2.GridPosition();
            map.LeaveRoom(pos.x * gridSize + pos.y);
        }
        else {
            Vector2Int pos = sharedRoom1.GridPosition();
            map.LeaveRoom(pos.x * gridSize + pos.y);
        }
    }
}
