using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private Dialogue[] dialogue;      // Reference to the dialgoue's array

    [SerializeField] private bool isTalking = false;   // If the NPC is talking to the player
    private QuestGiver questGiver;                     // Reference to the QuestGiver instance

    private bool tutorialCheck = true;              // If this dialogue related to tutorial

    [SerializeField] private int currentQuest = 0;  // Reference to the current quest 

    private void Start()
    {
        questGiver = GetComponent<QuestGiver>();
    }


    /// <summary>
    /// Assigns the given QuestGiver to this quest.
    /// </summary>
    /// <param name="questGiver">The QuestGiver to assign.</param>
    public void AssignQuestGiver(QuestGiver questGiver)
    {
        this.questGiver = questGiver;
    }


    /// <summary>
    /// When the player enters the NPC collider
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        questGiver.Player = FindPlayer.Instance.Player.GetComponent<PlayerStat>();
    }


    /// <summary>
    /// When the player exits the NPC collider
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isTalking)
            {
                EndConversation();
            }
        }
    }
    

    /// <summary>
    /// Start the conversation between the player and NPC
    /// </summary>
    public void StartConversation()
    {
        // If there is a collider to the NPC then the player can talk to him
        if(gameObject.GetComponent<BoxCollider>().enabled == true)
        {
            questGiver = GetComponent<QuestGiver>();
            questGiver.IsTalking = true;
            DialogueManager dialogueManager = FindObjectOfType<DialogueManager>(); // Find the DialogueManager instance

            if (dialogueManager != null)
            {
                 for (currentQuest = 0; currentQuest < dialogue.Length; currentQuest++)
                 {
                    // If the current dialogue has not been spoke then start it
                    if (!dialogue[currentQuest].alreadyTalkedThisSen && dialogue[currentQuest].wantToTalk)
                    {
                        // Passing the array of QuestGiver instances to StartDialogue
                        dialogueManager.StartDialogue(dialogue[currentQuest]);

                        break;  // To prevent continuing the Dialogue array
                    }
                 }
            }
                        
            currentQuest = 0;   // Reset the current quest
            isTalking = true;   // NPC is talking to the player
        }
    }


    /// <summary>
    /// After finishing the conversation set that the NPC dont want to talk
    /// </summary>
    private void EndConversation()
    {
        questGiver.IsTalking = false;
        isTalking = false;
    }


    private void Update()
    {
        // If there is more quest, the player can talk to him
        if (questGiver.Quest.IsComplete)
        {
            if (gameObject.GetComponent<BoxCollider>().enabled == false)
            {
                gameObject.GetComponent<BoxCollider>().enabled = true;
            }

            // Check if it's a tutorial NPC and handle tutorial specific interactions
            if (tutorialCheck)
            {
                if (gameObject.name == "TutorialNPC1")
                {
                    if (SceneManager.GetActiveScene().buildIndex == 2)
                    {
                        var house = GameObject.FindWithTag("TutorialHouse");
                        house.GetComponent<BoxCollider>().enabled = false;
                        GameObject.FindWithTag("TextTutorial").SetActive(false);
                        tutorialCheck = false;
                    }
                }
            }
        }
    }


    public Dialogue[] Dialogue
    {
        get { return dialogue; }
        set { dialogue = value; }
    }
}
