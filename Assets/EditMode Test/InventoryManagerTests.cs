using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class InventoryManagerTests
{
    private GameObject inventoryObject;
    private InventoryManager inventoryManager;

    [SetUp]
    public void SetUp()
    {
        ResetInventoryManagerInstance();

        inventoryObject = new GameObject("InventoryManager_TestObject");
        inventoryManager = inventoryObject.AddComponent<InventoryManager>();
    }

    [TearDown]
    public void TearDown()
    {
        if (inventoryObject != null)
        {
            Object.DestroyImmediate(inventoryObject);
        }

        ResetInventoryManagerInstance();
    }

    [Test]
    public void AddItem_AddsItemToInventory()
    {
        inventoryManager.AddItem("Apple", 2f, null);

        Assert.AreEqual(1, inventoryManager.inventoryItems.Count);
        Assert.AreEqual("Apple", inventoryManager.inventoryItems[0].itemName);
        Assert.AreEqual(2f, inventoryManager.inventoryItems[0].itemPrice);
        Assert.IsNull(inventoryManager.inventoryItems[0].itemImage);
    }

    [Test]
    public void AddItem_IncreasesTotalItems()
    {
        inventoryManager.AddItem("Apple", 2f, null);

        Assert.AreEqual(1, inventoryManager.totalItems);
    }

    [Test]
    public void AddItem_StoresMultipleItemsInCorrectOrder()
    {
        inventoryManager.AddItem("Apple", 2f, null);
        inventoryManager.AddItem("Milk", 3f, null);
        inventoryManager.AddItem("Bread", 4f, null);

        Assert.AreEqual(3, inventoryManager.inventoryItems.Count);
        Assert.AreEqual(3, inventoryManager.totalItems);

        Assert.AreEqual("Apple", inventoryManager.inventoryItems[0].itemName);
        Assert.AreEqual("Milk", inventoryManager.inventoryItems[1].itemName);
        Assert.AreEqual("Bread", inventoryManager.inventoryItems[2].itemName);
    }

    [Test]
    public void AddItem_AllowsDuplicateItems()
    {
        inventoryManager.AddItem("Apple", 2f, null);
        inventoryManager.AddItem("Apple", 2f, null);

        Assert.AreEqual(2, inventoryManager.inventoryItems.Count);
        Assert.AreEqual(2, inventoryManager.totalItems);
        Assert.AreEqual("Apple", inventoryManager.inventoryItems[0].itemName);
        Assert.AreEqual("Apple", inventoryManager.inventoryItems[1].itemName);
    }

    private static void ResetInventoryManagerInstance()
    {
        PropertyInfo instanceProperty = typeof(InventoryManager).GetProperty(
            "Instance",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
        );

        if (instanceProperty != null && instanceProperty.CanWrite)
        {
            instanceProperty.SetValue(null, null);
        }
    }
}
