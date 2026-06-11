using UnityEngine;

public class GroceryItem : MonoBehaviour
{
    public string itemName;
    public float itemPrice = 0f;
    public Sprite itemImage;

    private void Start()
    {
        if (itemName == "")
        {
            itemName = gameObject.name;
        }
    }
}
