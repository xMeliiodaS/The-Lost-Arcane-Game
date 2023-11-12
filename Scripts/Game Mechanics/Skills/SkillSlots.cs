using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkillSlots : MonoBehaviour
{
    [Header("Skill stats")]
    [SerializeField] private Skill skill;         // Reference to the skill's scriptable object
    [SerializeField] private int damageModifier;  // Damage modifier to the skill
    [SerializeField] private int requiredSP;      // Required SP to upgrade skill
    [SerializeField] private int jobID;           // Job id to the current job (1, 2, 3, 4)
    private readonly int nextDamageModifier = 2;  // When upgrading, the next upgrade will give more damage

    [Header("GUI")]
    [SerializeField] private TextMeshProUGUI skillLevelText;  // Reference to the skill Level Text
    [SerializeField] private TextMeshProUGUI reqSPText;       // Reference to the req SP Text
    [SerializeField] private TextMeshProUGUI currentSPText;   // Reference to the current SP Text

    private readonly int itemButton = 0;     // item button first child
    private readonly int levelUpButton = 1;  // level up button second child
    private readonly int icon = 2;           // icon third child
    private readonly int iconDrag = 0;       // icon drag first child of the icon

    private PlayerStat player;


    private void Start()
    {
        player = FindAnyObjectByType<PlayerStat>();
        currentSPText.text = player.SkillPoints.ToString();
    }


    /// <summary>
    /// Updates the visibility of the lock icon based on the player's skill level and the current scene.
    /// If the player's skill level is greater than the current scene index, the lock icon is enabled.
    /// Otherwise, it's disabled.
    /// </summary>
    private void Update()
    {
        int unlockSkills = gameObject.transform.parent.GetComponent<UnlockSkills>().skillLevelID;

        if (unlockSkills > SceneManager.GetActiveScene().buildIndex)
        {
            EnableLockIcon();
        }
        else
        {
            DisableLockIcon();
        }
    }

    /// <summary>
    /// If the skills tab is for this scene enable it.
    /// </summary>
    private void DisableLockIcon()
    {
        Transform parentTransform = gameObject.transform.parent;
        int childCount = parentTransform.childCount;

        // Disable interaction with the last two child objects
        for (int i = childCount - 2; i < childCount; i++)
        {
            Transform child = parentTransform.GetChild(i);
            child.gameObject.SetActive(false);
        }

        try
        {
            gameObject.transform.GetChild(levelUpButton).GetComponent<Button>().interactable = true;
            gameObject.transform.GetChild(itemButton).GetComponent<Button>().interactable = true;
            gameObject.transform.GetChild(icon).GetComponent<SkillSlotDrag>().enabled = true;
            gameObject.transform.GetChild(icon).GetChild(iconDrag).GetComponent<DragSkill>().enabled = true;
        }
        catch { }
    }


    /// <summary>
    /// If the skills tab is not for this scene disable it.
    /// </summary>
    private void EnableLockIcon()
    {
        Transform parentTransform = gameObject.transform.parent;
        int childCount = parentTransform.childCount;

        // Disable interaction with the last two child objects
        for (int i = childCount - 2; i < childCount; i++)
        {
            Transform child = parentTransform.GetChild(i);
            child.gameObject.SetActive(true);
        }

        try
        {
            gameObject.transform.GetChild(levelUpButton).GetComponent<Button>().interactable = false;
            gameObject.transform.GetChild(itemButton).GetComponent<Button>().interactable = false;
            gameObject.transform.GetChild(icon).GetComponent<SkillSlotDrag>().enabled = false;
            gameObject.transform.GetChild(icon).GetChild(iconDrag).GetComponent<DragSkill>().enabled = false;
        }
        catch { }
    }


    /// <summary>
    /// Upgrade the Skill stats and increase the required SP
    /// </summary>
    public void UpgradeSkill()
    {
        if (player.SkillPoints < requiredSP)
        {
            return;
        }

        player.DecreaseSkillPoints = requiredSP;
        Gold.instance.SetCurrentGold(player.CurrentGold.ToString());
        skill.damageModifier += damageModifier;                     // Increase the skill's damage
        damageModifier += nextDamageModifier;                       // The next upgrade increases more damage
                                      
        int skillLevelInt = Convert.ToInt16(skillLevelText.text);   // convert the skill level text to int on a new variable
        skillLevelInt++;                                            // Increase the skill level text
        skillLevelText.text = skillLevelInt.ToString();             // Set the text to the new skill level
                                                                     
        int currentSP = Convert.ToInt16(currentSPText.text);        // Convert the SP to int
        currentSP -= requiredSP;                                    // Decrease the current SP
        currentSPText.text = currentSP.ToString();                  // Update the SP's Text
        player.SkillPoints = currentSP;                             // Update the SP on the player

        int reqSPInt = Convert.ToInt16(reqSPText.text);             // convert the req SP text to int on a new variable
        reqSPInt++;                                                 // Increase the skill's req SP (UI)
        requiredSP++;                                               // Increase the actual skill's req SP
        reqSPText.text = reqSPInt.ToString();                       // Set the text to the new req SP
    }


    public Skill Skill
    {
        get { return skill; }
        set { skill = value; }
    }

    public int DamageModifier
    {
        get { return damageModifier; }
        set { damageModifier = value; }
    }

    public int RequiredSP
    {
        get { return requiredSP; }
        set { requiredSP = value; }
    }

    public TextMeshProUGUI SkillLevelText
    {
        get { return skillLevelText; }
        set { skillLevelText = value; }
    }

    public TextMeshProUGUI ReqSPText
    {
        get { return reqSPText; }
        set { reqSPText = value; }
    }
}
