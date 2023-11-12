using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DatabaseSystem : MonoBehaviour
{
    #region Singelton
    // One inventory at all times
    public static DatabaseSystem instance;
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
    [Header("Inventory and equipments Ref")]
    [SerializeField] private Transform invnetoryItemsParent;    // Reference to the parent transform that contains the InventorySlot objects
    [SerializeField] private Transform equipementsItemsParent;  // Reference to the parent transform that contains the InventorySlot objects

    private static string characterName;                // The player name
    [HideInInspector] private int enteriesTimee = 0;    // Temp

    [Header("Inventory and equipments class Ref")]
    [SerializeField] private InventoryUI inventoryUI;           // Reference to the InventoryUI's class
    [SerializeField] private EquipmentsUI equipmentsUI;         // Reference to the EquipmentsUI's class

    [Header("References to the skills, images")]
    [SerializeField] private GameObject parentOfSkills;         // Parent of object containing all the skills
    [SerializeField] private GameObject parentOfInventorySlots; // Parent of object containing all inventory slots
    [SerializeField] private GameObject imagesParent;           // Parent of object containing all the skill's and abilities images
    [SerializeField] private TextMeshProUGUI potionQuantity;    // Reference to the Potion quantity Text
    [SerializeField] private GameObject portal;                 // Reference to the portal game object
    [SerializeField] private Transform parentOfSkillsTab;       // Parent of all the Skill's tab

    private DialogueManager dialogueManager;     // Reference to the dialogue manager

    [SerializeField] private LastSceneSaveLoad lastSceneCutscene;

    // Paths to save and load ALL THE DATA
    private readonly string skillSlotPath = "_SkillSlot_";
    private readonly string itemsOnInventoryPath = "_SaveItemsOnInv_";
    private readonly string weaponsInHandsPath = "_SaveWeaponsInHands_";
    private readonly string currentEquipmentsPath = "_currentEquipments_";
    private readonly string currentItemsPath = "_currentItems_";
    private readonly string inventorySlotsPath = "_inventorySlots_";
    private readonly string equipmentSlotsPath = "_equipmentSlots_";
    private readonly string playerStatsPath = "_CharcaterStats_";
    private readonly string iconOnInvPath = "_iconOnInv_";
    private readonly string iconOnEqPath = "_iconOnEq_";

    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private string playerName;

    private void Start()
    {
        playerName = PlayerPrefs.GetString("PlayerName");
        Transform player = FindPlayer.Instance.Player.transform;
        player.GetComponent<PlayerStat>().PlayerName = playerName;

        dialogueManager = GetComponent<DialogueManager>();
    }



    public void Save()
    {
        characterName = playerName;
        string savePathCurrentEq = Application.persistentDataPath + characterName;

        // Save the state of all the quest givers on the scene
        try
        {
            dialogueManager.SaveQuestGivers();
        } 
        catch { }

        //-------------------SaveSkills-------------------
        // Save the state of the portal (Closed/Opened)
        try
        {
            SavePortalState(characterName);
        }
        catch { }


        //-------------------SaveSkills-------------------
        // The the Assigned skills to the hotkeys
        try
        {
            savePathCurrentEq += skillSlotPath;
            GetComponent<ParentObject>().SaveChildData(savePathCurrentEq, imagesParent);
        }
        catch { }

        savePathCurrentEq = Application.persistentDataPath + characterName;

        //-------------------SaveHealingPotionsQuantity-------------------
        // Save the healing potions quabtity
        try
        {
            savePathCurrentEq += skillSlotPath;
            int quantity = int.Parse(potionQuantity.text);
            SaveItemsInInv.SavePotionsQuantity(savePathCurrentEq, quantity);
        }
        // Some scenes does not have the Quantity
        catch { }

        savePathCurrentEq = Application.persistentDataPath + characterName;

        //-------------------SaveItemsOnInventory-------------------
        // Save the items on the inventory (Gameobject)
        try
        {
            savePathCurrentEq += itemsOnInventoryPath;
            SaveItemsInInv.SaveItemInInventory(Inventory.Instance.ItemsOnInventory, savePathCurrentEq);
        }
        catch { }

        savePathCurrentEq = Application.persistentDataPath + characterName;

        //-------------------SaveWeaponsInHand-------------------
        // Svae the weapons the player currently wearing (Gameobject)
        try
        {
            savePathCurrentEq += weaponsInHandsPath;
            WeaponsInHandData.SaveWeaponsInHand(Inventory.Instance.WeaponsInHand, savePathCurrentEq);
        }
        catch { }

        savePathCurrentEq = Application.persistentDataPath + characterName;

        //-------------------SaveCurrentEquipments-------------------
        // Save the current Equipments (Scriptable Object)
        try
        {
            savePathCurrentEq += currentEquipmentsPath; // Specify the save file path
            SavingCurrentEquioments.SaveEquipmentData(Inventory.Instance.CurrentEquipments, savePathCurrentEq);
        }
        catch { }

        savePathCurrentEq = Application.persistentDataPath + characterName;

        //-------------------SaveCurrentItems-------------------
        // Save the current Items (Scriptable Object)
        try
        {
            savePathCurrentEq += currentItemsPath; // Specify the save file path
            List<Item> currentItems = Inventory.Instance.Items;
            List<Equipment> currentEquipment = new(currentItems.Count);
            foreach (Item item in currentItems)
            {
                // If the item is equipment
                if (item is Equipment)
                {
                    currentEquipment.Add((Equipment)item);
                }
            }

            SavingCurrentItemsSO.SaveEquipmentData(currentEquipment, savePathCurrentEq);
        }
        catch { }

        savePathCurrentEq = Application.persistentDataPath + characterName;

        //-------------------SaveInventorySlots-------------------
        // Svae the All the inventory slots
        InventorySlot[] inventorySlots = null;
        try
        {
            savePathCurrentEq += inventorySlotsPath; // Specify the save file path
            inventorySlots = invnetoryItemsParent.GetComponentsInChildren<InventorySlot>();
            InventorySlotData.SaveInventorySlots(inventorySlots, savePathCurrentEq, parentOfInventorySlots);
        }
        catch { }

        savePathCurrentEq = Application.persistentDataPath + characterName;

        //-------------------SaveEquipmentsSlots-------------------
        // Svae the All the Equipment slots
        EquipmentSlots[] equipmentSlots = null;
        try
        {
            savePathCurrentEq += equipmentSlotsPath; // Specify the save file path
            equipmentSlots = equipementsItemsParent.GetComponentsInChildren<EquipmentSlots>();
            EquipmentSlotData.SaveEquipmentSlots(equipmentSlots, savePathCurrentEq);
        }
        catch { }

        savePathCurrentEq = Application.persistentDataPath + characterName;
        //-------------------SaveCharcaterStats-------------------
        // Save all the player data (Stats)
        try
        {
            savePathCurrentEq += playerStatsPath;
            Transform player = FindPlayer.Instance.Player.transform;
            var playerData = player.GetComponent<PlayerStat>();
            playerData.SavePlayerData(savePathCurrentEq);
        }
        catch { }

        savePathCurrentEq = Application.persistentDataPath + characterName;

        //-------------------SaveIconOfInventorySlots-------------------
        // Save the icon of the Inventory slots
        try
        {
            savePathCurrentEq += iconOnInvPath;

            GetComponent<SaveInvSlotsNew>().SaveIconInventorySlot(inventorySlots, savePathCurrentEq);
            savePathCurrentEq = Application.persistentDataPath + characterName;

            // Save the icon of the Equipment slots
            savePathCurrentEq += iconOnEqPath;
            GetComponent<SaveInvSlotsNew>().SaveIconEquipmentSlot(equipmentSlots, savePathCurrentEq);
        }
        catch { }

        //-------------------SaveSkillsStats-------------------
        // Save all the Skills stats on the skills tab
        try
        {
            SaveSkillsStats(true);
        }
        catch { }

        //-------------------SaveCharcaterNameAndScene-------------------
        // Save the character name and scene he is currently in
        try
        {
            int sceneID = SceneManager.GetActiveScene().buildIndex;
            PlayerPrefs.SetInt($"SceneID_{characterName}", sceneID);
            PlayerPrefs.Save();
        }
        catch { }



        //-------------------SaveQuestsInfo-------------------
        // Save the quests info (The quest the player has done)
        try
        {
            SaveQuests.instance.SaveQuestsInfo(SaveQuests.instance.savedQuestsArray);
        }
        catch { }


        //-------------------SaveCutsceneStat-------------------
        try
        {
            lastSceneCutscene.SaveCutsceneStat();
        }
        catch { }


        //-------------------SaveEnemies-------------------
        try
        {
            SaveEnemyActivations();
        }
        catch { }
    }


    /// <summary>
    /// Save the stats of all the Skills (Damage, level, require SP)
    /// </summary>
    /// <param name="isSave"> If the parameter is true then is saves the Skills
    /// Otherwise it load them </param>
    private void SaveSkillsStats(bool isSave)
    {
        int childCount = parentOfSkillsTab.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform parentChild = parentOfSkillsTab.GetChild(i);
            int childOfChildCount = parentChild.childCount;

            for (int j = 0; j < childOfChildCount; j++)
            {
                Transform skill = parentChild.GetChild(j);
                SkillSlots skillStats = skill.GetComponent<SkillSlots>();
                if(skillStats != null)
                {
                    if(isSave)
                    {
                        int damage = skillStats.DamageModifier;
                        int requireSP = skillStats.RequiredSP;

                        string levelText = skillStats.SkillLevelText.text; // Assuming SkillLevelText is a Text component
                        int level = int.Parse(levelText);

                        PlayerPrefs.SetInt("asd1" + "SkillDMG" + i + j, damage);
                        PlayerPrefs.SetInt("asd1" + "SkillSP" + i + j, requireSP);
                        PlayerPrefs.SetInt("asd1" + "SkillLVL" + i + j, level);
                    }
                    else
                    {
                        int damage = PlayerPrefs.GetInt("asd1" + "SkillDMG" + i + j);
                        int requireSP = PlayerPrefs.GetInt("asd1" + "SkillSP" + i + j);
                        int level = PlayerPrefs.GetInt("asd1" + "SkillLVL" + i + j);

                        skillStats.DamageModifier = damage;
                        skillStats.RequiredSP = requireSP;
                        skillStats.SkillLevelText.text = level.ToString();
                        skillStats.ReqSPText.text = requireSP.ToString();
                    }
                }
            }
        }
    }



    public void Load()
    {
        characterName = playerName;

        int enteriesTime = PlayerPrefs.GetInt("LoadTimes" + characterName);
        if (enteriesTime >= 1)
        {
            string savePathCurrentEq = Application.persistentDataPath + characterName;
            Debug.Log(savePathCurrentEq);

            Transform player = FindPlayer.Instance.Player.transform;
            var playerData = player.GetComponent<PlayerStat>();

            // Load all the quest givers stats
            try
            {
                dialogueManager.LoadQuestGivers();
            }
            catch { }


            //-------------------LoadPortal-------------------
            // Load the portal stats (Closed/Opened)
            try
            {
                LoadPortalState(characterName);
            }
            catch { }


            //-------------------LoadSkills-------------------
            // Load the Skills and assign them on the hotkeys
            try
            {
                savePathCurrentEq += skillSlotPath;
                GetComponent<ParentObject>().LoadChildData(savePathCurrentEq, imagesParent);

            }
            catch { }

            savePathCurrentEq = Application.persistentDataPath + characterName;


            //-------------------LoadHealingPotionsQuantity-------------------
            // Load the healing potions quantity
            try
            {
                savePathCurrentEq += skillSlotPath;
                var quantity = SaveItemsInInv.LoadPotionsQuantity(savePathCurrentEq);
                potionQuantity.text = quantity.ToString();
            }
            catch { }

            savePathCurrentEq = Application.persistentDataPath + characterName;


            //-------------------LoadItemsOnInventory-------------------
            // Load the items on the inventory (Gameobjects)
            try
            {
                savePathCurrentEq += itemsOnInventoryPath;
                SaveItemsInInv.LoadItemInInventory(savePathCurrentEq);
            }
            catch { }

            savePathCurrentEq = Application.persistentDataPath + characterName;


            //-------------------LoadWeaponsInHand-------------------
            // Load the weapons
            try
            {
                savePathCurrentEq += weaponsInHandsPath;
                WeaponsInHandData.LoadWeaponsInHand(savePathCurrentEq, player);
            }
            catch { }

            savePathCurrentEq = Application.persistentDataPath + characterName;


            //-------------------LoadCurrentEquipments-------------------
            // Load the current equipments (ScriptableObject)
            try
            {
                savePathCurrentEq += currentEquipmentsPath;
                SavingCurrentEquioments.LoadEquipmentData(Inventory.Instance.CurrentEquipments, savePathCurrentEq);
            }
            catch { }

            savePathCurrentEq = Application.persistentDataPath + characterName;


            //-------------------LoadCurrentItems-------------------
            // Load the current Items (ScriptableObject)
            savePathCurrentEq += currentItemsPath;
            try
            {
                List<Equipment> loadedEquipment = SavingCurrentItemsSO.LoadEquipmentData(savePathCurrentEq);
                List<Item> loadedItems = new (loadedEquipment);

                Inventory.Instance.Items = loadedItems;
            }
            catch { }


            savePathCurrentEq = Application.persistentDataPath + characterName;


            //-------------------LoadInventorySlots-------------------
            // Load the Inventory Slots Data and assign it
            InventorySlot[] inventorySlots = invnetoryItemsParent.GetComponentsInChildren<InventorySlot>();
            EquipmentSlots[] equipmentsSlots = equipementsItemsParent.GetComponentsInChildren<EquipmentSlots>();

            try
            {
                savePathCurrentEq += inventorySlotsPath; // Specify the save file path
                InventorySlotData.LoadInventorySlots(inventorySlots, savePathCurrentEq, parentOfInventorySlots);
            }
            catch { }

            savePathCurrentEq = Application.persistentDataPath + characterName;


            //-------------------LoadEquipmentSlots-------------------
            // Load the Equipment Slots Data and assign it
            try
            {
                savePathCurrentEq += equipmentSlotsPath; // Specify the save file path
                EquipmentSlotData.LoadEquipmentSlots(equipmentsSlots, savePathCurrentEq);
            }
            catch { }


            // Invoke the updateUI method in the InventoryUI script
            // In order to Refresh the Inventory and Equipemtn Slots
            try
            {
                inventoryUI.UpdateUI();
                equipmentsUI.UpdateUI();
            }
            catch { }

            savePathCurrentEq = Application.persistentDataPath + characterName;


            //-------------------LoadIconsForInventoryAndEquipmentsSlots-------------------
            // Load the skills
            try
            {
                savePathCurrentEq += iconOnInvPath;
                GetComponent<SaveInvSlotsNew>().LoadconInventorySlot(savePathCurrentEq);
            }
            catch { }

            savePathCurrentEq = Application.persistentDataPath + characterName;

            try
            {
                savePathCurrentEq += iconOnEqPath;
                GetComponent<SaveInvSlotsNew>().LoadIconEquipmentSlot(savePathCurrentEq);
            }
            catch { }


            //-------------------LoadSkillsStats-------------------
            // Load all the skills Stats in the Skills Tab
            try
            {
                SaveSkillsStats(false);
            }
            catch { }

            savePathCurrentEq = Application.persistentDataPath + characterName;


            //-------------------LoadCharcaterStats-------------------
            // Load all the player data (Stats)
            try
            {
                savePathCurrentEq += playerStatsPath;
                playerData.LoadPlayerData(savePathCurrentEq);
            }
            catch { }


            //-------------------LoadQuestsInfo-------------------
            // Load the quests the player has done
            try
            {
                SaveQuests.instance.LoadQuestsInfo();
            }
            catch { }


            //-------------------LoadCutsceneStat-------------------
            try
            {
                lastSceneCutscene.LoadCutsceneStat();
            }
            catch { }


            //-------------------LoadEnemies-------------------
            try
            {
                LoadEnemyActivations();
            }
            catch { }


            // Invoke the updateUI method in the InventoryUI script
            // In order to Refresh the Inventory and Equipemtn Slots
            try
            {
                inventoryUI.UpdateUI();
                equipmentsUI.UpdateUI();
            }
            catch { }
        }

        PlayerPrefs.SetInt("LoadTimes" + characterName, ++enteriesTimee);
    }


    /// <summary>
    /// Saves the activation states of multiple enemies.
    /// </summary>
    /// <param name="enemies">A list of enemy GameObjects to save activation states for.</param>
    public void SaveEnemyActivations()
    {
        int sceneID = SceneManager.GetActiveScene().buildIndex;
        foreach (var enemy in enemies)
        {
            // Generate a unique key for each enemy based on its name or identifier
            string key = "EnemyActivation_" + enemy.name + sceneID;

            // Save the activation state as 1 (active) or 0 (inactive) in PlayerPrefs
            PlayerPrefs.SetInt(key, enemy.activeSelf ? 1 : 0);
        }

        // Save the PlayerPrefs data
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the activation states of multiple enemies.
    /// </summary>
    /// <param name="enemies">A list of enemy GameObjects to load activation states for.</param>
    public void LoadEnemyActivations()
    {
        int sceneID = SceneManager.GetActiveScene().buildIndex;

        // Checks if they have saved data
        bool alreadySaved = PlayerPrefs.HasKey("EnemyActivation_" + enemies[0].name + sceneID);
        if(!alreadySaved)
        {
            return;
        }

        foreach (var enemy in enemies)
        {
            // Generate a unique key for each enemy based on its name or identifier
            string key = "EnemyActivation_" + enemy.name + sceneID;

            // Load the activation state (1 for active, 0 for inactive) from PlayerPrefs
            int activationValue = PlayerPrefs.GetInt(key, 0);

            // Set the activation state of the enemy GameObject
            enemy.SetActive(activationValue == 1);
        }
    }


    /// <summary>
    /// When deleting a saved game, all that saved game data will be deleted
    /// </summary>
    /// <param name="name"> The player name of the deleted game </param>
    public void DeleteAllPlayerData(string name)
    {

        characterName = playerName;
        string savePathCurrentEq = Application.persistentDataPath + characterName;

        
        // Delete skill slot data
        if (File.Exists(savePathCurrentEq + skillSlotPath))
        {
            File.Delete(savePathCurrentEq + skillSlotPath);
        }

        // Delete items on inventory data
        if (File.Exists(savePathCurrentEq + itemsOnInventoryPath))
        {
            File.Delete(savePathCurrentEq + itemsOnInventoryPath);
        }

        // Delete items on inventory data
        if (File.Exists(savePathCurrentEq + weaponsInHandsPath))
        {
            File.Delete(savePathCurrentEq + weaponsInHandsPath);
        }

        // Delete items on inventory data
        if (File.Exists(savePathCurrentEq + currentEquipmentsPath))
        {
            File.Delete(savePathCurrentEq + currentEquipmentsPath);
        }

        // Delete items on inventory data
        if (File.Exists(savePathCurrentEq + currentItemsPath))
        {
            File.Delete(savePathCurrentEq + currentItemsPath);
        }

        // Delete items on inventory data
        if (File.Exists(savePathCurrentEq + inventorySlotsPath))
        {
            File.Delete(savePathCurrentEq + inventorySlotsPath);
        }

        // Delete items on inventory data
        if (File.Exists(savePathCurrentEq + equipmentSlotsPath))
        {
            File.Delete(savePathCurrentEq + equipmentSlotsPath);
        }

        // Delete items on inventory data
        if (File.Exists(savePathCurrentEq + playerStatsPath))
        {
            File.Delete(savePathCurrentEq + playerStatsPath);
        }

        // Delete items on inventory data
        if (File.Exists(savePathCurrentEq + iconOnInvPath))
        {
            File.Delete(savePathCurrentEq + iconOnInvPath);
        }

        // Delete items on inventory data
        if (File.Exists(savePathCurrentEq + iconOnEqPath))
        {
            File.Delete(savePathCurrentEq + iconOnEqPath);
        }

        // Optionally, reset any flags or counters associated with loading times
        PlayerPrefs.SetInt("LoadTimes" + name, 0);
        
        PlayerPrefs.DeleteAll();
        
    }


    /// <summary>
    /// Save the portal state on the current scene if its closed or opened
    /// </summary>
    /// <param name="characterName"> The Player name (Path) </param>
    private void SavePortalState(string characterName)
    {
        bool isPortalActive = portal.activeSelf; // Assuming 'portal' is the reference to the GameObject representing the portal
        // Convert the boolean value to an integer (1 for active, 0 for inactive)
        int portalStatus = isPortalActive ? 1 : 0;

        // Saving the portal status using PlayerPrefs
        string portalKey = SceneManager.GetActiveScene().buildIndex + characterName + "Portal_1"; // Replace "Portal_1" with the actual unique key for this portal
        PlayerPrefs.SetInt(portalKey, portalStatus);
        PlayerPrefs.Save(); // Save the changes immediately (optional but recommended)
    }


    /// <summary>
    /// Load the portal state on the current scene if its closed or opened
    /// </summary>
    /// <param name="characterName"> The Player name (Path) </param>
    private void LoadPortalState(string characterName)
    {
        // Loading the portal status
        string portalKey = SceneManager.GetActiveScene().buildIndex + characterName + "Portal_1"; // Replace "Portal_1" with the actual unique key for this portal

        // Get the saved integer value from PlayerPrefs (1 for active, 0 for inactive)
        int portalStatus = PlayerPrefs.GetInt(portalKey);

        // Convert the integer value back to a boolean (true for active, false for inactive)
        bool isPortalActive = portalStatus == 1;

        // Now you can use the 'isPortalActive' variable to determine the portal's status
        // For example, you can activate or deactivate the portal GameObject based on the loaded status:
        portal.SetActive(isPortalActive);
    }




    public static string GetcharacterName()
    {
        return characterName;
    }

    public static void SetcharacterName(string name)
    {
        characterName = name;
    }
}
