using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f;

    private Animator characterAnimator;

    private void Update()
    {
        if (InputManager.Instance == null || InventoryManager.Instance == null)
        {
            Debug.LogWarning("InputManager or InventoryManager is missing");
            return;
        }

        if (InputManager.Instance.controls.Player.Interact.WasPressedThisFrame())
        {
            TryInteraction();
        }
    }

    private void TryInteraction()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, interactRange))
        {
            if (hit.collider.CompareTag("Character"))
            {
                if (characterAnimator == null)
                {
                    characterAnimator = hit.collider.GetComponent<Animator>();
                }

                characterAnimator.Play("Waving");
            }
            else if (hit.collider.CompareTag("CashRegister"))
            {
                InventoryManager.Instance.DisplayInventory();
            }
            else
            {
                GroceryItem item = hit.collider.GetComponent<GroceryItem>();

                if (item != null)
                {
                    if (InventoryManager.Instance.totalItems < InventoryManager.Instance.counterSlots.Length)
                    {
                        var invManager = InventoryManager.Instance;
                        var targetSlot = invManager.counterSlots[invManager.totalItems];
                        GameObject spawnedItem = Instantiate(item.gameObject, targetSlot.position, Quaternion.identity);
                        spawnedItem.GetComponent<Collider>().enabled = false;
                        spawnedItem.GetComponent<GroceryItem>().enabled = false;

                        invManager.AddItem(item.itemName, item.itemPrice, item.itemImage);
                    }
                    else
                    {
                        Debug.Log("More than required items");
                    }
                }
            }
        }
    }
}
