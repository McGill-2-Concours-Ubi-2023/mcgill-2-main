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
        player = GameObject.FindGameObjectWithTag("Player");
        m_InputActionAsset = player.GetComponent<PlayerInput>().actions;
        m_InputActionAsset["Interact"].performed += ctx => EnterPortal();
        canTeleport = false;
    }

    private async void EnterPortal()
    {
        if (canTeleport)
        {
            DontDestroyOnLoad(player.gameObject);
            player.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.FreezeOnCurrentState));
            player.Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.Desintegrate));
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
