using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    GameObject[] enemyGroups;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Instantiate(enemyGroups[Random.Range(0, enemyGroups.Length)], transform.position +new Vector3(0,0.5f),Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
