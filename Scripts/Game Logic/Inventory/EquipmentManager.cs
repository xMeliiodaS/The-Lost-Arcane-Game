using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    // instance to the EquipmentManager
    private static EquipmentManager instance;
    void Awake() 
    {
       instance = this;
    }

    public static EquipmentManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EquipmentManager();
            }
            return instance;
        }
    }

    [SerializeField] private List<GameObject> armorsInEquipment;    // List of the armor on equipments


    public delegate void OnEquipmentChanged(Equipment newItem,Equipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    private Inventory inventory;

    private void Start() 
    {
        inventory = Inventory.Instance;
    }


    /// <summary>
    /// Unequip item
    /// </summary>
    /// <param name="slotIndex"> an item with a particular index </param>
    public void Unequip(int slotIndex)
    {
        if (inventory.CurrentEquipments[slotIndex] != null) // If we have an item equipped
        {
            // Get the item being unequipped
            Equipment oldItem = inventory.CurrentEquipments[slotIndex];

            // Remove the item from the equipment slot
            inventory.CurrentEquipments[slotIndex] = null;

            // Notify listeners about the equipment change
            onEquipmentChanged?.Invoke(null, oldItem);

            // Remove the equipment from the player's inventory
            inventory.RemoveEquipment();

            var equippedWeapon = FindPlayer.Instance.Player.GetComponent<WeaponManager>().Weapon;
            try
            {
                if (equippedWeapon.name == oldItem.name)
                {
                    FindPlayer.Instance.Player.GetComponent<WeaponManager>().Weapon = null;
                }
            }
            catch { }
        }
    }
}
