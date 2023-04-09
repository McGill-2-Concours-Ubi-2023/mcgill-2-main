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
    void StartFight() { }
}

public class FinalBossController : MonoBehaviour, IBossTriggers, IHealthObserver
{

    [Header("Playground Properties")]
    public Transform topRightCorner;
    public CinemachineCameraShake cameraShake;
    public Transform topLeftCorner;
    public Transform playGround;
    public bool Attack = false;
    public Transform playerTransform;
    public Animator tentacleAnimator;
    [Header("LazerProperties")]
    public GameObject lazerPrefab;
    public Health health;
    private HealthObserverAdapter m_HealthObserverAdapter;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.isLoading = false;
        m_HealthObserverAdapter = gameObject.AddComponent<HealthObserverAdapter>();
        m_HealthObserverAdapter.ConcreteObserver = this;
    }
    
    public void OnHealthChange(float change, float currentHealth)
    {
        // send to UI
        Debug.Log($"Boss health change : {change} to : {currentHealth}");
    }

    public void OnDeath()
    {
        Debug.Log("Boss Death");
        // TODO
        Destroy(gameObject);
    }


    public void StartFight()
    {
        LazerSweepAttack(topRightCorner.position, topLeftCorner.position);
        //tentacleAnimator.SetTrigger("Shield");
        StartCoroutine(FightCoroutine());
    }

    private Dictionary<System.Func<IEnumerator>, int> functionCallCounts = new Dictionary<System.Func<IEnumerator>, int>();
    private int totalFunctionCalls = 0;
    IEnumerator FightCoroutine()
    {
        yield return new WaitForSeconds(15.0f);

        while (true)
        {
            // Create a list of function indices
            List<int> functionIndices = new List<int>();
            for (int i = 0; i < 7; i++)
            {
                if (i == 1 || i == 7)
                {
                    // Always include tentacle attack
                    functionIndices.Add(i);
                }
                else
                {
                    System.Func<IEnumerator> function = null;
                    switch (i)
                    {
                        case 0:
                            function = WaveLasers;
                            break;
                        case 2:
                            function = LazerBurstPeriodic_2;
                            break;
                        case 3:
                            function = LazerBurstWave;
                            break;
                        case 4:
                            function = StarLazer;
                            break;
                        case 5:
                            function = ScanPlayground;
                            break;
                        case 6:
                            function = SweepLazers_5;
                            break;
                        default:
                            break;
                    }

                    if (function != null)
                    {
                        // Add function index to list according to its probability
                        int functionCalls = functionCallCounts.ContainsKey(function) ? functionCallCounts[function] : 0;
                        float probability = Mathf.Pow(0.5f, functionCalls);
                        int numIndicesToAdd = Mathf.FloorToInt(probability * 100.0f);
                        for (int j = 0; j < numIndicesToAdd; j++)
                        {
                            functionIndices.Add(i);
                        }
                    }
                }
            }

            // Select a random function index from the list
            int randomIndex = Random.Range(0, functionIndices.Count);
            int patternIndex = functionIndices[randomIndex];

            // Call the selected function
            switch (patternIndex)
            {
                case 0:
                    StartCoroutine(WaveLasers());
                    break;
                case 1:
                case 7:
                    tentacleAnimator.SetTrigger("Attack");
                    break;
                case 2:
                    StartCoroutine(LazerBurstPeriodic_2());
                    break;
                case 3:
                    StartCoroutine(LazerBurstWave());
                    break;
                case 4:
                    StartCoroutine(StarLazer());
                    break;
                case 5:
                    StartCoroutine(ScanPlayground());
                    break;
                case 6:
                    StartCoroutine(SweepLazers_5());
                    break;
                default:
                    break;
            }

            // Update function call counts
            System.Func<IEnumerator> calledFunction = null;
            switch (patternIndex)
            {
                case 0:
                    calledFunction = WaveLasers;
                    break;
                case 2:
                    calledFunction = LazerBurstPeriodic_2;
                    break;
                case 3:
                    calledFunction = LazerBurstWave;
                    break;
                case 4:
                    calledFunction = StarLazer;
                    break;
                case 5:
                    calledFunction = ScanPlayground;
                    break;
                case 6:
                    calledFunction = SweepLazers_5;
                    break;
                default:
                    break;
            }
            if (calledFunction != null)
            {
                totalFunctionCalls++;
                if (!functionCallCounts.ContainsKey(calledFunction))
                {
                    functionCallCounts[calledFunction] = 1;
                }
                else
                {
                    functionCallCounts[calledFunction]++;
                }
            }
            yield return new WaitForSeconds(Random.Range(6.0f, 12.0f));
        }
    }


    IEnumerator Shield(float shieldTime)
    {
        tentacleAnimator.SetTrigger("Shield");
        {
            using HLockGuard guard = health.Lock();
            yield return new WaitForSeconds(shieldTime);
        }
        tentacleAnimator.SetTrigger("StopShield");
    }

    private GameObject GetOneLazer(Vector3 position, float lazerChargeTime, float lazerbeamDuration, float YRotation)
    {
        var obj_1 = Instantiate(lazerPrefab);
        obj_1.transform.position = position;
        obj_1.transform.Rotate(0f, YRotation, 0f);
        StartCoroutine(AwakeLazer(obj_1, lazerChargeTime, lazerbeamDuration));
        return obj_1;
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

    //USE THOSE FUNCTIONS BELOW
    
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

    IEnumerator WaveLasers()
    {
        float waveTime = 8.0f;
        Vector3 center = new Vector3(playGround.transform.position.x,
           playGround.transform.position.y + 2.0f, playGround.transform.position.z);

        while (waveTime > 0)
        {
            Vector3 playerPos = playerTransform.position;
            GameObject lazer = GetOneLazer(center, 1.0f, 1.0f, 0);
            Quaternion rotation = Quaternion.FromToRotation(lazer.transform.forward,
                (playerPos - lazer.transform.position).normalized);
            lazer.transform.rotation *= rotation;
            float randWait = Random.Range(1.0f, 2.0f);
            waveTime -= randWait;
            yield return new WaitForSeconds(randWait);
        }
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

    IEnumerator StarLazer()
    {
        Vector3 center = new Vector3(playGround.transform.position.x,
            playGround.transform.position.y + 2.0f, playGround.transform.position.z);
        float rotatingTime = 6.0f;
        float rotatingSpeed =   8.0f;
        List<GameObject> lasers = new List<GameObject>();

        for(int i = 0; i < 10; i++)
        {
            float YRotation = Mathf.Lerp(0, 360, (float)i / 10);
            GameObject laser = GetOneLazer(center, 1.0f, 7.0f, 180);
            laser.transform.Rotate(0,YRotation,0);
            lasers.Add(laser);
        }

        while (rotatingTime > 0)
        {
            rotatingTime -= Time.deltaTime;
            foreach (GameObject laser in lasers)
            {
                laser.transform.Rotate(0, Time.deltaTime * rotatingSpeed, 0);
            }
            yield return null;
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
            lazers[i] = GetOneLazer(position, 0.8f, 8.0f, 180);

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
