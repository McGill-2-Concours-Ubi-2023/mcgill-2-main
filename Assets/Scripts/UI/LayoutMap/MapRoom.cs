using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class MapRoom : MonoBehaviour
{
    [SerializeField]
    private int ID; 
    private bool hasVisited = false;
    private int x, y; //position of the room in the whole map
    private RoomTypes.RoomType type;
    private Sprite icon;
    private bool isRoom;// if it's a room 
    public Sprite visited;
    public Sprite visiting;
    public Sprite notVisited; 
    [SerializeField]
    private Image backGround;
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private DungeonRoom attachedRoom;
    
    public bool IsRoom() {
        return isRoom;
    }

    public void BindRoom(DungeonRoom room)
    {
        this.attachedRoom = room;
    }

    public RoomTypes.RoomType GetRoomType()
    {
        return attachedRoom.GetRoomType();
    }

    public void SetRoom(DungeonRoom room) {// set the cell to be a room
        isRoom = true;
        Color color = backGround.color;
        color.a = 1f;
        backGround.color = color;
        this.attachedRoom = room;
    }

    public void VisitRoom() {
        if (!hasVisited) {
            hasVisited = true;
        }
        backGround.sprite = visiting;
    }

    public void LeaveRoom() {
        backGround.sprite = visited; 
    }

    public void SetID(int id) {
        ID = id; 
    }

    public void SetType(RoomTypes.RoomType type) {
        this.type = type;
        if (type == RoomTypes.RoomType.Start) {
            backGround.sprite = visiting;
            hasVisited = true;
        }
        else if (type != RoomTypes.RoomType.Normal) {
            //TODO: set room's icon
        }
    }
}
