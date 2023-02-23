using UnityEngine;

public delegate bool EntityLabelDisplayControlStrategy();

public enum EntityLabelDisplayControlStrategyType
{
    Always,
    DistanceDependent,
    Custom
}

public class EntityLabelHolder : MonoBehaviour
{
    private GameObject m_Label;
    private GameObject m_MainCamera;
    public float distanceThreshold;
    public EntityLabelDisplayControlStrategyType displayControlStrategyType;
    private EntityLabelDisplayControlStrategy m_DisplayControlStrategy;

    private GameObject m_Player;

    private bool EntityLabelAlwaysDisplay()
    {
        return true;
    }
    
    private bool EntityLabelDistanceDependentDisplay()
    {
        float distance = Vector3.Distance(m_Player.transform.position, transform.position);
        return distance < distanceThreshold;
    }
    
    private EntityLabelDisplayControlStrategy ParseStrategy(EntityLabelDisplayControlStrategyType type)
    {
        return type switch
        {
            EntityLabelDisplayControlStrategyType.Always => EntityLabelAlwaysDisplay,
            EntityLabelDisplayControlStrategyType.DistanceDependent => EntityLabelDistanceDependentDisplay,
            EntityLabelDisplayControlStrategyType.Custom => null,
            _ => null
        };
    }
    
    private void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        m_DisplayControlStrategy = ParseStrategy(displayControlStrategyType);
        if (transform.childCount > 0)
        {
            m_Label = transform.GetChild(0).gameObject;
        }
        else
        {
            Debug.Assert(false);
        }
        m_Label.SetActive(true);
    }
    
    private void Update()
    {
        if (m_DisplayControlStrategy != null)
        {
            m_Label.SetActive(m_DisplayControlStrategy());
        }
        
        // text should look away from camera all the time
        transform.LookAt(m_MainCamera.transform);
        transform.Rotate(0, 180, 0);
        transform.up = m_MainCamera.transform.up;
    }
}
