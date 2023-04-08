using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICrateTriggers : ITrigger
{
    public void OnCollectCrate() { }
}

public class Crate : MonoBehaviour, ICrateTriggers
{
    private void OnDestroy()
    {
        try
        {
            GameObject.FindWithTag("Player")?.Trigger<ICrateTriggers>(nameof(ICrateTriggers.OnCollectCrate));
        }
        catch (Exception e)
        {
            // ignored
        }
    }
}
