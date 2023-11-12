using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform itemsParent; // A reference to the parent of all the slots

    private Inventory inventory;
    private InventorySlot[] slots;

    void Start()
    {
        // An instance to the inventory
        inventory = Inventory.Instance;

        // Triggering this event whenever an item got added or removed from the inventory
        inventory.onItemChangedCallback += UpdateUI; // Subscribing with the function
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }


    /// <summary>
    /// Whenever something change on the Invnetory Tab, this method will be called and modify it
    /// </summary>
    public void UpdateUI()
    {
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
        for (int i=0; i<slots.Length; i++)
        {

            if(i < Inventory.Instance.Items.Count) // there are more items to add
            {
                try
                {
                    slots[i].AddItem(inventory.Items[i]);
                }
                catch { }
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
