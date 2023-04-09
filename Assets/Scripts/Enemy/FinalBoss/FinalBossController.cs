using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public interface IBossFightTriggers : ITrigger
{
    void StartBossFight() { }
}

public interface IBossTriggers : ITrigger
{
    void StartAttack() { }
}

public class FinalBossController : MonoBehaviour, IBossTriggers
{
    [Header("Playground Properties")]
    public Transform topRightCorner;
    public Transform topLeftCorner;
    public bool Attack = false;
    [Header("LazerProperties")]
    public GameObject lazerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.isLoading = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Attack)
        {
            Attack = !Attack;
            //LazerSweepAttack(topRightCorner.position, topLeftCorner.position);
            StartCoroutine(SpawnRandomLazers_3());
        }
    }
    
    public void StartAttack()
    {
        Attack = true;
    }

    public void LazerSweepAttack(Vector3 position1, Vector3 position2)
    {
        var obj_1 =Instantiate(lazerPrefab);
        obj_1.transform.position = position1;
        obj_1.transform.Rotate(0f, 180f, 0f);
        GameObject obj_2 = Instantiate(lazerPrefab);
        obj_2.transform.position = position2;
        obj_2.transform.Rotate(0f, 180f, 0f);
        VisualEffect lazer1 = obj_1.GetComponent<VisualEffect>();
        VisualEffect lazer2 = obj_2.GetComponent<VisualEffect>();
        StartCoroutine(SweepLazers(lazer1, lazer2));
    }

    IEnumerator SpawnTwoLazers(Vector3 endpoint1, Vector3 endpoint2, float lazerbeamDuration, float lazerChargeTime)
    {
        var obj_1 = Instantiate(lazerPrefab);
        obj_1.transform.position = endpoint1;
        obj_1.transform.Rotate(0f, 180f, 0f);
        GameObject obj_2 = Instantiate(lazerPrefab);
        obj_2.transform.position = endpoint2;
        obj_2.transform.Rotate(0f, 180f, 0f);
        VisualEffect lazer1 = obj_1.GetComponent<VisualEffect>();
        VisualEffect lazer2 = obj_2.GetComponent<VisualEffect>();
        lazer1.SendEvent("OnLazerCharge");
        lazer2.SendEvent("OnLazerCharge");
        yield return new WaitForSeconds(lazerChargeTime);
        lazer1.GetComponentInChildren<LazerBeamCollider>().ActivateCollider();
        lazer2.GetComponentInChildren<LazerBeamCollider>().ActivateCollider();
        lazer1.SendEvent("OnLazerStart");
        lazer2.SendEvent("OnLazerStart");
        yield return new WaitForSeconds(lazerbeamDuration);
        lazer1.SendEvent("OnLazerStop");
        lazer2.SendEvent("OnLazerStop");
        yield return new WaitForSeconds(2.0f);
        Destroy(lazer1.gameObject);
        Destroy(lazer2.gameObject);
    }

    private GameObject GetOneLazer(Vector3 position, float lazerChargeTime, float lazerbeamDuration)
    {
        var obj_1 = Instantiate(lazerPrefab);
        obj_1.transform.position = position;
        obj_1.transform.Rotate(0f, 180f, 0f);
        StartCoroutine(AwakeLazer(obj_1, lazerChargeTime, lazerbeamDuration));
        return obj_1;
    }

    IEnumerator AwakeLazer(GameObject lazer, float lazerChargeTime, float lazerbeamDuration)
    {
        VisualEffect lazer1 = lazer.GetComponent<VisualEffect>();
        lazer1.SendEvent("OnLazerCharge");
        yield return new WaitForSeconds(lazerChargeTime);
        lazer1.GetComponentInChildren<LazerBeamCollider>().ActivateCollider();
        lazer1.SendEvent("OnLazerStart");
        yield return new WaitForSeconds(lazerbeamDuration);
        lazer1.SendEvent("OnLazerStop");
        yield return new WaitForSeconds(2.0f);
        Destroy(lazer1.gameObject);
    }

    IEnumerator SpawnOneLazer(Vector3 position, float lazerChargeTime, float lazerbeamDuration)
    {
        var obj_1 = Instantiate(lazerPrefab);
        obj_1.transform.position = position;
        obj_1.transform.Rotate(0f, 180f, 0f);
        VisualEffect lazer1 = obj_1.GetComponent<VisualEffect>();
        lazer1.SendEvent("OnLazerCharge");
        yield return new WaitForSeconds(lazerChargeTime);
        lazer1.GetComponentInChildren<LazerBeamCollider>().ActivateCollider();
        lazer1.SendEvent("OnLazerStart");
        yield return new WaitForSeconds(lazerbeamDuration);
        lazer1.SendEvent("OnLazerStop");
        yield return new WaitForSeconds(2.0f);
        Destroy(lazer1.gameObject);
    }

    IEnumerator SweepLazers(VisualEffect lazer1, VisualEffect lazer2)
    {
        const float lazerChargeTime = 1.0f;
        const float lazerRotationSpeed = 8.0f;
        const float lazerbeamDuration = 9.0f;
        lazer1.SendEvent("OnLazerCharge");
        lazer2.SendEvent("OnLazerCharge");
        yield return new WaitForSeconds(lazerChargeTime);
        lazer1.GetComponentInChildren<LazerBeamCollider>().ActivateCollider();
        lazer2.GetComponentInChildren<LazerBeamCollider>().ActivateCollider();
        lazer1.SendEvent("OnLazerStart");
        lazer2.SendEvent("OnLazerStart");
        float elapsedTime = 0;
        while (elapsedTime < lazerbeamDuration)
        {
            // Rotate towards right over time
            lazer1.transform.Rotate(0f, Time.deltaTime * lazerRotationSpeed, 0f);
            lazer2.transform.Rotate(0f, -Time.deltaTime * lazerRotationSpeed, 0f);
            elapsedTime += Time.deltaTime;
            Debug.Log(elapsedTime);
            yield return new WaitForEndOfFrame();
        }
        lazer1.SendEvent("OnLazerStop");
        lazer2.SendEvent("OnLazerStop");
        yield return new WaitForSeconds(2.0f);
        Destroy(lazer1.gameObject);
        Destroy(lazer2.gameObject);
    }

    IEnumerator LazerBurstPeriodic_2()
    {
        Vector3 endpoint_2 = topRightCorner.position;
        Vector3 endpoint_1 = topLeftCorner.position;
        float distance = (endpoint_1 - endpoint_2).magnitude;
        Vector3 endpoint_3 = endpoint_1 + new Vector3(distance / 7, 0, 0);
        Vector3 endpoint_4 = endpoint_1 + new Vector3(2 * distance / 7, 0, 0);
        Vector3 endpoint_5 = endpoint_1 + new Vector3(3 * distance / 7, 0, 0);
        Vector3 endpoint_6 = endpoint_1 + new Vector3(4 * distance / 7, 0, 0);
        Vector3 endpoint_7 = endpoint_1 + new Vector3(5 * distance / 7, 0, 0);
        Vector3 endpoint_8 = endpoint_1 + new Vector3(6 * distance / 7, 0, 0);
        StartCoroutine(SpawnTwoLazers(endpoint_1, endpoint_2, 0.8f, 0.5f));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(SpawnTwoLazers(endpoint_3, endpoint_8, 0.8f, 0.5f));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(SpawnTwoLazers(endpoint_4, endpoint_7, 0.8f, 0.5f));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(SpawnTwoLazers(endpoint_5, endpoint_6, 0.8f, 0.5f));
    }

    IEnumerator LazerBurstWave()
    {
        Vector3 endpoint_2 = topRightCorner.position;
        Vector3 endpoint_1 = topLeftCorner.position;
        float distance = (endpoint_1 - endpoint_2).magnitude;

        // Set the number of waves and the amplitude of the wave motion
        int numWaves = 6;

        for (int i = 0; i < numWaves; i++)
        {
            // Calculate the endpoints for each wave
            Vector3 waveStart = endpoint_1 + new Vector3((float)i * distance / numWaves, 0, 0);

            // Offset the endpoints based on the wave motion
            waveStart.y = endpoint_1.y;

            // Clamp the endpoints within the bounds of endpoint_1 and endpoint_2 on the x-axis
            waveStart.x = Mathf.Clamp(waveStart.x, endpoint_1.x, endpoint_2.x);

            // Spawn the lasers for this wave
            StartCoroutine(SpawnOneLazer(waveStart, 0.8f, 1.0f));
            yield return new WaitForSeconds(0.2f);
        }
    }


    IEnumerator ScanPlayground()
    {
        Vector3 rightCorner = topRightCorner.position;
        Vector3 endpoint_1 = topLeftCorner.position;
        float distance = (rightCorner - endpoint_1).magnitude;
        Vector3 endpoint_2 = endpoint_1 + new Vector3(distance/3, 0, 0);

        int numLasers = 4; // or 5
        GameObject[] lazers = new GameObject[4];

        for (int i = 0; i < numLasers; i++)
        {
            // Calculate the position of the lazer based on the number of lazers and the distance
            float xPosition = Mathf.Lerp(endpoint_1.x, endpoint_2.x, (float)i / (numLasers - 1));
            Vector3 position = new Vector3(xPosition, endpoint_1.y, endpoint_1.z);

            // Spawn the lazer
            lazers[i] = GetOneLazer(position, 0.8f, 8.0f);

            // Wait for a short period before spawning the next lazer
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(0.5f);

        float scanTimer = 4.0f;
        float scanSpeed = 5.0f;
        while(scanTimer > 0)
        {
            foreach (GameObject lazer in lazers)
            {
                if(lazer != null)
                lazer.transform.Translate(new Vector3(-Time.deltaTime * scanSpeed, 0, 0));
            }
            yield return null;
        }      
    }

    IEnumerator SpawnRandomLazers_3()
    {
        int numRepetitions = 5;
        Vector3 endpoint1 = topLeftCorner.position;
        Vector3 endpoint2 = topRightCorner.position;
        float distance = (endpoint2 - endpoint1).magnitude;

        for (int i = 0; i < numRepetitions; i++)
        {
            // Randomly select three positions on the x-axis within the range of the endpoints
            float randomPos1 = Random.Range(endpoint1.x, endpoint2.x - distance / 3f);
            float randomPos2 = Random.Range(randomPos1 + distance / 3f, endpoint2.x - distance / 3f);
            float randomPos3 = Random.Range(randomPos2 + distance / 3f, endpoint2.x);

            // Spawn three lazers at the random positions
            Debug.Log("YOO");
            SpawnOneLazer(new Vector3(randomPos1, endpoint1.y, endpoint1.z), 1.0f, 2.0f);
            SpawnOneLazer(new Vector3(randomPos2, endpoint1.y, endpoint1.z), 1.0f, 2.0f);
            SpawnOneLazer(new Vector3(randomPos3, endpoint1.y, endpoint1.z), 1.0f, 2.0f);

            yield return new WaitForSeconds(2.5f);
        }
    }

    IEnumerator SweepLazers_5()
    {
        Vector3 point1 = topRightCorner.position;
        Vector3 point2 = topLeftCorner.position;
        LazerSweepAttack(point1, point2);
        yield return new WaitForSeconds(1.0f);
        LazerSweepAttack(point1, point2);
        yield return new WaitForSeconds(1.0f);
        LazerSweepAttack(point1, point2);
        yield return new WaitForSeconds(1.0f);
        LazerSweepAttack(point1, point2);
        yield return new WaitForSeconds(1.0f);
        LazerSweepAttack(point1, point2);
        yield return new WaitForSeconds(1.0f);
    }
}
