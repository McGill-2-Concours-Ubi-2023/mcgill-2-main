using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

public interface ITrigger
{

}

public static class TriggerExt
{
    private readonly static Dictionary<string, Delegate> DelegateCache = new Dictionary<string, Delegate>();

    // GPT-4 wrote me this. Don't ask me how it works.
    public static void Trigger<T>(this GameObject gameObject, string triggerName, params object[] args)
        where T : ITrigger
    {
        if (!gameObject)
        {
            return;
        }

        T[] impls = gameObject.GetComponents<T>();
        string cacheKey = typeof(T).FullName + "." + triggerName;

        if (!DelegateCache.TryGetValue(cacheKey, out Delegate del))
        {
            MethodInfo method = typeof(T).GetMethod(triggerName);
            if (method == null)
            {
                return;
            }

            ParameterInfo[] methodParams = method.GetParameters();

            if (methodParams.Length != args.Length)
            {
                throw new ArgumentException("Number of arguments provided does not match the method's parameter count.");
            }

            Type delegateType;
            if (methodParams.Length == 0)
            {
                delegateType = typeof(Action<T>);
            }
            else
            {
                Type[] genericArgs = new[] { typeof(T) }.Concat(methodParams.Select(p => p.ParameterType)).ToArray();
                delegateType = GetActionType(genericArgs);
            }
            del = Delegate.CreateDelegate(delegateType, method);
            DelegateCache[cacheKey] = del;
        }
        
        object[] argumentArray = new object[] { null }.Concat(args).ToArray();

        foreach (T impl in impls)
        {
            argumentArray[0] = impl;
            del.DynamicInvoke(argumentArray);
        }
    }

    private static Type GetActionType(params Type[] typeArgs)
    {
        int typeArgCount = typeArgs.Length;
        Type genericActionType = typeArgCount switch
        {
            1 => typeof(Action<>),
            2 => typeof(Action<,>),
            3 => typeof(Action<,,>),
            4 => typeof(Action<,,,>),
            _ => throw new NotSupportedException("Methods with more than 3 parameters are not supported."),
        };

        return genericActionType.MakeGenericType(typeArgs);
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
    
    public static void TriggerHierarchy<T>(this GameObject gameObject, string triggerName, params object[] args)
        where T : ITrigger
    {
        if (!gameObject)
        {
            return;
        }
        GameObject root = gameObject.transform.root.gameObject;
        root.TriggerDown<T>(triggerName, args);
    }
}

