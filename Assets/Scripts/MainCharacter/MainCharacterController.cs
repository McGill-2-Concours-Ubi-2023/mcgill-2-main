using Cinemachine;
using UnityEngine;

public class MainCharacterController : MonoBehaviour, IMainCharacterTriggers
{
    public float MovementSpeed;
    public CinemachineVirtualCamera Camera;
    [HideInInspector]
    public object NavActionData;
    
    private ISimpleInventory<SimpleCollectible> m_SimpleCollectibleInventory;
    
    private void Awake()
    {
        m_SimpleCollectibleInventory = new SimpleInventory<SimpleCollectible>();
    }
    
    public void CollectCoin()
    {
        m_SimpleCollectibleInventory.AddItem(SimpleCollectible.Coin);
    }
}

