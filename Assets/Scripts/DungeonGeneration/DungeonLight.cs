using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DungeonLight : MonoBehaviour
{
    protected Animator animator;
    protected Light _light;
    protected static float maxRenderDistance = 12.0f;
    // Start is called before the first frame update

    private void OnEnable()
    {
        TryGetComponent<Animator>(out animator);
    }

    public void UpdateLight(Vector3 position)
    {
        if (_light == null) _light = GetComponent<Light>();
        float distance = (transform.position - position).magnitude;
        Debug.Log(distance);
        if (distance > maxRenderDistance)
        {
            _light.enabled = false;
        }
        else
        {
            _light.enabled = true;
        }
    }

    public abstract void Flicker();
}
