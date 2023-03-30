using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class DungeonDoor : MonoBehaviour
{
    [SerializeField]
    private DungeonRoom sharedRoom1;
    [SerializeField]
    private DungeonRoom sharedRoom2;
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Coroutine openCoroutine;
    private Coroutine closeCoroutine;
    private MeshCollider doorCollider;
    [SerializeField]
    private bool canOpen = true;
    private DungeonRoom bufferRoom;

    private void Start()
    {
        doorCollider = transform.Find("Door").GetComponent<MeshCollider>();
    }

    public bool CanOpen()
    {
        return canOpen;
    }

    public void Block()
    {
        canOpen = false;
    }

    public void Unlock()
    {
        canOpen = true;
    }

    public static DungeonDoor Create(GameObject parentObj, DungeonRoom originRoom, DungeonRoom targetRoom, DungeonData data)
    {
        Vector3 Y_Offset = new Vector3(0, data.GetNormalRoomPrefabs()[0].transform.localScale.y / 2, 0);
        Vector3 diff = originRoom.transform.position - targetRoom.transform.position;
        Vector3 doorPosition = parentObj.transform.position - diff/2 + Y_Offset;
        var doorObject = DungeonDrawer.DrawSingleObject(doorPosition, data.GetDoorPrefab(), parentObj);
        var doorComponent = doorObject.AddComponent<DungeonDoor>(); //canOpen = true by default;
        doorComponent.sharedRoom1 = originRoom;
        doorComponent.sharedRoom2 = targetRoom;
        var orientation = originRoom.transform.position - targetRoom.transform.position;
        var direction = new Vector3(orientation.x, 0, orientation.z);
        var angle = Vector3.SignedAngle(parentObj.transform.forward, direction, Vector3.up);
        doorObject.transform.Rotate(0, angle, 0);
        if (originRoom.IsIsolated() || targetRoom.IsIsolated()) doorComponent.canOpen = false; //lock door is isolated
        return doorComponent;
    }

    public List<DungeonRoom> GetSharedRooms()
    {
        return new List<DungeonRoom>() { sharedRoom1, sharedRoom2 };
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") && canOpen)
        {
            if (sharedRoom1 == DungeonRoom.activeRoom) 
            {
                bufferRoom = sharedRoom2;
            }
            else
            {
                bufferRoom = sharedRoom1;
            } 
            openCoroutine = StartCoroutine(Open());
            if (closeCoroutine != null) StopCoroutine(closeCoroutine);
            DungeonRoom.lastEnteredDoor = this;
        }
    }

    IEnumerator Close()
    {
        if (skinnedMeshRenderer == null) skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        var openRate = FindObjectOfType<MainCharacterController>().doorOpenRate;
        while (skinnedMeshRenderer.GetBlendShapeWeight(0) > 0)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(0, skinnedMeshRenderer.GetBlendShapeWeight(0) - openRate);
            UpdateCollider();
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Open()
    {
        if (skinnedMeshRenderer == null) skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        var openRate = FindObjectOfType<MainCharacterController>().doorOpenRate;
        while (skinnedMeshRenderer.GetBlendShapeWeight(0) < 100)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(0, skinnedMeshRenderer.GetBlendShapeWeight(0) + openRate);
            UpdateCollider();
            yield return new WaitForEndOfFrame();
        }               
    }

    private void UpdateCollider()
    {
        Mesh bakeMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(bakeMesh);
        doorCollider.sharedMesh = bakeMesh;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            closeCoroutine = StartCoroutine(Close());
            if (openCoroutine != null) StopCoroutine(openCoroutine);

            if (other.CompareTag("Player"))
            {
                if(DungeonRoom.activeRoom == bufferRoom)
                GoThorouthDoor();
            }
        }
    }

    public void SetRooms(DungeonRoom room1, DungeonRoom room2)
    {
        sharedRoom1 = room1;
        sharedRoom2 = room2;
    }

    public void GoThorouthDoor() {
        MapManager map = GameObject.Find("LayoutMap").GetComponent<MapManager>();
        DungeonRoom currentRoom = map.dungeonData.GetActiveRoom();
        Vector2Int position = currentRoom.GridPosition();
        int gridSize = map.dungeonGrid.GridSize(); 
        map.VisitRoom(position.x * gridSize + position.y);

        if (sharedRoom1 == currentRoom)
        {
            Vector2Int pos = sharedRoom2.GridPosition();
            sharedRoom1.SpawnEnemies();
            sharedRoom2.StopSpawnEnemies();
            map.LeaveRoom(pos.x * gridSize + pos.y);
        }
        else {
            Vector2Int pos = sharedRoom1.GridPosition();
            sharedRoom2.SpawnEnemies();
            sharedRoom1.StopSpawnEnemies();
            map.LeaveRoom(pos.x * gridSize + pos.y);
        }
    }

    public void ShowWalls()
    {
        if (DungeonRoom.GetActiveRoom() == sharedRoom1)
        {
            foreach (OccludableWall wall in sharedRoom2.GetOccludableWalls())
            {
                if (wall != null) wall.Occlude();
            }
        }
        else if (DungeonRoom.GetActiveRoom() == sharedRoom2)
        {
            foreach (OccludableWall wall in sharedRoom1.GetOccludableWalls())
            {
                if (wall != null) wall.Occlude();
            }
        }
    }
}
