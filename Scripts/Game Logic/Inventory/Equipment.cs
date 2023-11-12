using UnityEngine;

[CreateAssetMenu(fileName = " New Equipment", menuName ="Inventory/Equipment")]
[System.Serializable]

public class Equipment : Item
{
        #region Singelton
    // One inventory at all times
    public static Equipment instance;
    //public InventorySlot[] slots;
    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }
    #endregion

    public EquipmentSlot equipSlot; // reference to the enum equipments

    public int armorModifier;
    public int damageModifier;
    public bool isEquipped;

      public override void use()
      {// When the player use the item from the inventroy
            base.use();
            // Equip the item
            //Debug.Log("Should use");
            
            Inventory.Instance.PutItemOnPlayerHand(this);
            // Remove it from the inventory
            removeFromInventory();
            Inventory.Instance.Equip(this);
    }

    public bool HasDifferentEquipSlot(Equipment other)
    {
        return equipSlot != other.equipSlot;
    }
    public override string ToString()
    {
        return name +" "+ base.ToString();
    }

}

 public enum EquipmentSlot { Head, Pendant, Shoulder, Belt, Weapon, Shoe}

