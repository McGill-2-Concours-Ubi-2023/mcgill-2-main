using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFieldInteractor : MonoBehaviour
{
    [Range(0.1f, 10f)]
    public float influenceRadius;
    [Range(0, 2)]
    public float pushForce;
}
