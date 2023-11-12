using TMPro;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    //----------------------------------------------
    [SerializeField] private Quest quest;                         // The quest for the player

    [HideInInspector] private PlayerStat player;                  // Reference to the player

    [SerializeField] private TextMeshProUGUI titleText;           // The quest's title
    [SerializeField] private TextMeshProUGUI descriptionText;     // The quest's descreption
    [SerializeField] private TextMeshProUGUI expText;             // The quest's EXP reward
    [SerializeField] private TextMeshProUGUI goldText;            // The quest's Gold reward
    [SerializeField] private TextMeshProUGUI currentAmountText;
    [SerializeField] private TextMeshProUGUI requiredAmountText;

    [SerializeField] private Sprite itemImage;

    //----------------------------------------------

    [SerializeField] private bool isTalking = false;
    [SerializeField] private bool isFinalQuest = false;
    //----------------------------------------------
    [SerializeField] private GameObject activeQuest;            // Reference to the Quest button on the Quest window
    [SerializeField] private GameObject questButtonPrefab;      // Reference to the Quest button on the Quest window

    //----------------------------------------------
    private readonly float yOffset = 111f;                   // The vertical offset between the cloned objects
    private static int clonedObjectCount = 0;       // The total count of cloned objects

    //----------------------------------------------
    [SerializeField] private GameObject cameraRotationObject;   // Reference to the cameraRotationObject
    private Quaternion initialRotation;                         // Save the rotation of the camera

    //Tutorial----------------------------------------------
    [SerializeField] private bool isTutorial;
    [SerializeField] private GameObject arrowMarker;
    [SerializeField] private GameObject weaponTutorial;
    [SerializeField] private GameObject enemyGM;
    private GameObject clonedQuestButton;


    private void Start()
    {
        player = FindAnyObjectByType<PlayerStat>();

    }

    /// <summary>
    /// When the player click on the Accept Button, unlock the Camera rotation and
    /// Give the player the quest to do
    /// </summary>
    public void AcceptQuest()
    {

        LockCameraa.GetInstance().UnlockCameraRotation();

        if (quest.IsActive || quest.IsComplete)
        {
            Debug.Log("The player have this quest in progress");
        }
        
        if (isTutorial)
        {
            try
            {
                weaponTutorial.SetActive(true);
                arrowMarker.SetActive(true);
            }
            catch { }
            Invoke(nameof(ActivateEnemy), 1);
        }
        GiveQuest();

        //When the quest is not complete and not active
        if (!quest.IsComplete && !quest.IsActive)
        {
            if (isTutorial)
            {
                weaponTutorial.SetActive(true);
                arrowMarker.SetActive(true);
            }
            GiveQuest();
            return;
        }

    }

    /// <summary>
    /// Activate the enemy if this is the second quest
    /// </summary>
    public void ActivateEnemy()
    {
        if (DialogueManager.Instance.AcceptCounter == 2)
        {
            enemyGM.SetActive(true);
        }

    }


    /// <summary>
    /// When accepting the new quest clone the prefab
    /// </summary>
    /// <returns>Reference to the BridgeScript component of the cloned button.</returns>
    public BridgeScript CloneQuestPrefab()
    {
        // Calculate the new Y position based on the count of cloned objects
        float newYPosition = -clonedObjectCount * yOffset;

        // Instantiate the quest button prefab
        clonedQuestButton = Instantiate(questButtonPrefab, activeQuest.transform);
        clonedQuestButton.transform.localPosition = new Vector3(0f, newYPosition, 0f);
        clonedObjectCount++;

        SaveQuests.instance.savedQuestsArray.Add(clonedQuestButton);

        BridgeScript bridgeScriptRef = clonedQuestButton.GetComponentInChildren<BridgeScript>();
        return bridgeScriptRef;
    }

    private void Update()
    {
        if (this.quest.IsComplete)
        {
            gameObject.GetComponent<BoxCollider>().enabled = true;
            clonedQuestButton.transform.GetChild(1).gameObject.SetActive(true);
        }
    }


    /// <summary>
    /// Copy the quest properties to the new prefab
    /// </summary>
    public void GiveQuest()
    {
        var newPrefab = CloneQuestPrefab();

        newPrefab.title = quest.Title;
        newPrefab.description = quest.Description;
        newPrefab.expReward = quest.ExpReward;
        newPrefab.gold = quest.Gold;
        newPrefab.itemImage = itemImage;
        newPrefab.currentAmount = quest.Kill.CurrentAmount;
        newPrefab.requiredAmount = quest.Kill.RequiredAmount;
        newPrefab.acceptQuestButton.SetActive(false);

        quest.IsActive = true;

        // Add the quest to player's Quests
        player.Quest.Add(quest);
        gameObject.GetComponent<BoxCollider>().enabled = false;

        FirstQuest();
    }


    /// <summary>
    /// Activates the first quest for NPCs in the scene.
    /// </summary>
    private void FirstQuest()
    {
        // Get all the npcs that will give u a quest on this scene
        var questGivers = DialogueManager.Instance.QuestGivers;
        for (int i = 0; i < questGivers.Length; i++)
        {
            var alreadyTalked = questGivers[i].GetComponent<NPCInteraction>().Dialogue[0].alreadyTalkedThisSen;
            if (!alreadyTalked)
            {
                try
                {
                    var enemyParent = GameObject.Find("Enemies");
                    GameObject enemyName = enemyParent.transform.GetChild(i).gameObject;
                    if (enemyName.name.Equals($"Enemy{i}"))
                    {
                        enemyName.SetActive(true);
                        break;
                    }
                }
                catch { Debug.LogWarning("No enemies was found"); }
            }
        }
    }


    // GETTERS AND SETTERS
    public Quest Quest
    {
        get { return quest; }
        set { quest = value; }
    }

    public PlayerStat Player
    {
        get { return player; }
        set { player = value; }
    }

    public bool IsTalking
    {
        get { return isTalking; }
        set { isTalking = value; }
    }

}
