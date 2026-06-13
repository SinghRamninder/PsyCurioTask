using UnityEngine;

public class GroceryItem : MonoBehaviour
{
    public string itemName;
    public float itemPrice = 0f;
    public Sprite itemImage;

    private void Start()
    {
        SetDefaultNameIfEmpty();
    }

    public void SetDefaultNameIfEmpty()
    {
        if (string.IsNullOrEmpty(itemName))
        {
            itemName = gameObject.name;
        }
    }
}