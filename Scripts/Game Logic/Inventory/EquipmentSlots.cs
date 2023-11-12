using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlots : MonoBehaviour
{
    [SerializeField] private Image icon;            // Slot icon
    [SerializeField] private Button removeButton;   // reference to the remove button
    [SerializeField] private Equipment equipment;   // Equipment (Scriptable object)
    [SerializeField] private string equipType;      // Type of the equipment
    [SerializeField] private new string name;       // Item equipment
    [SerializeField] private string content;        // Content of the equipment


    /// <summary>
    /// Add the feilds of the equipment to the inventory
    /// </summary>
    /// <param name="newItem"> The added item </param>
    public void AddEquipment(Equipment newItem)
    {
        equipment = newItem;
        icon.sprite = equipment.icon;
        icon.enabled = true;
        removeButton.interactable = true;

        name = equipment.name;
        content = equipment.content;
        equipType = equipment.equipType.ToString();
    }


    /// <summary>
    /// When removing or equipping item, clear it's slot
    /// </summary>
    /// <param name="newItem"> The removed or equipped item </param>
    public void ClearSlot(int i)
    {
        equipment = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;

        name = string.Empty;
        content = string.Empty;
        equipType = null;
    }


    /// <summary>
    /// Handles the behavior when removing an item from the inventory tab.
    /// </summary>
    public void OnRemoveButton()
    {
        Inventory.Instance.ActivateGameObjectWhenDroppingItem(equipment.name, false);
        Inventory.Instance.Remove(equipment);
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

    public Equipment Equipment
    {
        get { return equipment; }
        set { equipment = value; }
    }

    public string EquipType
    {
        get { return equipType; }
        set { equipType = value; }
    }

    public new string Name
    {
        get { return name; }
        set { name = value; }
    }

    public string Content
    {
        get { return content; }
        set { content = value; }
    }

}
