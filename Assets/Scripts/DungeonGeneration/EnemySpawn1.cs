using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn1 : MonoBehaviour
{
    public GameObject[] enemies;
    public Vector3[] spawnpoints;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Vector3 sp in spawnpoints)
        {
            if (Random.Range(0, 3) == 0)
            {
                int rand = Random.Range(1, 3);
                for (int i = 0; i <= rand; i++)
                {
                    GameObject.Instantiate(enemies[Random.Range(0, enemies.Length)], transform.position + sp + new Vector3(0, 0.5f), Quaternion.identity);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
