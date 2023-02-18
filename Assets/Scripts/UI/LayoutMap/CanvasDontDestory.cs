using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasDontDestory : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
