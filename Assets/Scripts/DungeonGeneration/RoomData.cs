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
    [SerializeField]
    private bool isOverride = false;
    [SerializeField]
    private bool isIsolated = false;

    public RoomData(Vector3 position, RoomTypes.RoomType type, int prefabIndex)
    {
        this.position = position;
        this.type = type;
        this.prefabIndex = prefabIndex;
    }

    public void SetIsolated(bool isolate)
    {
        this.isIsolated = isolate;
    }

    public bool IsIsolated()
    {
        return isIsolated;
    }

    public void SetOverride(bool _override, int prefabIndex){
        isOverride = _override;
        this.prefabIndex = prefabIndex;
    }

    public bool IsOverride()
    {
        return isOverride;
    }

    public void SetRoomType(RoomTypes.RoomType type)
    {
        this.type = type;
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

