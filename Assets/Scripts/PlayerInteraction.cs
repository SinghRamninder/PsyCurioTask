using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private float itemAnimationDuration = 0.5f;
    [SerializeField] private GameObject noticeContainer;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip errorSound;

    private TMP_Text noticeText;
    private Animator characterAnimator;
    private Coroutine displayNoticeCoroutine;

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
        int layerIndex = LayerMask.NameToLayer("Checkout");
        int mask = layerIndex != -1 ? ~(1 << layerIndex) : Physics.DefaultRaycastLayers;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, interactRange, mask))
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
                if (InventoryManager.Instance.totalItems == 0)
                {
                    if (displayNoticeCoroutine != null)
                    {
                        StopCoroutine(displayNoticeCoroutine);
                    }

                    displayNoticeCoroutine = StartCoroutine(DisplayNotice("Please select at least 1 grocery item"));
                }
                else
                {
                    InventoryManager.Instance.DisplayInventory();
                }
            }
            else
            {
                GroceryItem item = hit.collider.GetComponent<GroceryItem>();

                if (item != null)
                {
                    if (InventoryManager.Instance.totalItems < InventoryManager.Instance.counterSlots.Length)
                    {
                        StartCoroutine(AudioManager.Instance.PlaySound(clickSound));
                        var invManager = InventoryManager.Instance;
                        var targetSlot = invManager.counterSlots[invManager.totalItems];
                        GameObject spawnedItem = Instantiate(item.gameObject, item.transform.position, Quaternion.identity);
                        spawnedItem.GetComponent<Collider>().enabled = false;
                        spawnedItem.GetComponent<GroceryItem>().enabled = false;

                        StartCoroutine(MoveItemSmoothly(spawnedItem.transform, targetSlot.position, itemAnimationDuration));

                        invManager.AddItem(item.itemName, item.itemPrice, item.itemImage);
                    }
                    else
                    {
                        StartCoroutine(AudioManager.Instance.PlaySound(errorSound));

                        if (displayNoticeCoroutine != null)
                        {
                            StopCoroutine(displayNoticeCoroutine);
                        }

                        displayNoticeCoroutine = StartCoroutine(DisplayNotice($"You cannot pick more than {InventoryManager.Instance.counterSlots.Length} items"));
                }
                }
            }
        }
    }

    private IEnumerator MoveItemSmoothly(Transform itemTransform, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = itemTransform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (itemTransform == null) yield break;

            itemTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (itemTransform != null)
        {
            itemTransform.position = targetPosition;
        }
    }

    private IEnumerator DisplayNotice(string notice)
    {
        if (noticeText == null)
        {
            noticeText = noticeContainer.GetComponentInChildren<TMP_Text>();
        }
        
        noticeText.text = string.Empty;
        noticeText.text = notice;

        noticeContainer.SetActive(true);
        
        yield return new WaitForSeconds(2f);

        noticeContainer.SetActive(false);
    }
}
