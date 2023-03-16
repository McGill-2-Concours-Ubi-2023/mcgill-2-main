using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

using method_identifier_t = System.UInt64;

public interface ITrigger
{

}

public static class TriggerExt
{
    private static Task m_LoadingTask;
    
    // registry of function pointers by finding all derived types of ITrigger and their implementations
    static TriggerExt()
    {
        m_LoadingTask = Task.Run(LoadTriggerMethods);
    }
    
    private static async Task LoadTriggerMethods()
    {
        // find all derived types of ITrigger
        List<Type> triggerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsInterface && typeof(ITrigger).IsAssignableFrom(t)).ToList();
        
        await Task.Yield();
        
        // find all implementations of ITrigger
        List<Type> triggerImpls = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && typeof(ITrigger).IsAssignableFrom(t)).ToList();
        
        await Task.Yield();
        
        // find all methods on ITrigger implementations
        IEnumerable<MethodInfo> triggerMethods = triggerImpls.AsParallel()
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance));
        
        await Task.Yield();
        
        // filter out methods on ITrigger implementations that are not in the ITrigger derived interfaces
        IEnumerable<MethodInfo> triggerMethodsFiltered = triggerMethods.AsParallel()
            .Where(m => triggerTypes.Any(t => t.GetMethod(m.Name) != null));
            
        await Task.Yield();
        
        ConcurrentDictionary<method_identifier_t, nint> nativeDelegateCache = new ConcurrentDictionary<method_identifier_t, nint>();

        triggerMethodsFiltered.AsParallel().ForAll(m =>
        {
            
            ParameterInfo[] methodParams = m.GetParameters();
            int paramCount = methodParams.Length;
            nint ptr = m.MethodHandle.GetFunctionPointer();
            method_identifier_t cacheKey = GetCacheKey(m.DeclaringType, m.Name);
            nativeDelegateCache[cacheKey] = ptr;
            Debug.Log($"Loaded trigger method: {cacheKey} with {paramCount} parameters at address 0x{(ulong)ptr:x}");
        });
        
        await Task.Yield();
        foreach ((method_identifier_t key, nint value) in nativeDelegateCache)
        {
            NativeDelegateCache[key] = value;
        }
    }
    
        
    private readonly static Dictionary<method_identifier_t, nint> NativeDelegateCache = new Dictionary<method_identifier_t, nint>();
    
    private static ulong GetCacheKey(Type type, string triggerName)
    {
        return ((ulong) type.GetHashCode()) << 32 | (uint)triggerName.GetHashCode();
    }

    #region Trigger 0 args
    public static void Trigger<T>(this GameObject gameObject, string triggerName)
    {
        if (!gameObject)
        {
            return;
        }
        
        if (!m_LoadingTask.IsCompleted)
        {
            m_LoadingTask.Wait();
        }
        
        T[] impls = gameObject.GetComponents<T>();
        
        foreach (T impl in impls)
        {
            unsafe
            {
                method_identifier_t cacheKey = GetCacheKey(impl.GetType(), triggerName);
                if (NativeDelegateCache.TryGetValue(cacheKey, out nint ptr))
                {
                    delegate* managed<object, void> del = (delegate* managed<object, void>)ptr;
                    del(impl);
                }
            }
        }
    }
    
    
    public static void TriggerUp<T>(this GameObject gameObject, string triggerName)
        where T : ITrigger
    {
        if (!gameObject)
        {
            return;
        }
        
        gameObject.Trigger<T>(triggerName);
        Transform parentTransform = gameObject.transform.parent;
        if (parentTransform)
        {
            parentTransform.gameObject.TriggerUp<T>(triggerName);
        }
    }

    public static void TriggerDown<T>(this GameObject gameObject, string triggerName)
        where T : ITrigger
    {
        if (!gameObject)
        {
            return;
        }
        
        gameObject.Trigger<T>(triggerName);
        foreach (Transform childTransform in gameObject.transform)
        {
            childTransform.gameObject.TriggerDown<T>(triggerName);
        }
    }

    public static void TriggerAll<T>(this GameObject gameObject, string triggerName)
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
            parentTransform.gameObject.TriggerUp<T>(triggerName);
        }
        gameObject.TriggerDown<T>(triggerName);
    }
    
    public static void TriggerHierarchy<T>(this GameObject gameObject, string triggerName)
        where T : ITrigger
    {
        if (!gameObject)
        {
            return;
        }
        GameObject root = gameObject.transform.root.gameObject;
        root.TriggerDown<T>(triggerName);
    }
    
    #endregion
    
    #region Trigger 1 arg
    
    public static void Trigger<T, T1>(this GameObject gameObject, string triggerName, T1 arg1)
    {
        if (!gameObject)
        {
            return;
        }
        
        if (!m_LoadingTask.IsCompleted)
        {
            m_LoadingTask.Wait();
        }
        
        T[] impls = gameObject.GetComponents<T>();
        
        foreach (T impl in impls)
        {
            unsafe
            {
                method_identifier_t cacheKey = GetCacheKey(impl.GetType(), triggerName);
                if (NativeDelegateCache.TryGetValue(cacheKey, out nint ptr))
                {
                    delegate* managed<object, T1, void> del = (delegate* managed<object, T1, void>)ptr;
                    del(impl, arg1);
                }
            }
        }
    }
    
    public static void TriggerUp<T, T1>(this GameObject gameObject, string triggerName, T1 arg1)
        where T : ITrigger
    {
        if (!gameObject)
        {
            return;
        }
        
        gameObject.Trigger<T, T1>(triggerName, arg1);
        Transform parentTransform = gameObject.transform.parent;
        if (parentTransform)
        {
            parentTransform.gameObject.TriggerUp<T, T1>(triggerName, arg1);
        }
    }

    public static void TriggerDown<T, T1>(this GameObject gameObject, string triggerName, T1 arg1)
        where T : ITrigger
    {
        if (!gameObject)
        {
            return;
        }
        
        gameObject.Trigger<T, T1>(triggerName, arg1);
        foreach (Transform childTransform in gameObject.transform)
        {
            childTransform.gameObject.TriggerDown<T, T1>(triggerName, arg1);
        }
    }

    public static void TriggerAll<T, T1>(this GameObject gameObject, string triggerName, T1 arg1)
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
            parentTransform.gameObject.TriggerUp<T, T1>(triggerName, arg1);
        }
        gameObject.TriggerDown<T, T1>(triggerName, arg1);
    }
    
    public static void TriggerHierarchy<T, T1>(this GameObject gameObject, string triggerName, T1 arg1)
        where T : ITrigger
    {
        if (!gameObject)
        {
            return;
        }
        GameObject root = gameObject.transform.root.gameObject;
        root.TriggerDown<T, T1>(triggerName, arg1);
    }
    
    #endregion

    #region Trigger 2 args
    public static void Trigger<T, T1, T2>(this GameObject gameObject, string triggerName, T1 arg1, T2 arg2)
    {
        if (!gameObject)
        {
            return;
        }
        
        if (!m_LoadingTask.IsCompleted)
        {
            m_LoadingTask.Wait();
        }
        
        T[] impls = gameObject.GetComponents<T>();
        
        foreach (T impl in impls)
        {
            unsafe
            {
                method_identifier_t cacheKey = GetCacheKey(impl.GetType(), triggerName);
                if (NativeDelegateCache.TryGetValue(cacheKey, out nint ptr))
                {
                    delegate* managed<object, T1, T2, void> del = (delegate* managed<object, T1, T2, void>)ptr;
                    del(impl, arg1, arg2);
                }
            }
        }
    }
    
   
    public static void TriggerUp<T, T1, T2>(this GameObject gameObject, string triggerName, T1 arg1, T2 arg2)
        where T : ITrigger
    {
        if (!gameObject)
        {
            return;
        }
        
        gameObject.Trigger<T, T1, T2>(triggerName, arg1, arg2);
        Transform parentTransform = gameObject.transform.parent;
        if (parentTransform)
        {
            parentTransform.gameObject.TriggerUp<T>(triggerName);
        }
    }

    public static void TriggerDown<T, T1, T2>(this GameObject gameObject, string triggerName, T1 arg1, T2 arg2)
        where T : ITrigger
    {
        if (!gameObject)
        {
            return;
        }
        
        gameObject.Trigger<T, T1, T2>(triggerName, arg1, arg2);
        foreach (Transform childTransform in gameObject.transform)
        {
            childTransform.gameObject.TriggerDown<T, T1, T2>(triggerName, arg1, arg2);
        }
    }

    public static void TriggerAll<T, T1, T2>(this GameObject gameObject, string triggerName, T1 arg1, T2 arg2)
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
            parentTransform.gameObject.TriggerUp<T, T1, T2>(triggerName, arg1, arg2);
        }
        gameObject.TriggerDown<T, T1, T2>(triggerName, arg1, arg2);
    }
    
    public static void TriggerHierarchy<T, T1, T2>(this GameObject gameObject, string triggerName, T1 arg1, T2 arg2)
        where T : ITrigger
    {
        if (!gameObject)
        {
            return;
        }
        GameObject root = gameObject.transform.root.gameObject;
        root.TriggerDown<T, T1, T2>(triggerName, arg1, arg2);
    }
    
    #endregion
    
    #region Trigger 3 args
    public static void Trigger<T, T1, T2, T3>(this GameObject gameObject, string triggerName, T1 arg1, T2 arg2, T3 arg3)
    {
        if (!gameObject)
        {
            return;
        }
        
        if (!m_LoadingTask.IsCompleted)
        {
            m_LoadingTask.Wait();
        }
        
        T[] impls = gameObject.GetComponents<T>();
        
        foreach (T impl in impls)
        {
            unsafe
            {
                method_identifier_t cacheKey = GetCacheKey(impl.GetType(), triggerName);
                if (NativeDelegateCache.TryGetValue(cacheKey, out nint ptr))
                {
                    delegate* managed<object, T1, T2, T3, void> del = (delegate* managed<object, T1, T2, T3, void>)ptr;
                    del(impl, arg1, arg2, arg3);
                }
            }
        }
    }
    
    public static void TriggerUp<T, T1, T2, T3>(this GameObject gameObject, string triggerName, T1 arg1, T2 arg2, T3 arg3)
        where T : ITrigger
    {
        if (!gameObject)
        {
            return;
        }
        
        gameObject.Trigger<T, T1, T2, T3>(triggerName, arg1, arg2, arg3);
        Transform parentTransform = gameObject.transform.parent;
        if (parentTransform)
        {
            parentTransform.gameObject.TriggerUp<T, T1, T2, T3>(triggerName, arg1, arg2, arg3);
        }
    }

    public static void TriggerDown<T, T1, T2, T3>(this GameObject gameObject, string triggerName, T1 arg1, T2 arg2, T3 arg3)
        where T : ITrigger
    {
        if (!gameObject)
        {
            return;
        }
        
        gameObject.Trigger<T, T1, T2, T3>(triggerName, arg1, arg2, arg3);
        foreach (Transform childTransform in gameObject.transform)
        {
            childTransform.gameObject.TriggerDown<T, T1, T2, T3>(triggerName, arg1, arg2, arg3);
        }
    }

    public static void TriggerAll<T, T1, T2, T3>(this GameObject gameObject, string triggerName, T1 arg1, T2 arg2, T3 arg3)
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
            parentTransform.gameObject.TriggerUp<T, T1, T2, T3>(triggerName, arg1, arg2, arg3);
        }
        gameObject.TriggerDown<T, T1, T2, T3>(triggerName, arg1, arg2, arg3);
    }
    
    public static void TriggerHierarchy<T, T1, T2, T3>(this GameObject gameObject, string triggerName, T1 arg1, T2 arg2, T3 arg3)
        where T : ITrigger
    {
        if (!gameObject)
        {
            return;
        }
        GameObject root = gameObject.transform.root.gameObject;
        root.TriggerDown<T, T1, T2, T3>(triggerName, arg1, arg2, arg3);
    }
    
    #endregion
}

