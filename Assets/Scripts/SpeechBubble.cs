using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

[Serializable]
public class UIitemSlots
{
    public Image itemImage;
    public TMP_Text itemName;
    public TMP_Text itemPrice;
}

public class SpeechBubble : MonoBehaviour
{
    public List<UIitemSlots> itemSlots = new List<UIitemSlots>();
    public TMP_Text totalPrice;
    [SerializeField] private float bubbleDisplayTime = 5f;

    private Camera mainCamera;
    private Animator animator;
    private Coroutine hideBubbleCoroutine;

    private void Start()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            Vector3 lookDirection = transform.position - mainCamera.transform.position;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(-lookDirection);
            }
        }
    }

    public void UpdateBubble(List<InventoryItems> inventoryItems)
    {
        if (hideBubbleCoroutine != null)
        {
            StopCoroutine(hideBubbleCoroutine);
        }

        hideBubbleCoroutine = StartCoroutine(hideBubble());

        Dictionary<string, (int count, float price, Sprite image)> groupedItems = new Dictionary<string, (int count, float price, Sprite image)>();

        foreach (var item in inventoryItems)
        {
            if (groupedItems.ContainsKey(item.itemName))
            {
                var val = groupedItems[item.itemName];
                val.count += 1;
                groupedItems[item.itemName] = val;
            }
            else
            {
                groupedItems[item.itemName] = (1, item.itemPrice, item.itemImage);
            }
        }

        // Reset all slots first
        foreach (var slot in itemSlots)
        {
            if (slot.itemName != null) slot.itemName.text = "";
            if (slot.itemPrice != null) slot.itemPrice.text = "";
            if (slot.itemImage != null)
            {
                slot.itemImage.sprite = null;
                slot.itemImage.enabled = false;
            }
        }

        int slotIndex = 0;
        float totalSum = 0f;

        foreach (var kvp in groupedItems)
        {
            if (slotIndex >= itemSlots.Count) break;

            var slot = itemSlots[slotIndex];
            string name = kvp.Key;
            int count = kvp.Value.count;
            float price = kvp.Value.price;
            Sprite img = kvp.Value.image;
            float sumPrice = price * count;
            totalSum += sumPrice;

            if (slot.itemName != null)
            {
                slot.itemName.text = $"{name}\n({count})";
            }
            if (slot.itemPrice != null)
            {
                slot.itemPrice.text = $"X {price}€ = {sumPrice}€";
            }
            if (slot.itemImage != null)
            {
                slot.itemImage.sprite = img;
                slot.itemImage.enabled = true;
            }

            slotIndex++;
        }

        if (totalPrice != null)
        {
            totalPrice.text = $"Total = {totalSum}€";
        }
    }

    private IEnumerator hideBubble()
    {
        yield return new WaitForSeconds(bubbleDisplayTime);

        animator.Play("BubbleDisappear");
        animator.Update(0f);

        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            yield return new WaitForSeconds(clipInfo[0].clip.length);
        }

        gameObject.SetActive(false);
    }
}