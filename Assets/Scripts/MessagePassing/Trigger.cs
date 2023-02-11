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
    public static void Trigger<T>(this GameObject gameObject, string triggerName, params object[] args)
        where T : ITrigger
    {
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
}

