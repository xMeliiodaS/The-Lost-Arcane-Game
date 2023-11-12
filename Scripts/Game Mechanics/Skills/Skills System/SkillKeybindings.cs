using StarterAssets;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class SkillKeybindings : MonoBehaviour
{
    [SerializeField] private int slotID;        // Skill Slot ID
    [SerializeField] private Image skillImage;  // Cooldown Icon
    [SerializeField] private float cooldown;    // Cooldown timer

    [SerializeField] private bool isSkillCooldown = false;      // If the skill is on cooldown or no
    [SerializeField] private bool isAbilityCooldown = false;    // If the ability is on cooldown or no
    [SerializeField] private bool isHealingCooldown = false;    // If the healing is on cooldown or no

    [SerializeField] private GameObject skill;  // Skill's reference
    [SerializeField] private Transform player;  // Reference to the Player

    [SerializeField] private Transform attachPoint;         // Some skills have effect to add before/after the skill (player hand)
    [SerializeField] private float effectAnimationTime;     // Time to execute the skill
    [SerializeField] private int effectAnimationNumber;     // The skill's animation's number (id)
    [SerializeField] private GameObject crosshair;          // Reference to the crosshair object
    [SerializeField] private GameObject cantUseSkillText;   // Reference to the cantuseskill object

    private bool isUsingEffect = false; // If the player using an effect now                            

    private InputAction skillsInput;    // Reference to the Input Action
    private @StarterAssets41 skillKey;  // Reference to the Skill keys



    // Fields for the player health and health bar
    [SerializeField] private HealthBar playerHealthBar;
    private readonly int healthChild = 2;

    private bool cooldownAnimation = false;                    // If there is an animation on the cooldown
    private const string SaveKey = "SkillKeybindings";

    private static bool isPlayerUsingSkillNow = false;

    private SkillInfo skillInfo;

    // To determine what is the type of this gameobject
    [SerializeField] private bool isSkill;
    [SerializeField] private bool isAbility;
    [SerializeField] private bool isHeal;

    [SerializeField] private int riftCounter = 0;

    [SerializeField] private Item potion;

    /// <summary>
    /// sets up input actions for skills, abilities, and healing based on the specified
    /// slotID and boolean variables (isSkill, isAbility, isHeal).
    /// When the script's GameObject is enabled, it assigns input actions,
    /// attaches corresponding methods, and enables the input actions for use in the game.
    /// </summary>
    private void OnEnable()
    {
        var skillKeyRef = skillKey.Player;
        skillsInput = skillKey.Player.FirstSkill;   // Assign the input action
        skillsInput.Enable();                       // Enable the input action
        InputAction[] skillActions = new InputAction[]
        {
                skillKeyRef.FirstSkill,
                skillKeyRef.SecondSkill,
                skillKeyRef.ThirdSkill,
                skillKeyRef.FourthSkill,
                skillKeyRef.FivthSkill,
                skillKeyRef.SixthSkill,
                skillKeyRef.SeventhSkill,
                skillKeyRef.EigthSkill,
                skillKeyRef.FirstAbility,
                skillKeyRef.SecondAbility,
                skillKeyRef.ThirdAbility,
                skillKeyRef.Heal,
        };

        for (int i = 0; i < skillActions.Length; i++)
        {
            if (this.slotID == i + 1)
            {
                if(isSkill)
                {
                    skillActions[i].performed += Skill;
                }
                else if (isAbility)
                {
                    skillActions[i].performed += Ability;
                }
                else if (isHeal)
                {
                    skillActions[i].performed += Healing;
                }

                skillActions[i].Enable();               // Enable the input action
                break;
            }
        }
    }

    

    private void Awake()
    {
        skillKey = new @StarterAssets41();                      // Assign the skillKey
    }


    /// <summary>
    /// method of this script ensures that all input actions related to skills,
    /// abilities, and healing are properly disabled and cleaned up when the
    /// GameObject becomes disabled.
    /// This helps to prevent any lingering effects or
    /// unwanted input responses when the object is no longer active in the scene
    /// </summary>
    private void OnDisable()
    {
        var skillKeyPlayer = skillKey.Player;
        //skillKey.Disable();                                           // Disable the input action
        skillsInput.Disable();
        skillKeyPlayer.FirstSkill.Disable();                            // Disable the input action
        skillKeyPlayer.SecondSkill.performed -= Skill;                  // Disable the input action
        skillKeyPlayer.ThirdSkill.performed -= Skill;                   // Disable the input action
        skillKeyPlayer.FourthSkill.performed -= Skill;                  // Disable the input action
        skillKeyPlayer.FivthSkill.performed -= Skill;                   // Disable the input action
        skillKeyPlayer.SixthSkill.performed -= Skill;                   // Disable the input action
        skillKeyPlayer.SeventhSkill.performed -= Skill;                 // Disable the input action
        skillKeyPlayer.EigthSkill.performed -= Skill;                   // Disable the input action
        skillKeyPlayer.FirstAbility.performed -= Skill;
        skillKeyPlayer.SecondAbility.performed -= Skill;
        skillKeyPlayer.ThirdAbility.performed -= Skill;
        skillKeyPlayer.FourthAbility.performed -= Skill;
        skillKeyPlayer.FirstAbility.performed -= Ability;
        skillKeyPlayer.SecondAbility.performed -= Ability;
        skillKeyPlayer.ThirdAbility.performed -= Ability;
        skillKeyPlayer.FourthAbility.performed -= Ability;
        skillKeyPlayer.Heal.performed -= Healing;
    }


    // Start is called before the first frame update
    void Start()
    {
        skillImage.fillAmount = 0;
        player = FindPlayer.Instance.Player.transform; // Find the player on the scene
    }


    // Update is called once per frame
    void Update()
    {
        if (isSkillCooldown || isAbilityCooldown || isHealingCooldown)
        {
            // Call CoolDownAnimation for the skill
            if (isSkillCooldown)
            {
                CoolDownAnimation1(ref isSkillCooldown, skill.GetComponent<SkillInfo>());
            }
        }
    }

    /// <summary>
    /// Heals the player if enough potions
    /// </summary>
    /// <param name="obj"></param>
    private void Healing(InputAction.CallbackContext obj)
    {
        if (!isHealingCooldown)
        {
            PlayerStat stat = player.GetComponent<PlayerStat>();
            if (!CheckIfEnoughPotions())
            {
                cantUseSkillText.SetActive(true);
                AudioManager.Instance.Play("CantUseSKillEffect");
                StartCoroutine(DeactiveCantUseSkill());
                return;
            }

            HealPlayer(stat);
            UpdateHealthBarUI(stat);
            StartCoroutine(StartHealingCooldown(cooldown));

            Inventory.Instance.ActivateGameObjectWhenDroppingItem("Potion", true);
            Inventory.Instance.Remove(potion);

        }
        else
        {
            cantUseSkillText.SetActive(true);
            AudioManager.Instance.Play("CantUseSKillEffect");
            StartCoroutine(DeactiveCantUseSkill());
        }
    }


    /// <summary>
    /// checks if the player has enough healing potions
    /// </summary>
    /// <returns></returns>
    private bool CheckIfEnoughPotions()
    {
        TextMeshProUGUI potionQuantityText = gameObject.transform.parent.GetChild(2).GetComponent<TextMeshProUGUI>();
        if (int.TryParse(potionQuantityText.text, out int intValue))
        {
            if (intValue > 0)
            {
                // The player has enough potions, decrement the value
                intValue--;
                // Convert the updated intValue back to string
                string updatedPotionQuantity = intValue.ToString();

                // Update the TextMeshProUGUI component with the new potion quantity
                potionQuantityText.text = updatedPotionQuantity;

                // Return true to indicate that the player has enough potions
                return true;
            }
            else
            {
                // The player has no more potions, return false
                return false;
            }
        }
        return false;
    }


    /// <summary>
    /// Add heal to the player health
    /// </summary>
    /// <param name="stat"></param>
    private void HealPlayer(PlayerStat stat)
    {
        int maxHealth = stat.MaxHealth;
        float modifiedHealth = maxHealth * 0.25f;

        // Make sure the modified health does not exceed the maximum health
        int finalHealth = Mathf.Min(stat.CurrentHealth + (int)modifiedHealth, maxHealth);

        stat.CurrentHealth = finalHealth;
    }


    /// <summary>
    /// Update the Health Bar UI
    /// </summary>
    /// <param name="stat"></param>
    private void UpdateHealthBarUI(PlayerStat stat)
    {
        int currentHealth = stat.CurrentHealth;
        playerHealthBar.transform.GetChild(healthChild).GetComponent<TextMeshProUGUI>().text = currentHealth.ToString();
        playerHealthBar.SetSliderBar(currentHealth);
    }


    /// <summary>
    /// Start the cooldown for the healing in order to use the potion again
    /// </summary>
    /// <param name="cooldownTime"> cooldownTime to reset the healing </param>
    /// <returns></returns>
    private IEnumerator StartHealingCooldown(float cooldownTime)
    {
        isHealingCooldown = true; // Set ability cooldown flag to true
        float timer = cooldownTime;

        while (timer > 0)
        {
            // Update the cooldown fill amount
            skillImage.fillAmount = 1 - (timer / cooldownTime);

            // Decrease the timer
            timer -= Time.deltaTime;

            yield return null;
        }

        // Reset the cooldown values
        isHealingCooldown = false;
        skillImage.fillAmount = 0;
    }


    /// <summary>
    /// Do the ability depend on what hotkey the player pressed on
    /// </summary>
    /// <param name="obj"></param>
    private void Ability(InputAction.CallbackContext obj)
    {
        // If there is a sprite attached to the first child it means there is an ability, else cant use abilities
        if (gameObject.transform.GetChild(0).GetComponent<Image>().sprite != null)
        {
            if (!isAbilityCooldown)
            {
                var teleport = player.GetComponent<Teleport>();
                skillInfo = gameObject.transform.GetChild(1).GetComponent<SkillInfo>();
                if (skillInfo.ability.ToString() == "teleport")
                {
                    
                    StartCoroutine(teleport.TeleportForward());
                    StartCoroutine(StartCooldownForAbility(cooldown));

                    return;
                }
                else if (skillInfo.ability.ToString() == "timeManup")
                {
                    var timeAbility = player.GetComponent<TimeAbility>();
                    timeAbility.SlowDownTime();

                    StartCoroutine(StartCooldownForAbility(cooldown));

                    return;
                }
                else if (skillInfo.ability.ToString() == "riftWalker")
                {
                    if(teleport.RiftWalker())
                    {
                        riftCounter++;
                    }
                    else
                    {
                        return;
                    }

                    // The player can make 3 pairs of the rift before the cooldown starts
                    if(riftCounter == 3)
                    {
                        // Reset the rift counter
                        riftCounter = 0;
                        
                        StartCoroutine(StartCooldownForAbility(cooldown));
                    }
                    return;
                }
                else if(skillInfo.ability.ToString() == "swiftSlasher")
                {
                    var swiftSlasher = player.GetComponent<SwiftSlasher>();

                    swiftSlasher.SpeedAbility();
                    StartCoroutine(StartCooldownForAbility(cooldown));
                }
            }
            else
            {
                cantUseSkillText.SetActive(true);
                AudioManager.Instance.Play("CantUseSKillEffect");
                StartCoroutine(DeactiveCantUseSkill());
            }
        }
    }


    /// <summary>
    /// Start the cooldown for the ability in order to use it again
    /// </summary>
    /// <param name="cooldownTime"> cooldownTime to reset the ability </param>
    /// <returns></returns>
    private IEnumerator StartCooldownForAbility(float cooldownTime)
    {
        isAbilityCooldown = true; // Set ability cooldown flag to true
        float timer = cooldownTime;

        while (timer > 0)
        {
            // Update the cooldown fill amount
            skillImage.fillAmount = 1 - (timer / cooldownTime);

            // Decrease the timer
            timer -= Time.deltaTime;

            yield return null;
        }

        isAbilityCooldown = false;
        skillImage.fillAmount = 0;
    }


    
    /// <summary>
    /// Do the skill depend on what hotkey the player pressed on
    /// </summary>
    /// <param name="obj"></param>

    void Skill(InputAction.CallbackContext obj)
    {
        var isPlayerHasWeapon = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Inventory>();

        // If the player does not have an equipped weapon, then he can't use the skill
        if (isPlayerHasWeapon.WeaponsInHand[8] == null)
            return;

        // If other skill is executing then he cant use the skill
        if (isPlayerUsingSkillNow)
            return;

        GetComponentsOnChildToMatchEffect();    // Get the components of the skill

        if (!isSkillCooldown)            // If there is no cooldown for this skill
        {
            isPlayerUsingSkillNow = true;
            Animator playerAnimator = player.GetComponent<Animator>();
            playerAnimator.SetLayerWeight(2, 0f);
            DoAnimation();

            // After the animation ends start executing the Skill
            StartCoroutine(PerformSkill(effectAnimationTime));

            // Before the Skill execute do animation
            PutBeforeEffectNearPlayer();
        }
        else
        {
            cantUseSkillText.SetActive(true);
            AudioManager.Instance.Play("CantUseSKillEffect");
            StartCoroutine(DeactiveCantUseSkill());
        }
    }


    /// <summary>
    /// Decativate the Text cant use skill after 1 second
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeactiveCantUseSkill()
    {
        yield return new WaitForSeconds(1f);
        cantUseSkillText.SetActive(false);
    }


    /// <summary>
    /// Match the skill and put it near the player on the right position and rotation
    /// </summary>
    public void GetComponentsOnChildToMatchEffect()
    {
        skillInfo = gameObject.transform.GetChild(1).GetComponent<SkillInfo>();
        try
        {
            // Get the time where the animation need to play before doing the skill
            effectAnimationTime = skillInfo.effectAnimationTime;

            // Get the id of the animation
            effectAnimationNumber = skillInfo.effectNumber;

            if (skillInfo.handEffectTransform != null && !isUsingEffect)
            {
                isUsingEffect = true;       // Mark the isUsingEffect as true

                // Try to get the hand's effect if there is
                var effectHand = skillInfo.handEffect;

                // Try to get the hand's transform effect if there is
                var effectHandTransform = skillInfo.handEffectTransform;

                // Instantiate and position the hand effect
                GameObject handEffect = Instantiate(effectHand);
                handEffect.transform.SetParent(effectHandTransform);
                handEffect.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }
        catch
        {
            Debug.LogWarning("There is no second child for the " + gameObject);
        }
    }


    /// <summary>
    /// Do animation before the skill
    /// </summary>
    public void DoAnimation()
    {
        if (skillInfo.isMagicEffect)
        {
            PerformMagicEffectAnimation();
        }
        else if (skillInfo.isSlashEffect)
        {
            PerformSlashEffectAnimation();
        }
    }


    /// <summary>
    /// Perform the magic effect animation
    /// </summary>
    private void PerformMagicEffectAnimation()
    {
        cooldownAnimation = true;                                   // Mark the cooldownAnimation as true
        skillInfo.isUsing = true;                                   // Mark the isUsing on the SkillInfo script as true
        isSkillCooldown = true;                                     // There is a cooldown
        skillImage.fillAmount = 1;                                  // Fill the Image with the cooldown Icon
        skill.GetComponent<SkillInfo>().isCooldown = true;          // Mark the isCooldown on the SkillInfo script as true
        Animator playerAnimator = player.GetComponent<Animator>();  // Get the player's Animator

        try
        {
            playerAnimator.SetInteger("EffectNumber", effectAnimationNumber); // Know which skill's Animation to use
            playerAnimator.SetTrigger("UseSkill"); // Start executing the animation
        }
        catch
        {
            Debug.LogWarning("No animation found for Magic effect");
        }
    }


    /// <summary>
    /// Perform the slash effect animation
    /// </summary>
    private void PerformSlashEffectAnimation()
    {
        cooldownAnimation = true;                                   // Mark the cooldownAnimation as true
        skillInfo.isUsing = true;                                   // Mark the isUsing on the SkillInfo script as true
        isSkillCooldown = true;                                     // There is a cooldown
        skillImage.fillAmount = 1;                                  // Fill the Image with the cooldown Icon
        skill.GetComponent<SkillInfo>().isCooldown = true;          // Mark the isCooldown on the SkillInfo script as true
        Animator playerAnimator = player.GetComponent<Animator>();  // Get the player's Animator

        try
        {
            playerAnimator.SetInteger("Slash Number", effectAnimationNumber); // Know which skill's Animation to use
            playerAnimator.SetTrigger("UseSlash"); // Start executing the animation
        }
        catch
        {
            Debug.LogWarning("No animation found for Slash effect");
        }
    }


    /// <summary>
    /// Handles the cooldown animation for skills and abilities.
    /// </summary>
    /// <param name="isCooldown"> A reference to the cooldown status for a skill or ability. </param>
    /// <param name="skillInfo"> The SkillInfo component associated with the skill or ability </param>
    public void CoolDownAnimation1(ref bool isCooldown, SkillInfo skillInfo)
    {
        // Decrease the fill amount of the skill image based on cooldown
        skillImage.fillAmount -= 1 / cooldown * Time.deltaTime;

        if (skillImage.fillAmount <= 0)
        {
            // Reset fill amount and cooldown status when cooldown is complete
            skillImage.fillAmount = 0;
            isCooldown = false; // Update the original isCooldown variable using ref

            if (skillInfo != null)
            {
                skillInfo.isCooldown = false;
            }
        }
    }


    /// <summary>
    /// Put the effect near the player
    /// </summary>
    public void PutBeforeEffectNearPlayer()
    {
        player = player.transform;
        // Calculate the spawn position and rotation based on the player's position and forward direction
        Vector3 spawnPosition = player.position + player.forward;
        Quaternion spawnRotation = player.rotation;

        // Increase the Y position by 1 unit
        spawnPosition.y += 1f;

        // Set the position and rotation of the skill object
        skill.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
    }


    /// <summary>
    /// Activate the Skill and perform it with the right angles and position
    /// </summary>
    /// <param name="waitingTime"> Waiting time to for for the animation, so the animation fir perfectly with the skill </param>
    /// <returns></returns>
    private IEnumerator PerformSkill(float waitingTime)
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(waitingTime);

        // Set isUsing flag to false
        try
        {
            skillInfo.isUsing = false;
        }
        catch { }

        player = player.transform;

        // Calculate the spawn position and rotation based on the player's position and forward direction
        Vector3 spawnPosition = player.position + player.forward;
        Quaternion spawnRotation = player.rotation;

        // Increase the Y position by 1 unit
        spawnPosition.y += 1f;

        // Set the position and rotation of the skill object
        skill.transform.SetPositionAndRotation(spawnPosition, spawnRotation);

        // Activate the skill object
        skill.SetActive(true);


        if (skillInfo.isSlashEffect)
        {
            AudioManager.Instance.Play("SkillEffect");
            StartCoroutine(SetColliderTrue());
            StartCoroutine(SetSlashEffectToFalse());
        }
        StartCoroutine(EnableUsingOtherSkills());
        if (skillInfo.isAroundPlayer)
        {
            skill.transform.SetPositionAndRotation(spawnPosition, Quaternion.Euler(0f, 115f, 0f));
        }
        else if(skillInfo.isUpToDownSkill)
        {
            skill.transform.position = spawnPosition;
        }

        // Check if there is a collision effect
        try
        {
            if (skillInfo.collisionEffect != null)
            {
                // Activate the collision effect
                skillInfo.collisionEffect.SetActive(true);

                // Destroy the collision effect after a specified duration
                Destroy(skillInfo.collisionEffect, 4f);
            }
        }
        catch { }

        // Reset the isUsingEffect flag
        isUsingEffect = false;
    }


    /// <summary>
    /// Enable using other skills when this skill finishes
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnableUsingOtherSkills()
    {
        float timeToDisableEffect = skillInfo.slashEffectTimeToDisable;

        var timeToUseOtherSkill = timeToDisableEffect * 0.6f;
        yield return new WaitForSeconds(timeToUseOtherSkill);
        isPlayerUsingSkillNow = false;
    }


    /// <summary>
    /// Disable the Slash effect and enable using other skills when this skill finishes
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetSlashEffectToFalse()
    {
        float timeToDisableEffect = skillInfo.slashEffectTimeToDisable;

        // Wait for the specified duration
        yield return new WaitForSeconds(timeToDisableEffect);
        gameObject.transform.GetChild(1).GetComponent<Collider>().enabled = false;
        isPlayerUsingSkillNow = false;
        skill.SetActive(false);
    }


    /// <summary>
    /// Set the skill's collider to true in order to do damage to enemies
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetColliderTrue()
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(0.19f);
        gameObject.transform.GetChild(1).GetComponent<Collider>().enabled = true;
    }


    /// <summary>
    /// Save the skill keybindings data
    /// </summary>
    public void SaveSkillKeybindings()
    {
        PlayerPrefs.SetInt($"{SaveKey}_SlotID", slotID);
        PlayerPrefs.SetFloat($"{SaveKey}_Cooldown", cooldown);
        PlayerPrefs.SetInt($"{SaveKey}_IsSkillCooldown", isSkillCooldown ? 1 : 0);
        PlayerPrefs.SetInt($"{SaveKey}_IsAbilityCooldown", isAbilityCooldown ? 1 : 0);
        PlayerPrefs.SetString($"{SaveKey}_Skill", skill != null ? skill.name : "");

        PlayerPrefs.Save();
    }


    /// <summary>
    /// Load the skill keybindings data
    /// </summary>
    public void LoadSkillKeybindings()
    {
        if (PlayerPrefs.HasKey($"{SaveKey}_SlotID"))
        {
            slotID = PlayerPrefs.GetInt($"{SaveKey}_SlotID");
            cooldown = PlayerPrefs.GetFloat($"{SaveKey}_Cooldown");
            isSkillCooldown = PlayerPrefs.GetInt($"{SaveKey}_IsSkillCooldown") == 1;
            isAbilityCooldown = PlayerPrefs.GetInt($"{SaveKey}_IsAbilityCooldown") == 1;

            string skillName = PlayerPrefs.GetString($"{SaveKey}_Skill");
            skill = !string.IsNullOrEmpty(skillName) ? GameObject.Find(skillName) : null;
        }
    }

    // Getters and Setters
    public GameObject Skiill
    {
        get { return skill; }
        set { skill = value; }
    }

    public Image SkillImage
    {
        get { return skillImage; }
        set { skillImage = value; }
    }
    public float Cooldown
    {
        get { return cooldown; }
        set { cooldown = value; }
    }

    public bool IsSkillCooldown
    {
        get { return isSkillCooldown; }
        set { isSkillCooldown = value; }
    }

    public bool IsAbilityCooldown
    {
        get { return isAbilityCooldown; }
        set { isAbilityCooldown = value; }
    }

    public float EffectAnimationTime
    {
        get { return effectAnimationTime; }
        set { effectAnimationTime = value; }
    }

    public int EffectAnimationNumber
    {
        get { return effectAnimationNumber; }
        set { effectAnimationNumber = value; }
    }

    public bool CooldownAnimation
    {
        get { return cooldownAnimation; }
        set { cooldownAnimation = value; }
    }
}
