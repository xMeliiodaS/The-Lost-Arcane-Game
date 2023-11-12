using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ChooseCharacter : MonoBehaviour
{
    public List<GameObject> characters;                 // Chosen character
    [SerializeField] private TMP_InputField inputName;  // Input name reference

    int previousIndex;                          // The Chosen character index
    bool isFirstSelect;                         // If the character selected

    public GameObject savedGamePrefab;          // The saved prefab
    public GameObject savedGameParent;          // Assign the parent object in the Inspector
    public GameObject deactivateInput;          // Deactivate the input game object  
    public GameObject activateChooseButtons;
    public GameObject notValidObject;           // If the name valid or not
    public GameObject joinGameText;             // Join the game text appears
    public GameObject nameAlreadyExist;

    private readonly float yOffset = 100f;      // The initial Y offset between instantiated objects

    private GameObject lastSavedGame;           // Declare a variable to store the last instantiated game object
    public GameObject deactivateBackButton;     // Declare a variable to store the last instantiated game object

    public GameObject player;

    public List<GameObject> savedGameArray;     // Saved game objects
    GameObject savedGame = null;                // current saved game

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


    private void Start()
    {
        previousIndex = CharacterSelection.selectedCharacter;
        isFirstSelect = true;

        // Load the saved game objects
        StartCoroutine(Load());
    }
    IEnumerator Load()
    {
        yield return new WaitForSeconds(1f);
        LoadSavedGames();
    }


    /// <summary>
    /// If the name valid (More than 3 characters)
    /// </summary>
    /// <returns> Return true if the name is valid, otherwise false </returns>
    public bool IsNameValid()
    {
        string username = inputName.text;

        // Regular expression pattern
        string pattern = @"^.{3,12}$";
        return Regex.IsMatch(username, pattern);
    }


    /// <summary>
    /// Check the character name, if the name is already taken
    /// </summary>
    public void CheckCharacterName()
    {
        nameAlreadyExist.SetActive(false);

        int characterCount = PlayerPrefs.GetInt("CharacterCount");  // Count of all the characters the user have now
        string characterName = inputName.text;
        if (!IsNameValid())
        {
            notValidObject.SetActive(true);
            return;
        }

        notValidObject.SetActive(false);

        if (IsCharacterNameTaken(characterCount))
        {
            return;
        }
        else
        {
            StartCoroutine(CreateChar(characterName));
            SaveChosenCharacterIndex(characterName);
        }
    }

    private void SaveChosenCharacterIndex(string characterName)
    {
        int chosenCharacterIndex = CharacterSelection.selectedCharacter;

        Debug.Log(characterName + "Index");
        Debug.Log(chosenCharacterIndex);

        PlayerPrefs.SetInt(characterName + "Index", chosenCharacterIndex);
    }


    /// <summary>
    /// Create the character
    /// </summary>
    /// <param name="characterName"> The character name </param>
    /// <param name="characterCount"> Character count in the database </param>
    public IEnumerator CreateChar(string characterName)
    {
        joinGameText.SetActive(true);


        InstantiateLoadPrefab(characterName);

        deactivateInput.SetActive(false);
        activateChooseButtons.SetActive(true);
        deactivateBackButton.SetActive(false);

        yield return new WaitForSeconds(1f);

        LoadScene(characterName);
    }


    /// <summary>
    /// Stop the lobby music, load the next scene with an image
    /// </summary>
    /// <param name="characterName"></param>
    private void LoadScene(string characterName)
    {
        AudioManager.Instance.Stop("MainMenuSFXBackground");

        Transform scene = GameObject.Find("LobbyManager").transform;

        var loadScene = scene.GetComponent<LoadingScreen>();
        loadScene.LoadingScene(SceneManager.GetActiveScene().buildIndex + 1);

        DatabaseSystem.SetcharacterName(characterName);
        PlayerPrefs.SetString("PlayerName", characterName);

    }


    /// <summary>
    /// Check if the character name is taken, check the database if it contains the entered name
    /// </summary>
    /// <param name="characterCount"></param>
    /// <returns> Retrun true if the character name is taken, otherwise return false </returns>
    public bool IsCharacterNameTaken(int characterCount)
    {
        if (characterCount == 0)
        {
            PlayerPrefs.SetString("CharacterName" + characterCount, inputName.text);
            PlayerPrefs.SetInt("CharacterCount", characterCount + 1);
            PlayerPrefs.Save();

            return false;
        }

        for (int i = 0; i < characterCount; i++)
        {
            string savedCharacterName = PlayerPrefs.GetString("CharacterName" + i);

            if (inputName.text == savedCharacterName)
            {
                nameAlreadyExist.SetActive(true);

                // The entered name already exists
                return true;
            }
        }

        PlayerPrefs.SetString("CharacterName" + characterCount, inputName.text);
        PlayerPrefs.SetInt("CharacterCount", characterCount + 1);
        PlayerPrefs.Save();

        // The entered name is not taken
        return false;
    }


    /// <summary>
    /// Instantiate the load game prefab and save it
    /// </summary>
    /// <param name="characterName"> The characterName is the primary key </param>
    private void InstantiateLoadPrefab(string characterName)
    {
        savedGame = Instantiate(savedGamePrefab);

        savedGame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = characterName;
        savedGame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = DateTime.UtcNow.ToString("dd-MM-yyyy");
        savedGame.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text += "1";
        savedGame.transform.SetParent(savedGameParent.transform);
        if (lastSavedGame != null)
        {
            // Calculate the new position below the last game object
            Vector3 newPosition = lastSavedGame.transform.localPosition - new Vector3(0f, yOffset, 0f);

            // Set the new position and other properties of the instantiated object
            savedGame.transform.SetLocalPositionAndRotation(newPosition, savedGamePrefab.transform.localRotation);
            savedGame.transform.localScale = savedGamePrefab.transform.localScale;
        }
        else
        {
            // If there is no last game object, set the position to the same as the prefab
            savedGame.transform.SetLocalPositionAndRotation(new Vector3(0f, 950f, 0f), savedGamePrefab.transform.localRotation);
            savedGame.transform.localScale = savedGamePrefab.transform.localScale;
        }
        
        // Update the last saved game object reference
        lastSavedGame = savedGame;
        savedGameArray.Add(lastSavedGame);
        SaveSavedGames(savedGameArray);

        UpdateSavedGameobjectPosition();
    }


    /// <summary>
    /// Delete the instantiated gameobject and all the player's data for this specific savedGame
    /// </summary>
    /// <param name="savedGame"> Reference to the saved game </param>
    public void DeleteSavedGame(GameObject savedGame)
    {
        // Reference to the saved array
        var savedArray = GameObject.Find("LobbyManager").GetComponent<ChooseCharacter>().savedGameArray;

        var name = savedGame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        KnowTheIndexOfTheDeleted(savedGame, savedArray);

        DeleteAllPlayerData(name);
        PlayerPrefs.DeleteKey(name);

        //savedGameArray.Remove(savedGame); // Remove the game object from the saved game array
        savedArray.Remove(savedGame);       // Remove the game object from the saved game array
        // Destroy the game object
        Destroy(savedGame);

        // Save the updated list of saved games
        SaveSavedGames(savedArray);
        UpdateSavedGameobjectPosition();
    }


    /// <summary>
    /// Know the index of the deleted gameobject and shift all the next saved game objects to close the gap
    /// </summary>
    /// <param name="savedGame"> The deleted saved game </param>
    /// <param name="savedArray"> Has all the saved games </param>
    public void KnowTheIndexOfTheDeleted(GameObject savedGame, List<GameObject> savedArray)
    {
        // Find the index of the GameObject to remove in the savedArray
        int indexToRemove = savedArray.IndexOf(savedGame);

        if (indexToRemove >= 0)
        {
            // Delete the PlayerPrefs key for the entry at indexToRemove
            PlayerPrefs.DeleteKey("CharacterName" + indexToRemove);

            // Shift data to close the gap caused by the removal
            for (int i = indexToRemove + 1; i < savedArray.Count; i++)
            {
                string characterName = PlayerPrefs.GetString("CharacterName" + i);

                // Shift the character name and other relevant data to the previous index
                PlayerPrefs.SetString("CharacterName" + (i - 1), characterName);

                // Shift the Y position of the GameObject to close the gap
                IncreaseYPosition(savedArray[i], 100f);
            }

            // Delete the last entry (now at savedArray.Count - 1)
            PlayerPrefs.DeleteKey("CharacterName" + (savedArray.Count - 1));                                         
            PlayerPrefs.Save();
        }

    }


    /// <summary>
    /// Update the Array' savedgame's y position accoriding to the Y.
    /// The gap between each saved gameobject is 100 and the first one
    /// is 950
    /// </summary>
    private void UpdateSavedGameobjectPosition()
    {
        try
        {
            // Reference to the saved array
            var savedArray = GameObject.Find("LobbyManager").GetComponent<ChooseCharacter>().savedGameArray;

            int yOffsetBetweenObjects = 100; // Gap between objects
            int firstObjectYPosition = 950; // Y position of the first object

            // Set the local Y position of the first object
            Vector3 firstObjectLocalPosition = savedArray[0].transform.localPosition;
            firstObjectLocalPosition.y = firstObjectYPosition;
            savedArray[0].transform.localPosition = firstObjectLocalPosition;

            for (int i = 0; i < savedArray.Count; i++)
            {
                Vector3 newPosition = savedArray[i].transform.localPosition;
                newPosition.y = firstObjectYPosition - (i * yOffsetBetweenObjects);
                savedArray[i].transform.localPosition = newPosition;
            }
        }
        catch { }

    }


    /// <summary>
    /// When deleting a saved game, a gap occur, so this function 
    /// shift the GameObjects to close the gap
    /// </summary>
    /// <param name="gameObject"> Current saved game </param>
    /// <param name="amount"> The amount to increase the Y's position </param>
    public void IncreaseYPosition(GameObject gameObject, float amount)
    {
        Transform transform = gameObject.transform;

        // Calculate the new Y position by adding the specified amount
        float newY = transform.localPosition.y + amount;

        // Update the Y position of the GameObject's transform
        Vector3 newPosition = new(transform.localPosition.x, newY, transform.localPosition.z);
        transform.localPosition = newPosition;
    }


    /// <summary>
    ///  Load the scene after seconds and show Image while going to the other scene
    /// </summary>
    private void LoadSceneDelayed()
    {
        Transform reference = GameObject.Find("LobbyManager").transform;
        var loadingScreen = reference.GetComponent<LoadingScreen>();

        string playerName = GetName.userName;
        DatabaseSystem.SetcharacterName(GetName.userName);

        PlayerPrefs.SetString("PlayerName", playerName);

        Debug.Log("Testing the player name: " + DatabaseSystem.GetcharacterName());
        loadingScreen.LoadingScene(PlayerPrefs.GetInt($"SceneID_{DatabaseSystem.GetcharacterName()}"));
    }


    /// <summary>
    /// Invoke the LoadSceneDelayed() after 1 second
    /// </summary>
    public void LoadSavedGame()
    {
        Invoke(nameof(LoadSceneDelayed), 1f);
    }


    /// <summary>
    /// Depends on the index the player chose, set the character to active and the previous to deactive
    /// </summary>
    public void Choose()
    {
        if (!isFirstSelect)
        {
            // Disable the previous character
            characters[previousIndex].SetActive(false);

            // Enable current character
            characters[CharacterSelection.selectedCharacter].SetActive(true);

            // Saving the current character into a variable
            previousIndex = CharacterSelection.selectedCharacter;
        }
        else
        {
            // First time choosing character
            characters[CharacterSelection.selectedCharacter].SetActive(true);

            // Flag for the first time choosing character
            isFirstSelect = false;
        }
    }


    /// <summary>
    /// Save the transform of the instantiated gameobject using the
    /// unity-built in save system (PlayerPrefs)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="index"></param>
    private void SaveGameObjectTransform(Transform transform, int index)
    {
        var trnasName = transform.name;
        var transPos = transform.position;
        var transRot = transform.rotation;
        var transScale = transform.localScale;

        // Save the position, rotation, and scale of the transform to PlayerPrefs using unique keys with the provided index
        PlayerPrefs.SetFloat($"{trnasName}_PosX_{index}", transPos.x);
        PlayerPrefs.SetFloat($"{trnasName}_PosY_{index}", transPos.y);
        PlayerPrefs.SetFloat($"{trnasName}_PosZ_{index}", transPos.z);

        PlayerPrefs.SetFloat($"{trnasName}_RotX_{index}", transRot.x);
        PlayerPrefs.SetFloat($"{trnasName}_RotY_{index}", transRot.y);
        PlayerPrefs.SetFloat($"{trnasName}_RotZ_{index}", transRot.z);
        PlayerPrefs.SetFloat($"{trnasName}_RotW_{index}", transRot.w);

        PlayerPrefs.SetFloat($"{trnasName}_ScaleX_{index}", transScale.x);
        PlayerPrefs.SetFloat($"{trnasName}_ScaleY_{index}", transScale.y);
        PlayerPrefs.SetFloat($"{trnasName}_ScaleZ_{index}", transScale.z);

        // Save PlayerPrefs
        PlayerPrefs.Save();
    }


    /// <summary>
    /// Load all the instantiated saved games accoriding to thier saved transform
    /// </summary>
    /// <param name="transform"> The transform of the current instantiated saved game </param>
    /// <param name="index"> Its index of the list </param>
    private void LoadGameObjectTransform(Transform transform, int index)
    {
        string tName = transform.name;
        // Load the position, rotation, and scale of the transform from PlayerPrefs using the unique keys with the provided index
        float posX = PlayerPrefs.GetFloat($"{tName}_PosX_{index}");
        float posY = PlayerPrefs.GetFloat($"{tName}_PosY_{index}");
        float posZ = PlayerPrefs.GetFloat($"{tName}_PosZ_{index}");

        float rotX = PlayerPrefs.GetFloat($"{tName}_RotX_{index}");
        float rotY = PlayerPrefs.GetFloat($"{tName}_RotY_{index}");
        float rotZ = PlayerPrefs.GetFloat($"{tName}_RotZ_{index}");
        float rotW = PlayerPrefs.GetFloat($"{tName}_RotW_{index}");

        float scaleX = PlayerPrefs.GetFloat($"{tName}_ScaleX_{index}");
        float scaleY = PlayerPrefs.GetFloat($"{tName}_ScaleY_{index}");
        float scaleZ = PlayerPrefs.GetFloat($"{tName}_ScaleZ_{index}");

        // Set the loaded position, rotation, and scale to the transform
        transform.SetPositionAndRotation(new Vector3(posX, posY, posZ), new Quaternion(rotX, rotY, rotZ, rotW));
        transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        lastSavedGame = transform.gameObject;
    }


    /// <summary>
    /// Save the List of the Saved instantiated game objects
    /// </summary>
    /// <param name="savedGameArray"> Reference to the list </param>
    private void SaveSavedGames(List<GameObject> savedArray)
    {
        StringBuilder stringBuilder = new();

        // Serialize the saved games data
        for (int i = 0; i < savedArray.Count; i++)
        {
            GameObject savedGame = savedArray[i];
            string characterName = savedGame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            string date = savedGame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
            string level = savedGame.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text;

            // Append the data to the string builder
            stringBuilder.AppendLine($"{characterName},{date},{level}");

            // Save the transform data of the game object
            SaveGameObjectTransform(savedGame.transform, i);
        }

        // Save the serialized saved games data to PlayerPrefs
        PlayerPrefs.SetString("SavedGames", stringBuilder.ToString());

        // Save PlayerPrefs
        PlayerPrefs.Save();
    }



    /// <summary>
    /// Load the saved instantiated game objects
    /// </summary>
    private void LoadSavedGames()
    {
        // Retrieve the serialized saved games data from PlayerPrefs
        string serializedData = PlayerPrefs.GetString("SavedGames");

        // Clear the existing saved game objects
        foreach (GameObject savedGame in savedGameArray)
        {
            Destroy(savedGame);
        }
        savedGameArray.Clear();

        // Deserialize and instantiate the saved game objects
        string[] serializedLines = serializedData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < serializedLines.Length; i++)
        {
            string serializedLine = serializedLines[i];
            string[] data = serializedLine.Split(',');

            string characterName = data[0];
            string date = data[1];
            string level = data[2];

            GameObject savedGame = Instantiate(savedGamePrefab);

            // Assign the fields
            savedGame.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = characterName;
            savedGame.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = date;
            savedGame.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = level;
            savedGame.transform.SetParent(savedGameParent.transform);

            // Load the transform of the instantiated saved game object
            LoadGameObjectTransform(savedGame.transform, i);

            // Add the new saved game to the array
            savedGameArray.Add(savedGame);

            // Update the position of the saved games array items
            UpdateSavedGameobjectPosition();
        }
    }


    /// <summary>
    /// When deleting a saved game, all that saved game data will be deleted
    /// </summary>
    /// <param name="name"> The player name of the deleted game </param>
    public void DeleteAllPlayerData(string name)
    {
        string savePathCurrentEq = Application.persistentDataPath + name;

        try
        {
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
        }

        catch { }
    }
}