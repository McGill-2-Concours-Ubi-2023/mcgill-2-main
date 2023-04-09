using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemySpawn1 : MonoBehaviour
{
    public GameObject[] enemies;
    public Vector3[] spawnpoints;
    public VisualEffect volumeFog;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Vector3 sp in spawnpoints)
        {
            if (Random.Range(0, 3) == 0)
            {
                int rand = Random.Range(1, 2);
                for (int i = 0; i <= rand; i++)
                {
                    GameObject.Instantiate(enemies[Random.Range(0, enemies.Length)], transform.position + sp + new Vector3(0, 0.5f), Quaternion.identity);
                }
            }
        }
    }

    //Below is used to manage the fog arround the spawn

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        volumeFog.SetVector3("PlayerPosition", player.transform.position);
    }

    public void DissipateAmbientFog()
    {
        StartCoroutine(DissipateFog(0.5f));
    }

    IEnumerator DissipateFog(float timer)
    {
        volumeFog.SendEvent("OnDissipate");
        //below is for faster dissipation
        float elapsedTime = 0;
        float currentFogAlpha = volumeFog.GetFloat("Alpha");
        while (elapsedTime < timer)
        {
            float t = elapsedTime / timer;
            float threshold = Mathf.Lerp(currentFogAlpha, 0, t);
            volumeFog.SetFloat("Alpha", threshold);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //Destroy(volumeFog.gameObject);
    }
}
