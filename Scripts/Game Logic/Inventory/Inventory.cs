using UnityEngine;
using StarterAssets;
using System.Collections.Generic;

[System.Serializable]

public class Inventory : MonoBehaviour
{
    // Making a reference to the inventory
    #region Singelton

    // One inventory at all times
    private static Inventory instance;

    public static Inventory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Inventory();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }
    #endregion

    // list of the current items/equipemtns
    [SerializeField] private List<GameObject> itemsOnInventory = new();

    [SerializeField] private WeaponManager[] weaponManager;     // Player's WeaponManager Component
    [SerializeField] private GameObject[] weaponsInHand;        // Current/Switched Weapons in hand

    [SerializeField] private Equipment[] currentEquipments; // array of the current equipemnts

    private Equipment oldItem = null;           // Switched item

    [SerializeField] private int maxSpace = 26; // Max space on te inventory

    private bool isEquipped = false;            // Is the item equipped
    private GameObject oldSwitchedItem;         // The old switched item
    private int tempPlaceType = 0;

    private List<Item> items = new();           // List of items
    private Transform activePlayerTransform;    // field to store the transform of the active player


    // Delegate function---------------------------------------------------
    // this function will be called When something change on the inventory tab
    public onItemChanged onItemChangedCallback;
    public delegate void onItemChanged();

    // this function will be called When something change on the Equipments tab
    public onEquipmentChanged onEquipmentChangedCallback;
    public delegate void onEquipmentChanged();


    private void Start()
    {
        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length; // the length of the array
        currentEquipments = new Equipment[numSlots]; // the current equipment
        weaponsInHand = new GameObject[12];
    }


    /// <summary>
    /// Add equipment to the Equipments tab and modify the damage and armor values
    /// </summary>
    /// <param name="newItem"> item that is being equipped </param>
    public void Equip(Equipment newItem)
    {
        // curent Equipment index
        int slotIndex = (int)newItem.equipSlot;

        // if equipemnt is already there
        if (currentEquipments[slotIndex] != null)
        {
            oldItem = currentEquipments[slotIndex];
            Add(oldItem);
        }

        if (EquipmentManager.Instance.onEquipmentChanged != null)
        {
            // Old item can't be null
            oldItem = currentEquipments[slotIndex];

            EquipmentManager.Instance.onEquipmentChanged.Invoke(newItem, oldItem);
        }

        currentEquipments[slotIndex] = newItem;

        FindPlayer.Instance.Player.GetComponent<WeaponManager>().Weapon = weaponsInHand[8];

        onEquipmentChangedCallback?.Invoke();
    }


    /// <summary>
    /// Remove equipment from the Equipments tab and modify the damage and armor values
    /// </summary>
    public void RemoveEquipment()
    {
        onEquipmentChangedCallback?.Invoke();
    }


    /// <summary>
    /// // Add a gameObject to the list when we pick up an item
    /// </summary>
    /// <param name="item"> item that is being picked up </param>
    public void AddItem(GameObject item)
    {
        itemsOnInventory.Add(item);
    }


    /// <summary>
    /// Remove item from the inventory
    /// </summary>
    /// <param name="item"> item that is being removed </param>
    public void RemoveItem(GameObject item)
    {
        itemsOnInventory.Remove(item);
    }


    /// <summary>
    /// adds items to the item's list and checks if enough space
    /// </summary>
    /// <param name="item"> item is a scriptableobject that is being added to the items list </param>
    /// <returns></returns>
    public bool Add(Item item)
    { 
        if (!item.isDefaultItem) // prevent adding default items
        {
            if (items.Count >= maxSpace) // if there is no more space on the inventory
            {
                return false;
            }

            items.Add(item); // if there still space on the inventory

            onItemChangedCallback?.Invoke();
        }
        return true;
    }


    /// <summary>
    /// Activate the threw game object and put it on the ground
    /// </summary>
    /// <param name="itemName"> The name of the item that is being dropped </param>
    public void ActivateGameObjectWhenDroppingItem(string itemName, bool destroyDroppedItem)
    {
        activePlayerTransform = FindPlayer.Instance.Player.transform/*FindObjectOfType<ThirdPersonController>().transform*/;
        
        if (itemsOnInventory == null) // if the item is null dont do anything
        {
            return;
        }

        for (int i = 0; i < itemsOnInventory.Count; i++)
        {
            if (itemsOnInventory[i].name == itemName) // If the item name is exactly like the gameObject name
            {
                itemsOnInventory[i].transform.SetParent(null);

                if (destroyDroppedItem)
                {
                    Destroy(itemsOnInventory[i]);
                }
                else
                {
                    itemsOnInventory[i].SetActive(true); // Set the gameObject activation to true    
                }
        
                try
                {
                    itemsOnInventory[i].transform.GetComponent<SphereCollider>().enabled = true;
                    itemsOnInventory[i].transform.GetChild(0).gameObject.SetActive(true);   // Activate the vfx effect
                }
                catch { }

                UpdateWeaponManagers(itemsOnInventory[i]);

                // Make the item appear on front player
                if (IsMageWeapon(itemsOnInventory[i]))
                {
                    itemsOnInventory[i].transform.localScale = new Vector3(7f, 7f, 7f);
                }
                
                itemsOnInventory[i].transform.position = activePlayerTransform.position + new Vector3(0f, 0.5f, 0f) + activePlayerTransform.forward * 2f;

                // Remove the item from the inventory
                itemsOnInventory.RemoveAt(i); 

                break; // Stop looping through the list once the item has been found and activated
            }
        }
    }


    /// <summary>
    /// update the weapon managers with the dropped item
    /// </summary>
    /// <param name="item"> The item that is being dropped </param>
    private void UpdateWeaponManagers(GameObject item)
    {
        foreach (WeaponManager wm in weaponManager)
        {
            try
            {
/*                wm.Weapon = item;
*/                var makeWeaponPickDisabled = wm.Weapon.GetComponent<ItemPickUp>();
                makeWeaponPickDisabled.enabled = true;
            }
            catch { }
        }
    }


    /// <summary>
    /// Function that equip the item on the player body
    /// </summary>
    /// <param name="item"> The equipment that is being equipped </param>
    // 
    public void PutItemOnPlayerHand(Equipment item)
    {   
        activePlayerTransform = FindPlayer.Instance.Player.transform; // reference to the player that has the ThirdPersonController Sscript
        int currentItemIndex = (int)item.equipSlot * 2;
        
        // Activate the game object
        for (int i = 0; i < itemsOnInventory.Count; i++) // For for all the items on the inventory
        {
            if (itemsOnInventory[i].name == item.name) // If the item name is exactly like the gameObject name
            {
                DisableOldItemEnableCurrent(i, currentItemIndex);
                IfOldItem(currentItemIndex);
                AddArmorToPlayer(currentItemIndex, (Equipment)item);
                break; // Stop looping through the list once the item has been found and equipped
            }
        }

        // Handle deactivating old item if any
        if (currentItemIndex < weaponsInHand.Length - 1 && weaponsInHand[currentItemIndex + 1] != null &&
                weaponsInHand[currentItemIndex + 1] != null &&
                itemsOnInventory.Contains(weaponsInHand[currentItemIndex + 1]))
        {
            weaponsInHand[currentItemIndex + 1].SetActive(false);
        }

    }



    /// <summary>
    /// Enable new item and make sure old item is disabled
    /// </summary>
    /// <param name="i"> The index of the equipment </param>
    /// <param name="currentItemIndex"> The index of the current equipment (New equipment) </param>
    public void DisableOldItemEnableCurrent(int i, int currentItemIndex)
    {
        weaponsInHand[currentItemIndex] = itemsOnInventory[i];
        itemsOnInventory[i].SetActive(true); // Set the gameObject to active

        try 
        { 
            itemsOnInventory[i].transform.GetComponent<SphereCollider>().enabled = false;
            itemsOnInventory[i].transform.GetChild(0).gameObject.SetActive(false); // Deactivate the vfx effect
        }   
        catch 
        {
            Debug.LogWarning("No child was found (no VFX was found)");
        }

        try
        {
            weaponsInHand[currentItemIndex].GetComponent<ItemPickUp>().enabled = false; // Pressing E won't pick up the item
        }
        catch {}
        
        // If the item is equipped 
        if (isEquipped && tempPlaceType == currentItemIndex)
        {
            // We put the last item we had before the new on the next index
            weaponsInHand[currentItemIndex + 1] = oldSwitchedItem;
            
            try
            {
                weaponsInHand[currentItemIndex + 1].GetComponent<ItemPickUp>().enabled = true; // Pressing E will pick up the item
            }
            catch { }
            
            // Check if the new item is not the same as the old item
            if (weaponsInHand[currentItemIndex] != weaponsInHand[currentItemIndex + 1])
            {
                weaponsInHand[currentItemIndex+1].SetActive(false);
            }
        }
    }


    /// <summary>
    /// Disabling the old item
    /// </summary>
    /// <param name="currentItemIndex"> The current item + 1 ==> is the old item that will be disabled </param>
    public void IfOldItem(int currentItemIndex)
    {
        // If the item is equipped (Just not for the first time)
        if (isEquipped)
        {
            try
            {
                if (weaponsInHand[currentItemIndex + 1] != weaponsInHand[currentItemIndex])
                {
                    weaponsInHand[tempPlaceType + 1] = oldSwitchedItem;                // Get the last used item before the new item
                    weaponsInHand[currentItemIndex + 1].SetActive(false);   // Deactivate it
                }
            }
            catch
            {
                Debug.LogWarning("No old item was found");
            }
        }

        isEquipped = true;
        oldSwitchedItem = weaponsInHand[currentItemIndex];    // Save the last item before the new iten on variable
        tempPlaceType = currentItemIndex;          // Save the index on variable
    }

    
    /// <summary>
    /// Equip the armor to the player correct position
    /// </summary>
    /// <param name="currentItemIndex"> The index of the current item </param>
    /// <param name="item"> The type of the equipment </param>
    public void AddArmorToPlayer(int currentItemIndex, Equipment item)
    {
        Transform armor = weaponsInHand[currentItemIndex].transform;

        switch (item.equipSlot)
        {
            case EquipmentSlot.Shoulder:
                AttachArmorToSlot(armor, "Shoulder");
                break;

            case EquipmentSlot.Weapon:
                AttachArmorToSlot(armor, "Weapon");
                break;

            case EquipmentSlot.Head:
                AttachArmorToSlot(armor, "Head");
                break;
        }

        EquipAndDisableWeapon(currentItemIndex);
    }

    /// <summary>
    /// Assign the weapon for the Weapon Manager class in order to make damage with the equipped weapon
    /// </summary>
    /// <param name="currentItemIndex"> The current index of the weapon </param>
    private void EquipAndDisableWeapon(int currentItemIndex)
    {
        foreach (WeaponManager currentEquippedWeapon in weaponManager)
        {
            try
            {
                //currentEquippedWeapon.Weapon = weaponsInHand[currentItemIndex];
                DisableItemPickup(currentEquippedWeapon.Weapon);
            }
            catch { }
        }

        try
        {
            if (weaponsInHand[currentItemIndex] == weaponsInHand[currentItemIndex + 1])
            {
                weaponsInHand[currentItemIndex + 1] = null;
            }
        }
        catch { }
    }

    /// <summary>
    /// Check if the weapon is for Mage or not
    /// </summary>
    /// <param name="weapon"></param>
    /// <returns> Return true if the weapon is for mage, else false </returns>
    private bool IsMageWeapon(GameObject weapon)
    {
        return weapon.GetComponent<ItemPickUp>().IsMageWeapon;
    }

    /// <summary>
    /// attach armor to the specified slot
    /// </summary>
    /// <param name="armor"> Get the position and rotation to fit perfectly on the player </param>
    /// <param name="slotTag"> Find the slot Tag on the player's mesh </param>
    private void AttachArmorToSlot(Transform armor, string slotTag)
    {
        GameObject targetSlot = GameObject.FindGameObjectWithTag(slotTag);
        armor.SetParent(targetSlot.transform);
        
        try
        {
            ItemPickUp itemPickUp = armor.GetComponent<ItemPickUp>();
            armor.SetLocalPositionAndRotation(new Vector3(itemPickUp.PX, itemPickUp.PY, itemPickUp.PZ), Quaternion.Euler(itemPickUp.PR1, itemPickUp.PR2, itemPickUp.PR3));
        
            if(IsMageWeapon(armor.gameObject))
            {
                armor.localScale = Vector3.zero;
            }
        }
        catch { }
    }


    /// <summary>
    /// disable ItemPickUp component for a given GameObject
    /// </summary>
    /// <param name="obj"> The given gameobject </param>
    private void DisableItemPickup(GameObject obj)
    {
        try
        {
            var itemPickUp = obj.GetComponent<ItemPickUp>();
            itemPickUp.enabled = false;
        }
        catch { }
    }

    /// <summary>
    /// Remove the item from the inventory
    /// </summary>
    /// <param name="item"> The removed item </param>
    public void Remove(Item item)
    {     
        items.Remove(item);
     
        onItemChangedCallback?.Invoke();
    }


    // Getters and Setters
    // Properties for the private fields
    public List<GameObject> ItemsOnInventory
    {
        get { return itemsOnInventory; }
        set { itemsOnInventory = value; }
    }

    public WeaponManager[] WeaponManagers
    {
        get { return weaponManager; }
        set { weaponManager = value; }
    }

    public GameObject[] WeaponsInHand
    {
        get { return weaponsInHand; }
        set { weaponsInHand = value; }
    }

    public Equipment[] CurrentEquipments
    {
        get { return currentEquipments; }
        set { currentEquipments = value; }
    }

    public Equipment OldItem
    {
        get { return oldItem; }
        set { oldItem = value; }
    }

    public int MaxSpace
    {
        get { return maxSpace; }
        set { maxSpace = value; }
    }

    public bool IsEquipped
    {
        get { return isEquipped; }
        set { isEquipped = value; }
    }

    public List<Item> Items
    {
        get { return items; }
        set { items = value; }
    }

    public void SetWeaponsInHand(GameObject item, int i)
    {
        this.weaponsInHand[i] = item;
    }
}
