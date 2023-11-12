using UnityEngine;

public class EquipmentsUI : MonoBehaviour
{
    [SerializeField] private Transform itemsParent; // A reference to the parent of all the slots

    private Inventory inventory;
    [SerializeField] private EquipmentSlots[] slots;


    // Start is called before the first frame update
    void Start()
    {
        inventory = Inventory.Instance; // An instance to the inventory

        // Triggering this event whenever an item got added or removed from the inventory
        inventory.onEquipmentChangedCallback += UpdateUI; // Subscribing with the function
        slots = itemsParent.GetComponentsInChildren<EquipmentSlots>();
    }

    /// <summary>
    /// This function is called when pressing the X button on the Equipments Tab,
    /// Removing the item from the equipments and putting it again on the Inventory.
    /// </summary>
    /// <param name="i"> The index of the removed requipemnt </param>
    public void RemoveFromEquipment(int i)
    {
        AudioManager.Instance.Play("RemoveEquipmentEffect");

        Inventory.Instance.Items.Add((Item)slots[i].Equipment);
        Inventory.Instance.onItemChangedCallback?.Invoke();

        Inventory.Instance.WeaponsInHand[i * 2].SetActive(false);
        Inventory.Instance.WeaponsInHand[i * 2].transform.SetParent(null);
        Inventory.Instance.WeaponsInHand[i * 2] = null;
        Inventory.Instance.WeaponsInHand[i * 2 + 1] = null;

        slots[i].ClearSlot(i);

        EquipmentManager.Instance.Unequip(i);
        Inventory.Instance.CurrentEquipments[i] = null;
    }


    /// <summary>
    /// Whenever something change on the Equipments Tab, this method will be called and modify it
    /// </summary>
    public void UpdateUI()
    {
        slots = itemsParent.GetComponentsInChildren<EquipmentSlots>();
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < Inventory.Instance.CurrentEquipments.Length) // there are more items to add
            {
                try
                {
                    slots[i].AddEquipment(inventory.CurrentEquipments[i]);

                }
                catch { }
            }
            else
            {
                slots[i].ClearSlot(i);
            }
        }
    }
}
