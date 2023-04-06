using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassInteractor : MonoBehaviour
{
    [Range(1, 5)]
    public int id;
    [Range(0, 100)]
    public float influenceRadius;
}
