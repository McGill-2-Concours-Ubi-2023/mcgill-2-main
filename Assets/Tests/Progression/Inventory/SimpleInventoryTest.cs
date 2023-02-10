using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SimpleInventoryTest
{
    private enum TestItemType
    {
        A, B, C
    }
    
    [Test]
    public void SimpleInventoryNoArgConstructorTest()
    {
        ISimpleInventory<TestItemType> inventory = new SimpleInventory<TestItemType>();
        Assert.AreEqual(int.MaxValue, inventory.GetMax(TestItemType.A));
    }
    
    [Test]
    public void SimpleInventoryMaxCountTest()
    {
        ISimpleInventory<TestItemType> inventory = new SimpleInventory<TestItemType>(new Dictionary<TestItemType, int>
        {
            {TestItemType.A, 5},
            {TestItemType.B, 10},
            {TestItemType.C, 15}
        });
        Assert.AreEqual(5, inventory.GetMax(TestItemType.A));
        Assert.AreEqual(10, inventory.GetMax(TestItemType.B));
        Assert.AreEqual(15, inventory.GetMax(TestItemType.C));
    }

    [Test]
    public void SimpleInventoryExceptionTest()
    {
        ISimpleInventory<TestItemType> inventory = new SimpleInventory<TestItemType>(new Dictionary<TestItemType, int>
        {
            {TestItemType.A, 5},
            {TestItemType.B, 10}
        });
        Assert.Throws<InventoryFullException<TestItemType>>(() =>
        {
            for (int i = 0; i < 6; i++)
            {
                inventory.AddItem(TestItemType.A);
            }
        });
        Assert.Throws<InventoryEmptyException<TestItemType>>(() =>
        {
            inventory.RemoveItem(TestItemType.B);
        });
    }

    [Test]
    public void SimpleInventoryNormalTest()
    {
        ISimpleInventory<TestItemType> inventory = new SimpleInventory<TestItemType>(new Dictionary<TestItemType, int>
        {
            {TestItemType.A, 5},
            {TestItemType.B, 10}
        });
        inventory.AddItem(TestItemType.B);
        Assert.AreEqual(1, inventory.GetCount(TestItemType.B));
        inventory.RemoveItem(TestItemType.B);
        Assert.AreEqual(0, inventory.GetCount(TestItemType.B));
        inventory.AddItem(TestItemType.C);
        Assert.AreEqual(1, inventory.GetCount(TestItemType.C));
    }
    
    [UnityTest]
    public IEnumerator SimpleInventoryTestWithEnumeratorPasses()
    {
        yield return null;
    }
}
