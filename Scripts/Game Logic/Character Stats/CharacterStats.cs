using System.Collections;
using TMPro;
using UnityEngine;

public abstract class CharacterStats : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;               // Maximum health of the character
    [SerializeField] private int currentHealth;                 // The current health of this object

    // Stats for dealing damage and reducing damage received.
    [SerializeField] private Stats damage;                      // Current character damage
    [SerializeField] private Stats armor;                       // Current character defense

    // HealthBar UI components for displaying health status.
    [SerializeField] private HealthBar healthBar;               // Reference to the character healthBar component
    [SerializeField] private HealthBar playerHealthBar;         // Reference to the player healthBar component
    [SerializeField] private HealthBar expBar;                  // Reference to the player EXPBar

    private GameObject questProgress;                           // Reference to the quest progress GameObject 

    private readonly int healthChild = 2;       // Child index for the health display in the UI.
    private readonly int maxhealthChild = 4;    // Child index for the maximum health display in the UI.
    [Space]

    [SerializeField] private int expGiver;      // Experience points given when this enemy is defeated
    protected bool isDead = false;          // Is this gameobject dead

    private Animator animator;
    private void Awake()
    {
        currentHealth = maxHealth;
        healthBar.SetMax(maxHealth);
        playerHealthBar.SetSliderBar(maxHealth);
    }


    private void Start()
    {
        questProgress = GameObject.FindWithTag("QuestProgress");
        animator = GetComponent<Animator>();
    }

    // An abstract method for Die()
    public abstract void Die();


    /// <summary>
    /// When the character (Player or Enemy) takes damage
    /// </summary>
    /// <param name="damage"> Taken damage </param>
    /// <param name="isCritical"> If the hit was Critical </param>
    public void TakeDamage(int damage, bool isCritical = false)
     {
        if(!isDead)
        {
            TakeDamageCalculation(ref damage, armor);
            CreateEnemyPopup(damage, isCritical);


            if (currentHealth <= 0)
            {
                isDead = true;
                CheckQuest();
                Die();
            }
        }
    }


    /// <summary>
    /// Reset the trigger if didnt execute the animation within 0.6s
    /// </summary>
    /// <param name="animator"> The target Animator </param>
    /// <returns></returns>
    IEnumerator ResetHitTrigger(Animator animator)
    {
        yield return new WaitForSeconds(0.6f); // Wait for one second
        animator.ResetTrigger("isHit"); // Reset the "isHit" trigger
    }


    /// <summary>
    /// Main function to calculate and apply damage
    /// </summary>
    /// <param name="damage"> Taken Damage </param>
    /// <param name="armor"> Defense to reduce Taken Damage </param>
    private void TakeDamageCalculation(ref int damage, Stats armor)
    {
        animator = GetComponent<Animator>();
        animator.ResetTrigger("isHit");

        PlayerStat playerStat = FindObjectOfType<PlayerStat>();
        bool isPlayer = this is PlayerStat;

        if (playerStat.IsBlocking && isPlayer)
        {
            ApplyBlockedDamage(ref damage, armor);
        }
        else if(!playerStat.IsBlocking)
        {
            ApplyUnblockedDamage(ref damage, armor);
        }

        healthBar.SetSliderBar(currentHealth);

        if (isPlayer)
        {
            playerHealthBar.transform.GetChild(healthChild).GetComponent<TextMeshProUGUI>().text = playerStat.currentHealth.ToString();
            playerHealthBar.transform.GetChild(maxhealthChild).GetComponent<TextMeshProUGUI>().text = playerStat.maxHealth.ToString();
        }
    }


    /// <summary>
    /// Helper function to apply damage when the player is blocking the attack
    /// </summary>
    /// <param name="damage"> Taken Damage </param>
    /// <param name="armor"> Defense to reduce Taken Damage </param>
    /// <param name="playerStat"> Reference to the player stats </param>
    private void ApplyBlockedDamage(ref int damage, Stats armor)
    {
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        currentHealth -= (int)(damage * 0.5f);
    }


    /// <summary>
    /// Helper function to apply damage when the player is not blocking
    /// </summary>
    /// <param name="damage"> Taken Damage </param>
    /// <param name="armor"> Defense to reduce Taken Damage </param>
    private void ApplyUnblockedDamage(ref int damage, Stats armor)
    {
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);
        currentHealth -= damage;

        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("isHit");

        if (Random.value <= 0.3)
        {
            AudioManager.Instance.Play("GetHitEffect");
        }

        StartCoroutine(ResetHitTrigger(animator));

        if (this is EnemyStats stats && stats.IsEnemyBoss)
        {
            stats.Enraged();
        }
    }


    /// <summary>
    /// When damaging enemies a popup of the damage amount will appear
    /// </summary>
    /// <param name="damage"> Taken Damage </param>
    /// <param name="isCritical"> If the hit was Critical </param>
    private void CreateEnemyPopup(int damage, bool isCritical)
    {
        if (!transform.gameObject.CompareTag("Player"))
        {
            Vector3 randomness = new(Random.Range(0f, 0.25f), Random.Range(0f, 0.25f), Random.Range(0f, 0.25f));
            Vector3 popupPosition = transform.position + new Vector3(0f, 1f, 0f); // Adjust the Y-coordinate (2f) as needed
            
            // If the hit was not critical
            if (!isCritical)
            {
                DamagePopup.GetInstance().CreatePopup(popupPosition + randomness, damage.ToString(), Color.yellow);
            }
            else // If the hit was critical
            {
                DamagePopup.GetInstance().CreatePopup(popupPosition + randomness, damage.ToString(), Color.red);
            }
        }

    }

    /// <summary>
    /// When killing enemy check if the current quest is to kill this type of enemies
    /// </summary>
    private void CheckQuest()
    {
        if (this is not PlayerStat)
        {
            PlayerStat playerStat = FindObjectOfType<PlayerStat>();      // Find the PlayerStat component
            if (playerStat != null)
            {
                // Check if any active quests are related to this character
                foreach (Quest quest in playerStat.Quest)
                {
                    CheckQuestProgress(quest, playerStat);
                }
            }
        }
    }


    /// <summary>
    /// Check the quest progress
    /// </summary>
    /// <param name="quest"> Current quest </param>
    /// <param name="playerStat"> Reference to the player stats </param>
    private void CheckQuestProgress(Quest quest, PlayerStat playerStat)
    {
        EnemyType enemyType = quest.Kill.EnemyType1;
        CurrnetEnemyType currnetEnemyType = gameObject.GetComponent<EnemyStats>().CurrentEnemyType;

        if (quest.IsActive && enemyType.ToString() == currnetEnemyType.ToString())
        {
            quest.Kill.EnemyKilled();

            try
            {
                AddKillsCount(quest);
            }
            catch { }

            QuestComplete(quest, playerStat);
        }
    }


    /// <summary>
    /// Add the kills amount to the quest progress
    /// </summary>
    /// <param name="quest"> Current quest </param>
    private void AddKillsCount(Quest quest)
    {
        int current = quest.Kill.currentAmount;
        int required = quest.Kill.requiredAmount;
        if(questProgress == null)
        {
            questProgress = GameObject.FindWithTag("QuestProgress");
        }

        var killsCount = questProgress.transform.GetChild(0);
        killsCount.gameObject.SetActive(true);

        var killsCounter = killsCount.GetComponent<TextMeshProUGUI>();

        killsCounter.text = current.ToString();
        killsCounter.text += " / ";
        killsCounter.text += required.ToString();
        killsCounter.text += " " + RemoveDuplicateSuffix(gameObject.name);

        AudioManager.Instance.Play("QuestProgressEffect");

        StartCoroutine(DeactivateTheKillCounter(killsCount));
    }


    /// <summary>
    /// Remove the enemy name "(x)"
    /// </summary>
    /// <param name="name"></param>
    /// <returns> Return the new name after removing the (X) </returns>
    private string RemoveDuplicateSuffix(string name)
    {
        // Check if the name contains the duplicate suffix "(x)"
        int duplicateSuffixIndex = name.LastIndexOf('(');
        if (duplicateSuffixIndex != -1)
        {
            // Remove the duplicate suffix from the name
            name = name[..duplicateSuffixIndex].TrimEnd();
        }
        return name;
    }


    /// <summary>
    /// After 2s deactivate the kill progress counter
    /// </summary>
    /// <param name="killsCount"> Kill counter ( killsCount / requiredAmount ) </param>
    /// <returns></returns>
    IEnumerator DeactivateTheKillCounter(Transform killsCount)
    {
        yield return new WaitForSeconds(2f);
        killsCount.gameObject.SetActive(false);
    }


    /// <summary>
    /// If the quest complete give the player EXP
    /// </summary>
    /// <param name="quest"> Current Quest </param>
    /// <param name="playerStat"> Reference to the player's stats </param>
    private void QuestComplete(Quest quest, PlayerStat playerStat)
    {
        if (quest.Kill.isReached() && quest.Gathering.requiredAmount >= 0 && quest.Gathering.isReached())
        {
            quest.Complete(playerStat);
            AudioManager.Instance.Play("VictoryQuestEffect");

            UpdatePlayerStatsUI.Instance.UpdateStatsAndUI(playerStat, expGiver);
        }
    }


    /// <summary>
    /// Helper function to calculate the percentage with two decimal places
    /// </summary>
    /// <param name="currentExp"> currentExp </param>
    /// <param name="requierdExp"> requierdExp </param>
    /// <returns></returns>
    public float CalculatePercentageWithTwoDecimals(int currentExp, int requierdExp)
    {
        float percentExp = (float)currentExp / requierdExp;
        int truncatedValue = (int)(percentExp * 100);
        float result = (float)truncatedValue / 100;
        result *= 100f;
        return result;
    }


    public int MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    public int CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    public Stats Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    public Stats Armor
    {
        get { return armor; }
        set { armor = value; }
    }

    public HealthBar HealthBar
    {
        get { return healthBar; }
        set { healthBar = value; }
    }

    public HealthBar ExpBar
    {
        get { return expBar; }
        set { expBar = value; }
    }

    public int ExpGiver
    {
        get { return expGiver; }
        set { expGiver = value; }
    }
}