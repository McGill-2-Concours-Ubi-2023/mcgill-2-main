using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

public interface ITrigger
{

}

public static class TriggerExt
{
    private class TriggerResolver
    {
        private static TriggerResolver m_Instance;
        private readonly static object m_Lock = new object();
        internal static TriggerResolver Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    lock (m_Lock)
                    {
                        m_Instance ??= new TriggerResolver();
                    }
                }

                return m_Instance;
            }
        }

        internal Action ResolveTrigger<T>(T impl, string triggerName) where T : ITrigger
        {
            return null;
        }
        
        internal Action<T1> ResolveTrigger<T, T1>(T impl, string triggerName, T1 t1) where T : ITrigger
        {
            return null;
        }
        
        internal Action<T1, T2> ResolveTrigger<T, T1, T2>(T impl, string triggerName, T1 t1, T2 t2) where T : ITrigger
        {
            return null;
        }
    }
    
    public static void Trigger<T>(this GameObject gameObject, string triggerName, params object[] args)
        where T : ITrigger
    {
        if (!gameObject)
        {
            return;
        }
        
        T[] impls = gameObject.GetComponents<T>();
        MethodInfo method = typeof(T).GetMethods().FirstOrDefault(m => m.Name == triggerName);
        if (method == null)
        {
            return;
        }
        
        foreach (T impl in impls)
        {
            method.Invoke(impl, args);
        }
    }

    public static void TriggerUp<T>(this GameObject gameObject, string triggerName, params object[] args)
        where T : ITrigger
    {
        if (!gameObject)
        {
            return;
        }
        
        gameObject.Trigger<T>(triggerName, args);
        Transform parentTransform = gameObject.transform.parent;
        if (parentTransform)
        {
            parentTransform.gameObject.TriggerUp<T>(triggerName, args);
        }
    }

    public static void TriggerDown<T>(this GameObject gameObject, string triggerName, params object[] args)
        where T : ITrigger
    {
        if (!gameObject)
        {
            return;
        }
        
        gameObject.Trigger<T>(triggerName, args);
        foreach (Transform childTransform in gameObject.transform)
        {
            childTransform.gameObject.TriggerDown<T>(triggerName, args);
        }
    }

    public static void TriggerAll<T>(this GameObject gameObject, string triggerName, params object[] args)
        where T : ITrigger
    {
        // trigger up then down
        if (!gameObject)
        {
            return;
        }
        Transform parentTransform = gameObject.transform.parent;
        if (parentTransform)
        {
            parentTransform.gameObject.TriggerUp<T>(triggerName, args);
        }
        gameObject.TriggerDown<T>(triggerName, args);
    }
}

