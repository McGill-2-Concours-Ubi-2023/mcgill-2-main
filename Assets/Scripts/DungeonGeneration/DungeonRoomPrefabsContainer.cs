using UnityEngine;

public interface DungeonRoomPrefabsContainer
{
    public GameObject[] GetNormalRoomPrefabs();
    public GameObject[] GetStartRoomPrefabs();
    public GameObject[] GetSpecialRoomPrefabs();
    public GameObject[] GetTreasureRoomPrefabs();
}

