using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerStat : CharacterStats
{
    // Character stats data
    [Header("Stats")]
    [SerializeField] private string playerName;     // Name of the player character
    [SerializeField] private int bossDamage;        // Damage done by the player to bosses
    [SerializeField] private int criticalRate;      // Critical rate of the player
    [SerializeField] private int criticalDamage;    // Critical damage multiplier of the player
                                                    
    // Level-related data                           
    [Header("Level")]                               
    [SerializeField] private int currentLevel;      // Current level of the player
                                                      
    // Experience (EXP) data                          
    [Header("EXP")]                                   
    [SerializeField] private int currentExp;        // Current amount of experience points (EXP) of the player
    [SerializeField] private int totalExp;          // Total accumulated experience points (EXP) of the player
    [SerializeField] private int requireExp;        // Experience points (EXP) required to level up
    [Space]                                         
                                                    
    // Skill-related data                           
    [SerializeField] private int skillPoints;           // Number of available skill points for the player
    [SerializeField] private int currentGold;           // Current amount of in-game currency (gold) of the player
    private int newLevelExp;                            // Experience points (EXP) accumulated towards the next level
    [SerializeField] private int currentJobNumber = 1;  // Current job number of the player
    [Space]

    // Blocking data
    [SerializeField] private bool isBlocking;   // Flag indicating if the player character is blocking

    // UI elements
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI levelText;             // UI Text element displaying the current level
    [SerializeField] private TextMeshProUGUI expRequiredText;       // UI Text element displaying required EXP for the next level
    [SerializeField] private TextMeshProUGUI currentExpText;        // UI Text element displaying current EXP of the player

    [SerializeField] private TextMeshProUGUI skillPointsText1;      // UI Text element displaying available skill points
    [SerializeField] private TextMeshProUGUI playercurrentGold;     // UI Text element displaying current amount of gold

    [SerializeField] private TextMeshProUGUI currentHealthText;     // UI Text element displaying current health of the player
    [SerializeField] private TextMeshProUGUI maxHealthText;         // UI Text element displaying maximum health of the player

    [SerializeField] private List<Quest> quest;         // List of quests for the player

    [SerializeField] private GameObject levelUpEffect;      // Effect for leveling up
    [SerializeField] private Transform wipeEffectWhenDie;   // Transition when the player dies


    private Animator wipe;

    void Start()
    {
        // Subscripe to the OnEquipmentChange function
        EquipmentManager.Instance.onEquipmentChanged += OnEquipmentChange;      // Modify the Damage and Armor

        HealthBar.Slider.value = CurrentHealth; // Setting the health value to the current health (UI)
        HealthBar.Slider.maxValue = MaxHealth;  // Setting the max health value to the current health (UI)

        ExpBar.Slider.maxValue = RequireExp;    // Setting the required exp value to the current health (UI)
        ExpBar.Slider.value = CurrentExp;       // Setting the current exo value to the current health (UI)

        try
        {
            Gold.instance.playercurrentGoldText.text = CurrentGold.ToString();  // Setting the current gold TEXT value to the current health (UI)
        }
        catch { }

        currentHealthText.text = CurrentHealth.ToString();  // Setting the health TEXT value to the current health (UI)
        maxHealthText.text = MaxHealth.ToString();          // Setting the max health TEXT value to the current health (UI)

        currentExpText.text = CurrentExp.ToString();    // Setting the required exp TEXT value to the current health (UI)
        expRequiredText.text = RequireExp.ToString();   // Setting the current exo TEXT value to the current health (UI)

        wipe = wipeEffectWhenDie.GetComponent<LevelLoader>().Transition;

        // When the scene start, do the Wipe transition
        wipe.SetTrigger("Reset");
    }


    /// <summary>
    /// When the player level up, increase the level and the required EXP for the new level
    /// And increase the skillPoints
    /// </summary>
    /// <param name="expGiver">  </param>
    public void LevelUp()
    {
        if (requireExp <= currentExp)
        {
            currentLevel++;                             // Increase the current level by one
            levelText.text = currentLevel.ToString();   // Update the UI to display the new level
            newLevelExp = currentExp - requireExp;      // Calculate EXP remaining towards the next level
            requireExp += 150;                          // Increase the EXP for the next level

            Damage.IncreaseDamage(5);

            var newMaxHealth = MaxHealth + 75;
            MaxHealth = newMaxHealth;
            maxHealthText.text = MaxHealth.ToString();
            HealthBar.Slider.maxValue = MaxHealth;

            float result = Result();
            UpdateExpUI(result);

            skillPoints += 3;
            SetskillPointsText();

            currentExp = newLevelExp;
            expRequiredText.text = requireExp.ToString();

            // Level up effect
            AudioManager.Instance.Play("LevelupEffect");

            levelUpEffect.SetActive(true);
            levelUpEffect.transform.parent = transform;
            levelUpEffect.transform.position = transform.position;
            StartCoroutine(DeactivateEffect());
        }
    }


    /// <summary>
    /// Decativate the level up effect after short duration.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeactivateEffect()
    {
        yield return new WaitForSeconds(2.5f);
        levelUpEffect.transform.parent = null;
        levelUpEffect.SetActive(false);
    }

    public void IncreaseSkillPoints(int value)
    {
        skillPoints += value;
    }

    /// <summary>
    /// Result of the percentage EXP
    /// </summary>
    /// <returns></returns>
    private float Result()
    {
        float newLevelExpPercent = (float)newLevelExp / requireExp;     // Percent exp
        int truncatedValue = (int)(newLevelExpPercent * 100);           // Multiply by 100 and convert to integer
        float result = (float)truncatedValue / 100;                     // Divide by 100 to get the first two decimals
        return result *= 100f;
    }

    /// <summary>
    /// Update the EXP bar UI
    /// </summary>
    /// <param name="result"></param>
    private void UpdateExpUI(float result)
    {
        // Update the UI to display the remaining EXP and the progress percentage
        var healthTrans = ExpBar.gameObject.transform;
        ExpBar.SetSliderBar(newLevelExp);
        ExpBar.SetMax(requireExp);
        healthTrans.GetChild(2).GetComponent<TextMeshProUGUI>().text = newLevelExp.ToString();
        healthTrans.GetChild(4).GetComponent<TextMeshProUGUI>().text = result.ToString();
    }


    /// <summary>
    /// Set the skills point value text (UI)
    /// </summary>
    /// <param name="skillPointsText"></param>
    private void SetskillPointsText()
    {
        skillPointsText1.text = skillPoints.ToString();
    }


    /// <summary>
    /// The Player dies and back to the last checkpoint
    /// </summary>
    public override void Die()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("Dead");

        DisablePlayerComponents();
        StartCoroutine(ResetScene());
    }


    /// <summary>
    /// Make the player unable to move until reseting the scene
    /// </summary>
    private void DisablePlayerComponents()
    {
        AudioManager.Instance.StopAllSounds();
        AudioManager.Instance.Play("DeathEffect");

        GetComponent<CameraRotation>().enabled = false;
        GetComponent<ThirdPersonController>().enabled = false;
        GetComponent<PlayerInput>().enabled = false;
        GetComponent<PlayerCamera>().enabled = false;
    }


    /// <summary>
    /// Reset Scene, and go to the last CheckPoint
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetScene()
    {
        yield return new WaitForSeconds(1f);

        wipe.SetTrigger("Die");
        yield return new WaitForSeconds(0.99f);

        // Get the current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Load the current scene again
        SceneManager.LoadScene(currentSceneIndex);
        StartCoroutine(CheckPoint.Load());
    }


    /// <summary>
    /// Save the player data (All the fields)
    /// </summary>
    /// <param name="path"> Unique path to save and load the player data </param>
    public void SavePlayerData(string path)
    {
        if (gameObject.activeSelf)
        {
            SaveSystem.SavePlayer(this, path);
            SaveSystem.SavePlayerTransform(this, path + SceneManager.GetActiveScene().buildIndex);

            Stats characterStats = GetComponent<CharacterStats>().Damage;
            StatsData.SaveStats(characterStats, path + "damage");
        }
    }


    /// <summary>
    /// Load the player data (All the fields)
    /// </summary>
    /// <param name="path">Unique path to save and load the player data</param>
    public void LoadPlayerData(string path)
    {
        if (gameObject.activeSelf)
        {
            PlayerData data = SaveSystem.LoadPlayer(path);

            try
            {
                PlayerTransformData a = SaveSystem.LoadPlayerTransform(path + SceneManager.GetActiveScene().buildIndex);

                Vector3 position1;

                position1.x = a.Position[0];
                position1.y = a.Position[1];
                position1.z = a.Position[2];

                gameObject.SetActive(false);
                transform.position = position1;
                gameObject.SetActive(true);
            }
            catch { }
            gameObject.SetActive(true);

            CurrentHealth = data.Health;
            MaxHealth = data.MaxHealth;

            PlayerName = data.PlayerName;
            BossDamage = data.BossDamage;
            CriticalRate = data.CriticalRate;
            CriticalDamage = data.CriticalDamage;

            CurrentLevel = data.CurrentLevel;
            CurrentExp = data.CurrentExp;
            TotalExp = data.TotalExp;
            RequireExp = data.RequireExp;

            SkillPoints = data.SkillPoints;
            skillPointsText1.text = SkillPoints.ToString();

            CurrentGold = data.CurrentGold;
            NewLevelExp = data.NewLevelExp;
            CurrentJobNumber = data.CurrentJobNumber;

            HealthBar.Slider.value = CurrentHealth;
            HealthBar.Slider.maxValue = MaxHealth;

            ExpBar.Slider.maxValue = RequireExp;
            ExpBar.Slider.value = CurrentExp;

            Gold.instance.playercurrentGoldText.text = CurrentGold.ToString();

            currentHealthText.text = CurrentHealth.ToString();
            maxHealthText.text = MaxHealth.ToString();
            currentExpText.text = CurrentExp.ToString();
            expRequiredText.text = RequireExp.ToString();

            Damage = StatsData.LoadStats(path + "damage");
        }
    }


    /// <summary>
    /// This function is called whenever the player equip or unequip items
    /// Modify the player's damage and armor
    /// </summary>
    /// <param name="newItem"> The new equipped Item </param>
    /// <param name="oldItem"> The old equipped Item </param>
    void OnEquipmentChange(Equipment newItem, Equipment oldItem)
    {
        //SetskillPointsText(8);
        if (newItem != null)
        {
            Armor.AddModifier(newItem.armorModifier);
            Damage.AddModifier(newItem.damageModifier);
        }
        if (oldItem != null)
        {
            Armor.RemoveModifier(oldItem.armorModifier);
            Damage.RemoveModifier(oldItem.damageModifier);
        }
    }


    // Getters and Setters
    public string PlayerName
    {
        get { return playerName; }
        set { playerName = value; }
    }

    public int BossDamage
    {
        get { return bossDamage; }
        set { bossDamage = value; }
    }

    public int CriticalRate
    {
        get { return criticalRate; }
        set { criticalRate = value; }
    }

    public int CriticalDamage
    {
        get { return criticalDamage; }
        set { criticalDamage = value; }
    }

    public int CurrentLevel
    {
        get { return currentLevel; }
        set { currentLevel = value; }
    }

    public int CurrentExp
    {
        get { return currentExp; }
        set { currentExp += value; }
    }

    public int TotalExp
    {
        get { return totalExp; }
        set { totalExp += value; }
    }

    public int RequireExp
    {
        get { return requireExp; }
        set { requireExp = value; }
    }


    public int SkillPoints
    {
        get { return skillPoints; }
        set { skillPoints = value; }
    }

    public int DecreaseSkillPoints
    {
        get { return skillPoints; }
        set { skillPoints -= value; }
    }

    public int CurrentGold
    {
        get { return currentGold; }
        set { currentGold += value; }
    }

    public int NewLevelExp
    {
        get { return newLevelExp; }
        set { newLevelExp = value; }
    }

    public int CurrentJobNumber
    {
        get { return currentJobNumber; }
        set { currentJobNumber = value; }
    }

    public bool IsBlocking
    {
        get { return isBlocking; }
        set { isBlocking = value; }
    }


    public TextMeshProUGUI ExpRequiredText
    {
        get { return expRequiredText; }
        set { expRequiredText = value; }
    }

    public List<Quest> Quest
    {
        get { return quest; }
        set { quest = value; }
    }
}
