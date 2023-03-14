using UnityEngine;

[System.Serializable]
public class RoomData
{
    [SerializeField]
    private Vector3 position;
    [SerializeField]
    private RoomTypes.RoomType type;
    [SerializeField]
    private int prefabIndex;

    public RoomData(Vector3 position, RoomTypes.RoomType type, int prefabIndex)
    {
        this.position = position;
        this.type = type;
        this.prefabIndex = prefabIndex;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public RoomTypes.RoomType GetRoomType()
    {
        return type;
    }

    public int GetPrefabIndex()
    {
        return prefabIndex;
    }
}

