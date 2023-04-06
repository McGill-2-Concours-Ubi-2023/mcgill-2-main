using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTypes : MonoBehaviour
{
    public static float NormalSpawnProb = 70.0f;
    public static float TreasureSpawnProb = 20.0f;
    public static float SpecialSpawnProb = 10.0f;
    public static float[] SpawnProbabilities = { NormalSpawnProb, TreasureSpawnProb, SpecialSpawnProb };

    public enum RoomType {
       Normal, Boss, Treasure, Special, Start //add room types as needed
    }

    public static RoomType GetRandomRoomType()
    {
        float rand = UnityEngine.Random.Range(0f, 100f);
        float cumulativeProbability = 0f;
        int ignoreCount = 0;
        for (int i = 0; i < Enum.GetValues(typeof(RoomType)).Length; i++)
        {
            if ((RoomType)i == RoomType.Boss || (RoomType)i == RoomType.Start)
            {
                ignoreCount++;
                continue; // skip the Boss and Start room types
            }

            cumulativeProbability += SpawnProbabilities[i - ignoreCount];
            if (rand <= cumulativeProbability)
            {
                return (RoomType)(i);
            }
        }
        return RoomType.Normal;
    }
}
