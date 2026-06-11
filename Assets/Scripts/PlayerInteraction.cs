using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange;

    private void Update()
    {
        InputManager inputManager = InputManager.Instance;
        InventoryManager inventoryManager = InventoryManager.Instance;

        if (inputManager == null || inventoryManager == null)
        {
            Debug.LogWarning("InputManager or InventoryManager is missing");
            return;
        }

        if (inputManager.controls.Player.Interact.WasPressedThisFrame())
        {
            if (inventoryManager.totalItems < inventoryManager.counterSlots.Length)
            {
                TryItemPickup(inventoryManager);
            }
            else
            {
                Debug.Log("More than required items");
            }
        }
    }

    private void TryItemPickup(InventoryManager invManager)
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit))
        {
            GroceryItem item = hit.collider.GetComponent<GroceryItem>();

            if (item != null)
            {
                var targetSlot = invManager.counterSlots[invManager.totalItems];
                GameObject spawnedItem = Instantiate(item.gameObject, targetSlot.position, Quaternion.identity);
                spawnedItem.GetComponent<Collider>().enabled = false;
                spawnedItem.GetComponent<GroceryItem>().enabled = false;

                invManager.AddItem(item.itemName, item.itemPrice, item.itemImage);
            }
        }
    }
}
