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
    [HideInInspector] public List<InventoryItems> inventoryItems = new List<InventoryItems>();
    [HideInInspector] public int totalItems = 0;

    [SerializeField] private GameObject speechBubble;

    public void AddItem(string name, float price, Sprite image)
    {
        inventoryItems.Add(new InventoryItems(name, price, image));
        totalItems++;
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
