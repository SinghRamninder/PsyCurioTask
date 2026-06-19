using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InventoryItems
{
    public string itemName;
    public float itemPrice;
    public Sprite itemImage;

    public InventoryItems(string name, float price, Sprite image)
    {
        itemName = name;
        itemPrice = price;
        itemImage = image;
    }
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance {  get; private set; }

    public Transform[] counterSlots;
    [HideInInspector] public Dictionary<int, InventoryItems> inventoryItems = new Dictionary<int, InventoryItems>();

    [SerializeField] private GameObject speechBubble;

    public void AddItem(int slotNumber, string name, float price, Sprite image)
    {
        inventoryItems.Add(slotNumber, new InventoryItems(name, price, image));
    }

    public void RemoveItem(int slotNumber)
    {
        inventoryItems.Remove(slotNumber);
    }

    public int GetSlotIndex(Transform slotTransform)
    {
        for (int i = 0; i < counterSlots.Length; i++)
        {
            if (counterSlots[i] == slotTransform)
            {
                return i;
            }
        }
        return -1;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void DisplayInventory()
    {
        if (speechBubble != null)
        {
            speechBubble.SetActive(true);
            SpeechBubble bubble = speechBubble.GetComponent<SpeechBubble>();
            if (bubble != null)
            {
                bubble.UpdateBubble(inventoryItems);
            }
        }
    }
}
