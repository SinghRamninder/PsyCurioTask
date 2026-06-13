using NUnit.Framework;
using UnityEngine;

public class GroceryItemTests
{
    [Test]
    public void SetDefaultNameIfEmpty_WhenItemNameIsEmpty_UsesGameObjectName()
    {
        GameObject itemObject = new GameObject("Apple");
        GroceryItem groceryItem = itemObject.AddComponent<GroceryItem>();

        groceryItem.itemName = "";

        groceryItem.SetDefaultNameIfEmpty();

        Assert.AreEqual("Apple", groceryItem.itemName);

        Object.DestroyImmediate(itemObject);
    }

    [Test]
    public void SetDefaultNameIfEmpty_WhenItemNameAlreadyExists_KeepsExistingName()
    {
        GameObject itemObject = new GameObject("AppleObject");
        GroceryItem groceryItem = itemObject.AddComponent<GroceryItem>();

        groceryItem.itemName = "Apple";

        groceryItem.SetDefaultNameIfEmpty();

        Assert.AreEqual("Apple", groceryItem.itemName);

        Object.DestroyImmediate(itemObject);
    }

    [Test]
    public void GroceryItem_StoresPriceCorrectly()
    {
        GameObject itemObject = new GameObject("Apple");
        GroceryItem groceryItem = itemObject.AddComponent<GroceryItem>();

        groceryItem.itemPrice = 2.5f;

        Assert.AreEqual(2.5f, groceryItem.itemPrice);

        Object.DestroyImmediate(itemObject);
    }
}