using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
   [SerializeField] private Image icon;             // Slot icon
   [SerializeField] private Button removeButton;    // reference to the remove button
   [SerializeField] private Item item;              // Item (Scriptable object)
   [SerializeField] private Transform itemsParent;  // The parent of the items
   [SerializeField] private Button itemButton;      // Reference to the item button
   [SerializeField] private string equipType;       // Type of the item
   [SerializeField] private new string name;        // Item name
   [SerializeField] private string content;         // Content of the item


    /// <summary>
    /// Add the feilds of the item to the inventory
    /// </summary>
    /// <param name="newItem"> The added item </param>
    public void AddItem(Item newItem)
    { // Adding an item to the inventory
        itemButton.enabled = true;
        itemButton.interactable = true;
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        removeButton.interactable = true;
        name = item.name;
        content = item.content;
        equipType = item.equipType.ToString();
    }


    /// <summary>
    /// When removing or equipping item, clear it's slot
    /// </summary>
    /// <param name="newItem"> The removed or equipped item </param>
    public void ClearSlot()
    {   
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
        itemButton.interactable = false;
        itemButton.enabled=false;
        name = string.Empty; 
        content = string.Empty;
        equipType = null;
    }


    /// <summary>
    /// Handles the behavior when removing an item from the inventory tab.
    /// </summary>
    public void OnRemoveButtonFromInv()
    {
        // Can't remove crystal from the inventroy
        string itemName = item.name;
        if (itemName == "Crystall" || itemName == "Key")
        {
            return;
        }

        Inventory.Instance.ActivateGameObjectWhenDroppingItem(item.name, false);
        Inventory.Instance.Remove(item);
        AudioManager.Instance.Play("RemoveEquipmentEffect");
    }


    /// <summary>
    /// Handles the behavior when removing an item from the equipment tab.
    /// </summary>
    public void OnRemoveButtonFromEquipment()
    {
        Inventory.Instance.ActivateGameObjectWhenDroppingItem(item.name, false);
        Inventory.Instance.Remove(item);
    }

    
    /// <summary>
    /// Use the item
    /// </summary>
    public void UseItem()
    {   
        if (item != null)
        {
            AudioManager.Instance.Play("EquipItemEffect");
            item.use();
        }
    }


    public Image Icon
    {
        get { return icon; }
        set { icon = value; }
    }

    public Button RemoveButton
    {
        get { return removeButton; }
        set { removeButton = value; }
    }

    public Item Item
    {
        get { return item; }
        set { item = value; }
    }

    public string EquipType
    {
        get { return equipType; }
        set { equipType = value; }
    }

    public string Content
    {
        get { return content; }
        set { content = value; }
    }
}

