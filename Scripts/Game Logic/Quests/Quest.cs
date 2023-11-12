using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Quest
{

    [SerializeField] private bool isActive;     // If the quest is active
    [SerializeField] private bool isComplete;   // Is the quest complete
    [SerializeField] private string title;      // The quest's title

    [TextArea(4, 10)]
    [SerializeField] private string description;    // The quest's description
    [SerializeField] private int expReward;         // The quest's expReward
    [SerializeField] private int gold;              // The quest's gold

    [SerializeField] private KillQuestGoal kill ;           // If the quest is about killing mobs
    [SerializeField] private GatheringQuestGoal gathering;  // If the quest is about picking up items

    private static int completedQuests = 0;  // Completed quests


    /// <summary>
    /// Marks the quest as completed, updates the quest count, triggers events for last and before-last quests,
    /// updates player's experience rewards, and sets quest states.
    /// </summary>
    public void Complete(PlayerStat playerStat)
    {
        completedQuests++;

        BeforeLastQuest();

        // The last quest on the scene
        LastQuest();


        isComplete = true;
        isActive = false;

        playerStat.CurrentExp = expReward;
        playerStat.TotalExp = expReward;


        Debug.Log("Completed");
    }

    
    /// <summary>
    /// The quest before the last is always for the Boss,
    /// so the boss gameobject is set to active.
    /// </summary>
    private void BeforeLastQuest()
    {
        if (completedQuests + 1 == QuestManager.Instance.QuestNumberInScene && SceneManager.GetActiveScene().buildIndex == 3)
        {
            var questGivers = DialogueManager.Instance.QuestGivers;
            for (int i = 0; questGivers.Length > 0; i++)
            {
                var alreadyTalked = questGivers[i].GetComponent<NPCInteraction>().Dialogue[0].alreadyTalkedThisSen;

                if (!alreadyTalked)
                {
                    questGivers[i].gameObject.SetActive(true);
                    break;
                }
            }

            try
            {
                GameObject boss = GameObject.Find("BossParent").transform.GetChild(0).gameObject;
                boss.SetActive(true);
                boss.GetComponent<EnemyController>().enabled = false;
            }
            catch { }
        }
    }


    /// <summary>
    /// Final quest to open the portal to navigate to the next scene.
    /// Usually called when the boss dies.
    /// </summary>
    private void LastQuest()
    {
        if (completedQuests == QuestManager.Instance.QuestNumberInScene)
        {
            // Find the portal and activate it
            GameObject.FindGameObjectWithTag("Portal").transform.GetChild(0).gameObject.SetActive(true);
            completedQuests = 0;  // Reset the counter
        }
    }

    public bool IsActive
    {
        get { return isActive; }
        set { isActive = value; }
    }

    public bool IsComplete
    {
        get { return isComplete; }
        set { isComplete = value; }
    }

    public string Title
    {
        get { return title; }
        set { title = value; }
    }

    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    public int ExpReward
    {
        get { return expReward; }
        set { expReward = value; }
    }

    public int Gold
    {
        get { return gold; }
        set { gold = value; }
    }

    public KillQuestGoal Kill
    {
        get { return kill; }
        set { kill = value; }
    }

    public GatheringQuestGoal Gathering
    {
        get { return gathering; }
        set { gathering = value; }
    }
}
