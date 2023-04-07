using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;

public interface HSimpleInventoryLock : IDisposable
{
    
}

public interface ISimpleInventory<in TKey>
{
    public void AddItem(TKey key);
    public void RemoveItem(TKey key);
    public void AddInBulk(TKey key, int count);
    public void RemoveInBulk(TKey key, int count);
    public int GetCount(TKey key);
    public int GetMax(TKey key);
    public void ResetAll();
    public HSimpleInventoryLock Lock();
}

public class InventoryFullException<T> : System.Exception
{
    public InventoryFullException(int max) : base($"Inventory for {typeof(T)} is full. Max: {max}")
    {
    }
}

public class InventoryEmptyException<T> : System.Exception
{
    public InventoryEmptyException() : base($"Inventory for {typeof(T)} is empty")
    {
    }
}

public class SimpleInventory<TKey> : ISimpleInventory<TKey>
{
    private readonly IDictionary<TKey, int> m_InventoryCounts;
    private readonly IReadOnlyDictionary<TKey, int> m_InventoryMaximums;
    private bool m_SuppressWrites = false;
    private readonly object m_Lock = new object();

    private class SimpleInventoryLock : HSimpleInventoryLock
    {
        private readonly SimpleInventory<TKey> m_Inventory;

        internal SimpleInventoryLock([NotNull] SimpleInventory<TKey> inventory)
        {
            m_Inventory = inventory;
            Monitor.Enter(m_Inventory.m_Lock);
            m_Inventory.m_SuppressWrites = true;
        }
        
        ~SimpleInventoryLock()
        {
            Dispose();
        }

        public void Dispose()
        {
            Monitor.Exit(m_Inventory.m_Lock);
            m_Inventory.m_SuppressWrites = false;
            GC.SuppressFinalize(this);
        }
    }

    public SimpleInventory(IReadOnlyDictionary<TKey, int> inventoryMaximums)
    {
        m_InventoryCounts = new Dictionary<TKey, int>();
        m_InventoryMaximums = inventoryMaximums;
    }
    
    public SimpleInventory()
    {
        m_InventoryCounts = new Dictionary<TKey, int>();
        m_InventoryMaximums = new Dictionary<TKey, int>();
    }

    public void AddItem(TKey key)
    {
        if (m_SuppressWrites)
        {
            return;
        }
        EnsureKeyExists(key);
        if (m_InventoryCounts[key] >= GetMax(key))
        {
            throw new InventoryFullException<TKey>(GetMax(key));
        }
        m_InventoryCounts[key]++;
    }

    public void RemoveItem(TKey key)
    {
        if (m_SuppressWrites)
        {
            return;
        }
        EnsureKeyExists(key);
        if (m_InventoryCounts[key] <= 0)
        {
            throw new InventoryEmptyException<TKey>();
        }
        m_InventoryCounts[key]--;
    }

    public void AddInBulk(TKey key, int count)
    {
        if (m_SuppressWrites)
        {
            return;
        }
        EnsureKeyExists(key);
        if (m_InventoryCounts[key] + count > GetMax(key))
        {
            throw new InventoryFullException<TKey>(GetMax(key));
        }
        
        m_InventoryCounts[key] += count;
    }

    public void RemoveInBulk(TKey key, int count)
    {
        EnsureKeyExists(key);
        if (m_InventoryCounts[key] - count < 0)
        {
            throw new InventoryEmptyException<TKey>();
        }
        
        m_InventoryCounts[key] -= count;
    }

    public int GetCount(TKey key)
    {
        EnsureKeyExists(key);
        return m_InventoryCounts[key];
    }
    
    public int GetMax(TKey key)
    {
        return m_InventoryMaximums.ContainsKey(key) ? m_InventoryMaximums[key] : int.MaxValue;
    }
    
    private void EnsureKeyExists(TKey key)
    {
        if (!m_InventoryCounts.ContainsKey(key))
        {
            m_InventoryCounts[key] = 0;
        }
    }

    public void ResetAll()
    {
        if (m_SuppressWrites)
        {
            return;
        }
        m_InventoryCounts.Clear();
    }

    public HSimpleInventoryLock Lock()
    {
        return new SimpleInventoryLock(this);
    }
}

