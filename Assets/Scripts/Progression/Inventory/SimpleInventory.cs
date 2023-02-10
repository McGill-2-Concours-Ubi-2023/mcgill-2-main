using System.Collections.Generic;
using UnityEngine;

public interface ISimpleInventory<in TKey>
{
    public void AddItem(TKey key);
    public void RemoveItem(TKey key);
    public int GetCount(TKey key);
    public int GetMax(TKey key);
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
        if (m_InventoryCounts.ContainsKey(key))
        {
            if (m_InventoryCounts[key] >= GetMax(key))
            {
                throw new InventoryFullException<TKey>(GetMax(key));
            }
            m_InventoryCounts[key]++;
        }
        else
        {
            if (GetMax(key) <= 0)
            {
                throw new InventoryFullException<TKey>(GetMax(key));
            }
            m_InventoryCounts[key] = 1;
        }
    }

    public void RemoveItem(TKey key)
    {
        if (m_InventoryCounts.ContainsKey(key))
        {
            if (m_InventoryCounts[key] <= 0)
            {
                throw new InventoryEmptyException<TKey>();
            }
            m_InventoryCounts[key]--;
        }
        else
        {
            m_InventoryCounts[key] = 0;
            throw new InventoryEmptyException<TKey>();
        }
    }

    public int GetCount(TKey key)
    {
        return m_InventoryCounts.ContainsKey(key) ? m_InventoryCounts[key] : 0;
    }
    
    public int GetMax(TKey key)
    {
        return m_InventoryMaximums.ContainsKey(key) ? m_InventoryMaximums[key] : int.MaxValue;
    }
}

