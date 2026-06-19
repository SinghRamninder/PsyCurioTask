using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class SpeechBubbleTests
{
    private GameObject canvasObject;
    private GameObject bubbleObject;
    private SpeechBubble speechBubble;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        canvasObject = new GameObject("TestCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        bubbleObject = new GameObject("SpeechBubble_TestObject");
        bubbleObject.transform.SetParent(canvasObject.transform);
        bubbleObject.AddComponent<Animator>();
        speechBubble = bubbleObject.AddComponent<SpeechBubble>();

        speechBubble.itemSlots = new List<UIitemSlots>();

        for (int i = 0; i < 5; i++)
        {
            speechBubble.itemSlots.Add(CreateUISlot(canvasObject.transform, i));
        }

        GameObject totalPriceObject = new GameObject("TotalPriceText");
        totalPriceObject.transform.SetParent(canvasObject.transform);
        speechBubble.totalPrice = totalPriceObject.AddComponent<TextMeshProUGUI>();

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (bubbleObject != null)
        {
            Object.Destroy(bubbleObject);
        }

        if (canvasObject != null)
        {
            Object.Destroy(canvasObject);
        }

        yield return null;
    }

    [UnityTest]
    public IEnumerator UpdateBubble_GroupsDuplicateItems()
    {
        Dictionary<int, InventoryItems> items = new Dictionary<int, InventoryItems>
        {
            {0, new InventoryItems("Apple", 2f, null)},
            {1, new InventoryItems("Apple", 2f, null)},
            {2, new InventoryItems("Milk", 3f, null)}
        };

        speechBubble.UpdateBubble(items);
        yield return null;

        UIitemSlots appleSlot = FindSlotByItemName("Apple");

        Assert.IsNotNull(appleSlot);
        Assert.AreEqual("Apple\n(2)", appleSlot.itemName.text);
        Assert.AreEqual("X 2€ = 4€", appleSlot.itemPrice.text);
    }

    [UnityTest]
    public IEnumerator UpdateBubble_CalculatesCorrectTotalPrice()
    {
        Dictionary<int, InventoryItems> items = new Dictionary<int, InventoryItems>
        {
            {0, new InventoryItems("Apple", 2f, null)},
            {1, new InventoryItems("Milk", 3f, null)},
            {2, new InventoryItems("Bread", 4f, null)}
        };

        speechBubble.UpdateBubble(items);
        yield return null;

        Assert.AreEqual("Total = 9€", speechBubble.totalPrice.text);
    }

    [UnityTest]
    public IEnumerator UpdateBubble_ClearsUnusedSlots()
    {
        foreach (UIitemSlots slot in speechBubble.itemSlots)
        {
            slot.itemName.text = "Old Item";
            slot.itemPrice.text = "Old Price";
            slot.itemImage.enabled = true;
        }

        Dictionary<int, InventoryItems> items = new Dictionary<int, InventoryItems>
        {
            {0, new InventoryItems("Apple", 2f, null)}
        };

        speechBubble.UpdateBubble(items);
        yield return null;

        for (int i = 1; i < speechBubble.itemSlots.Count; i++)
        {
            Assert.AreEqual("", speechBubble.itemSlots[i].itemName.text);
            Assert.AreEqual("", speechBubble.itemSlots[i].itemPrice.text);
            Assert.IsFalse(speechBubble.itemSlots[i].itemImage.enabled);
            Assert.IsNull(speechBubble.itemSlots[i].itemImage.sprite);
        }
    }

    [UnityTest]
    public IEnumerator UpdateBubble_EnablesImageForUsedSlot()
    {
        Texture2D texture = new Texture2D(16, 16);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f));

        Dictionary<int, InventoryItems> items = new Dictionary<int, InventoryItems>
        {
            {0, new InventoryItems("Apple", 2f, sprite)}
        };

        speechBubble.UpdateBubble(items);
        yield return null;

        UIitemSlots appleSlot = FindSlotByItemName("Apple");

        Assert.IsNotNull(appleSlot);
        Assert.AreEqual(sprite, appleSlot.itemImage.sprite);
        Assert.IsTrue(appleSlot.itemImage.enabled);

        Object.Destroy(sprite);
        Object.Destroy(texture);
    }

    private UIitemSlots CreateUISlot(Transform parent, int index)
    {
        GameObject nameObject = new GameObject($"ItemName_{index}");
        nameObject.transform.SetParent(parent);
        TMP_Text itemNameText = nameObject.AddComponent<TextMeshProUGUI>();

        GameObject priceObject = new GameObject($"ItemPrice_{index}");
        priceObject.transform.SetParent(parent);
        TMP_Text itemPriceText = priceObject.AddComponent<TextMeshProUGUI>();

        GameObject imageObject = new GameObject($"ItemImage_{index}");
        imageObject.transform.SetParent(parent);
        Image itemImage = imageObject.AddComponent<Image>();

        return new UIitemSlots
        {
            itemName = itemNameText,
            itemPrice = itemPriceText,
            itemImage = itemImage
        };
    }

    private UIitemSlots FindSlotByItemName(string itemName)
    {
        foreach (UIitemSlots slot in speechBubble.itemSlots)
        {
            if (slot.itemName != null && slot.itemName.text.StartsWith(itemName))
            {
                return slot;
            }
        }

        return null;
    }
}
