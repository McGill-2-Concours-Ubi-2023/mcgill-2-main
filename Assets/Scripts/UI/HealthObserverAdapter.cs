using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class HealthObserverAdapter : MonoBehaviour
{
    private Health m_Health;
    private IHealthObserver m_ConcreteObserver;

    public IHealthObserver ConcreteObserver
    {
        set
        {
            if (m_ConcreteObserver != null)
            {
                throw new Exception("Illegal state: ConcreteObserver is already set");
            }
            m_ConcreteObserver = value;
            m_Health.OnHealthChange += m_ConcreteObserver.OnHealthChange;
            m_Health.OnDeath += m_ConcreteObserver.OnDeath;
        }
    }

    private void Awake()
    {
        m_Health = GetComponent<Health>();
    }
    
    private void OnDestroy()
    {
        m_Health.OnHealthChange -= m_ConcreteObserver.OnHealthChange;
        m_Health.OnDeath -= m_ConcreteObserver.OnDeath;
        IDisposable disposable = m_ConcreteObserver as IDisposable;
        disposable?.Dispose();
    }
}
