using UnityEngine;

public class QuestManager : MonoBehaviour
{
    #region Singelton

    private static QuestManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one instance of Inventory found!");
            return;
        }
        instance = this;
    }

    public static QuestManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new QuestManager();
            }
            return instance;
        }
    }


    #endregion

    private int questNumberInScene;     // The quest's number on the current scene

    private void Start()
    {
        // Get the quests number on scene accoriding to the length of the Quest Givers
        var questGiversNumInScene = GetComponent<DialogueManager>().QuestGivers.Length;

        questNumberInScene = questGiversNumInScene;
    }

    public int QuestNumberInScene
    {
        get { return questNumberInScene; }
        set { questNumberInScene = value; }
    }
}
