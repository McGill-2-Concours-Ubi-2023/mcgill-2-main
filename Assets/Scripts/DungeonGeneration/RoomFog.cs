using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RoomFog : MonoBehaviour
{
    public VisualEffect volumeFog;
    private GameObject player;
    // Start is called before the first frame update
    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        volumeFog.SetVector3("PlayerPosition", player.transform.position);
    }

    public void DissipateAmbientFog()
    {
        if(isActiveAndEnabled)
        StartCoroutine(TryDisableFog());
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
        yield return new WaitForSeconds(1.0f);
        volumeFog.enabled = false;
    }

    IEnumerator TryDisableFog()
    {
        bool fogEnabled = volumeFog.enabled;
        while (fogEnabled)
        {
            StartCoroutine(DissipateFog(0.5f));
            yield return new WaitForSeconds(0.5f);
        }
        StopAllCoroutines();
    }
}
