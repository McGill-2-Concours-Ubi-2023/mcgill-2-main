using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallComposite : MonoBehaviour
{
    public GameObject doorWall;
    public GameObject plainWall;  

    public bool Incomplete()
    {
        return (plainWall == null);
    }
}

