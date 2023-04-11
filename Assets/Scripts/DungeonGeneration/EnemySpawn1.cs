using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemySpawn1 : MonoBehaviour
{
    public GameObject[] enemies;
    public Vector3[] spawnpoints;
    private GameObject player;
    public bool debugSpawn=false;
    public bool doubleSpawn = true;
    // Start is called before the first frame update

    public void SpawnEnemies(DungeonRoom room)
    {
        if (debugSpawn)
        {
            foreach (Vector3 sp in spawnpoints)
            {
                if (Random.Range(0, 3) == 0)
                {
                    int rand = Random.Range(1, 2);
                    for (int i = 0; i <= rand; i++)
                    {
                        var composite = GameObject.Instantiate(enemies[Random.Range(0, enemies.Length)], transform.position + sp + new Vector3(0, 0.5f), Quaternion.identity);
                        var ais = composite.GetComponentsInChildren<EnemyAI>();
                        foreach (var ai in ais)
                        {
                            room.AddEnemy(ai);
                        }
                        composite.transform.DetachChildren();

                    }
                }
            }
        }
        else
        {
            foreach (Vector3 sp in spawnpoints)
            {

                int rand = Random.Range(0, 5);
                var composite = GameObject.Instantiate(enemies[Random.Range(0, enemies.Length)], transform.position + sp + new Vector3(0, 0.5f), Quaternion.identity);
                var ais = composite.GetComponentsInChildren<EnemyAI>();
                foreach (var ai in ais)
                {
                    room.AddEnemy(ai);
                }
                composite.transform.DetachChildren();
                if (rand == 4 && doubleSpawn)
                {
                    GameObject altComposite = GameObject.Instantiate(enemies[Random.Range(0, enemies.Length)], transform.position + sp + new Vector3(0, 0.5f), Quaternion.identity);
                    var ais_alt = composite.GetComponentsInChildren<EnemyAI>();
                    foreach (var ai in ais_alt)
                    {
                        room.AddEnemy(ai);
                    }
                    altComposite.transform.DetachChildren();
                }
            }
        }
    }

}
