using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    #region Singelton
    // One inventory at all times
    private static DialogueManager instance;
    //public InventorySlot[] slots;
    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    public static DialogueManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DialogueManager();
            }
            return instance;
        }
    }

    #endregion


    [SerializeField] private TextMeshProUGUI nameText;          // Character name
    [SerializeField] private TextMeshProUGUI dialogueText;      // Dialogue itself
    [SerializeField] private Animator animator;                 // Animation for the dialogue
    [SerializeField] private Image characterSpriteImage;        // Reference to the UI Image component
    [SerializeField] private Button continueBtn;                // Reference to the continue button
    [SerializeField] private Queue<string> sentences;           // The dialogue's queue
    [SerializeField] private GameObject dialogueWindow;         // Reference to the dialogue gameobject window

    private int sentenecesCount;                        // Sentences count of the dialogue
    private int currentSentenece = 0;                   // Current sentence
    [SerializeField] private bool isTask;               // If there is Task
    [SerializeField] private QuestGiver[] questGivers;         // Array to store multiple QuestGiver instances

    private Dialogue tempDialogue;                  // Assign the temp dialogue to the dialogue for the quest
    private int i=0;
    private int acceptCounter = 0; 

    private string playerName;                      // Use to the quest save
    private readonly string path = "WantToTalk_";   // Path to save and load
    

    private void Start()
    {
        sentences = new Queue<string>();    // Create a new Queue on start
        playerName = FindPlayer.Instance.Player.GetComponent<PlayerStat>().PlayerName;
    }


    /// <summary>
    /// Starts a dialogue sequence with a character, displaying their sentences one by one.
    /// </summary>
    /// <param name="dialogue">The Dialogue object containing character information and sentences.</param>
    public void StartDialogue(Dialogue dialogue)
    {
        // Store the provided dialogue information
        tempDialogue = dialogue;

        // Set up the "Continue" button appearance
        continueBtn.GetComponentInChildren<TextMeshProUGUI>().text = "<< Continue >>";
        Image imageComponent = continueBtn.GetComponent<Image>();
        Color newColor = imageComponent.color;
        newColor.a = 0f;
        imageComponent.color = newColor;

        // Set the dialogueWindow to true so the user can see it
        dialogueWindow.SetActive(true);

        // Execute the animation
        animator.SetBool("isOpen", true);

        // Put the character name on the nameText.text
        nameText.text = dialogue.name;  
        characterSpriteImage.sprite = dialogue.characterSprite;

        // Clear existing sentences and reset counters
        sentences.Clear();
        currentSentenece = 0;

        // Queue up all sentences from the dialogue
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        // Store the total number of sentences and whether it's a task
        sentenecesCount = sentences.Count;
        isTask = dialogue.isTask;

        // Display the first sentence
        DisplayNextSentence();
    }


    /// <summary>
    /// Displays the next sentence in the dialogue sequence.
    /// </summary>
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)  // If the sentence is 0 then the dialogue ends
        {
            // End the dialogue
            EndDialogue();
            tempDialogue.alreadyTalkedThisSen = true;
            tempDialogue.wantToTalk = false;

            // Make the next NPC want to talk to the player
            try
            {
                questGivers[tempDialogue.npcNumber + 1].GetComponent<NPCInteraction>().Dialogue[0].wantToTalk = true;
            }
            catch { }

            // Unlock camera rotation and reactivate UI gameplay
            LockCameraa.GetInstance().UnlockCameraRotation();
            //DisableUI.GetInstance().GetUiGameplay().SetActive(true);
            DisableUI.GetInstance().ShowAllUIElements();

            return;
        }

        DisableUI.GetInstance().HideAllUIElements();

        if (isTask)
        {
            ShowTask();
        }

        // Get the next sentence and animate its display
        string sentence = sentences.Dequeue();

        // Stop any ongoing text animation
        StopAllCoroutines();

        // Display sentence with cool animation
        StartCoroutine(TypeSentence(sentence));
        currentSentenece += 1;
    }


    /// <summary>
    /// Displays and handles tasks or quests within the dialogue.
    /// </summary>
    public void ShowTask()
    {
        if (currentSentenece == sentenecesCount - 1)  // If the sentences is 0 then the dialogue ends
        {
            // Iterate through quest givers and process accepting quests
            for (; i < questGivers.Length; i++)
            {
                // Set button text and color for quest acceptance
                continueBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Accept";
                Image imageComponent = continueBtn.GetComponent<Image>();
                imageComponent.color = new Color(0.102f, 0.8f, 1f, 1f);
                acceptCounter++;

                // Get reference to player's stats and combat components
                PlayerStat playerStat = FindObjectOfType<PlayerStat>();
                playerStat.GetComponent<Combat>().SetCurrentNPCNull();

                // Check if the continue button has an onClick handler
                if (continueBtn.onClick != null)
                {
                    // Accept the quest and increment index
                    questGivers[i].AcceptQuest();
                    i++;
                    break;
                }

            }
        }
    }


    /// <summary>
    /// Add word by word to the dialogue text for a cool animation
    /// </summary>
    /// <param name="sentence"> Current sentence </param>
    /// <returns></returns>
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.004f);
        }
    }


    /// <summary>
    /// Ends the dialogue when the player clicks on "Accept"
    /// </summary>
    void EndDialogue()
    {
        // Close the dialogue window animation
        animator.SetBool("isOpen", false);

        // Wait for the dialogue window animation to complete
        StartCoroutine(WaitForAnimationToEnd());

        // Unlock the camera rotation
        LockCameraa.GetInstance().UnlockCameraRotation();

        // Re-enable the gameplay UI
        DisableUI.GetInstance().ShowAllUIElements();
    }


    /// <summary>
    /// Wait the animation to end then deactivate the Dialogue Window
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForAnimationToEnd()
    {
        yield return new WaitForSeconds(1f);
        dialogueWindow.SetActive(false);
    }


    /// <summary>
    /// Save the status of NPCs that want to talk to the player for future continuation.
    /// </summary>
    public void SaveQuestGivers()
    {
        for(int i=0;  i<questGivers.Length; i++)
        {
            // Get whether the NPC wants to talk
            var wannaTalk = questGivers[i].GetComponent<NPCInteraction>().Dialogue[0].wantToTalk;

            // Convert boolean to integer and save in PlayerPrefs
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            PlayerPrefs.SetInt(path + sceneIndex + playerName + i, wannaTalk ? 1 : 0);
        }
    }


    /// <summary>
    /// Load the status of NPCs that want to talk to the player for continuation.
    /// </summary>
    public void LoadQuestGivers()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (!PlayerPrefs.HasKey(path + sceneIndex + playerName + 0))
        {
            questGivers[0].GetComponent<NPCInteraction>().Dialogue[0].wantToTalk = true;
            return;
        }

        for (int i = 0; i < questGivers.Length; i++)
        {
            // Load the saved integer value from PlayerPrefs
            
            int wannaTalk = PlayerPrefs.GetInt(path + sceneIndex + playerName + i);

            // Convert the integer value back to a boolean
            bool talk = wannaTalk == 1;

            // Set the NPC's wantToTalk state based on the loaded value
            questGivers[i].GetComponent<NPCInteraction>().Dialogue[0].wantToTalk = talk;
        }
    }


    public QuestGiver[] QuestGivers
    {
        get { return questGivers; }
        set { questGivers = value; }
    }

    public int AcceptCounter
    {
        get { return acceptCounter; }
        set { acceptCounter = value; }
    }
}