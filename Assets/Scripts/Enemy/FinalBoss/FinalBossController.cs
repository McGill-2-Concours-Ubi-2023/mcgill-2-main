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
            LazerSweepAttack(topRightCorner.position, topLeftCorner.position);
            //StartCoroutine(LazerBurst());
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

    IEnumerator SpawnTwoLazers(Vector3 endpoint1, Vector3 endpoint2, float lazerbeamDuration)
    {
        const float lazerChargeTime = 0.5f;
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

    IEnumerator SweepLazers(VisualEffect lazer1, VisualEffect lazer2)
    {
        const float lazerChargeTime = 1.0f;
        const float lazerRotationSpeed = 5.0f;
        const float lazerbeamDuration = 4.0f;
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

    IEnumerator LazerBurst()
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
        StartCoroutine(SpawnTwoLazers(endpoint_1, endpoint_2, 0.7f));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(SpawnTwoLazers(endpoint_3, endpoint_8, 0.7f));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(SpawnTwoLazers(endpoint_4, endpoint_7, 0.7f));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(SpawnTwoLazers(endpoint_5, endpoint_6, 0.7f));
    }
}
