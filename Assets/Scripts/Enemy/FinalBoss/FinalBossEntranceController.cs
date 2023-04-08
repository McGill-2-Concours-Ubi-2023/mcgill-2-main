using UnityEngine;

public class FinalBossEntranceController : MonoBehaviour
{
    private bool m_Entered = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameObject.FindWithTag("Player"))
        {
            if (m_Entered) return;
            m_Entered = true;
            // start fight sequence
            other.gameObject.Trigger<IBossFightTriggers>(nameof(IBossFightTriggers.StartBossFight));
        }
    }
}
