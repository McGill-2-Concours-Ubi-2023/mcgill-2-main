using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Portal : MonoBehaviour
{
    private GameObject player;
    private InputActionAsset m_InputActionAsset;
    private bool canTeleport;

    private void OnEnable()
    {
        m_InputActionAsset["Interact"].performed += ctx => EnterPortal();
        player = GameObject.FindGameObjectWithTag("Player");
        canTeleport = false;
    }

    private async void EnterPortal()
    {
        if (canTeleport)
        {
            player.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.FreezeOnCurrentState));
            player.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.ActivateTrail));
            await Task.Delay(3000);
            GameManager.Instance.LoadScene(2);
        }     
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTeleport = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTeleport = false;
        }
    }
}
