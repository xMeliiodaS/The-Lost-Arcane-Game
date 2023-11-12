using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : CharacterStats
{
    [SerializeField] private CurrnetEnemyType currnetEnemyType;     // Enemy type (Use for quests)
    [SerializeField] private bool isEnemyBoss;                      // If the enemy is boss

    [SerializeField, Tooltip("The value is by percentage (1 is 100%)")]
    private float enrageHP;     // Enrage when this object' HP is lower than enrageHP*100

    private bool isEnraged;     // If the boss is enraged (More powerful)
    private Animator animator;  // Reference to the animator

    private readonly int currentExpChildNum = 2;
    private readonly int percentExpChildNum = 4;

    [SerializeField] private bool isFinalBoss;

    // Items that the boss drops when defeated
    [Header("Boss Drop Loot")]
    [SerializeField] private GameObject arcane;
    [SerializeField] private GameObject maskArmor;
    [SerializeField] private GameObject shoulderArmor;
    [SerializeField] private GameObject weaponArmor;
    [SerializeField] private GameObject pendantArmor;
    [SerializeField] private GameObject beltArmor;
    [SerializeField] private GameObject ringArmor;
    [SerializeField] private GameObject healingPotions;

    [SerializeField] private GameObject droppedKey;


    /// <summary>
    /// When the HP of the boss is less than the enrageHP, it becomes more powerful
    /// </summary>
    public void Enraged()
    {
        // If the health is (X)% or lower -> Enrage
        if (CurrentHealth <= enrageHP * MaxHealth && !isEnraged)
        {
            animator = GetComponent<Animator>();
            isEnraged = true;
            animator.SetTrigger("Enraged");
            animator.speed = 1.19f;

            if(isFinalBoss)
            {
                Instantiate(droppedKey, transform.position, Quaternion.identity).SetActive(true);
            }
            
        }
    }


    /// <summary>
    /// The character dies and disappear after 4 seconds
    /// </summary>
    public override void Die()
      {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("Dead");

        UpdatePlayerStats();        
        DropItems();                
        DisableEnemyComponents();

        DropHealingPotions(1, 0.4f);
        Destroy(gameObject, 4f);
     }


    /// <summary>
    /// Update the player stats (exp)
    /// </summary>
    private void UpdatePlayerStats()
    {
        // Find the player's PlayerStat so we can increase the currentExp property
        PlayerStat playerStat = FindObjectOfType<PlayerStat>();

        playerStat.TotalExp = ExpGiver;     // Increase the total EXP
        playerStat.CurrentExp = ExpGiver;   // Add to the player's EXP more EXP

        UpdatePlayerStatsUI.Instance.UpdateStatsAndUI(playerStat, ExpGiver);

        HealthBar.gameObject.SetActive(false);
    }


    /// <summary>
    /// If the enemy is Boss he drops loot for the next scene
    /// </summary>
    void DropItems()
    {
        // If the character is a boss
        if (gameObject.name.Contains("Boss"))
        {
            DropBossItems();
            DropHealingPotions(Random.Range(1, 4), 1);
        }
    }


    /// <summary>
    /// Make the enemy unable to move and remove the collider
    /// </summary>
    private void DisableEnemyComponents()
    {
        // Disable the movements for the enemy
        try
        {   // Not all enemies have Box Collider Component
            GetComponent<BoxCollider>().enabled = false;
        }
        catch 
        { 
            Debug.LogWarning("No BoxCollider component found");
        }

        try
        {   // Not all enemies have Character Controller Component
            GetComponent<CharacterController>().enabled = false;
        }
        catch 
        { 
            Debug.LogWarning("No CharacterController component found");
        }

        try
        {
            GetComponent<BossEffect>().enabled = false;
        }
        catch 
        {
            Debug.LogWarning("No BossEffect component found");
        }

        GetComponent<Enemy>().enabled = false;
        GetComponent<RandomEnemyMovements>().enabled = false;
        GetComponent<EnemyController>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;

    }


    /// <summary>
    /// activate boss-specific items when it dies
    /// </summary>
    private void DropBossItems()
    {
        InstantiateAndActivateItem(arcane);
        InstantiateAndActivateItem(maskArmor);
        InstantiateAndActivateItem(shoulderArmor);
        InstantiateAndActivateItem(weaponArmor);
        InstantiateAndActivateItem(pendantArmor);
        InstantiateAndActivateItem(beltArmor);
        InstantiateAndActivateItem(ringArmor);
    }


    /// <summary>
    /// Helper function to instantiate and activate an item
    /// </summary>
    /// <param name="itemPrefab"> The Instantiated item </param>
    private void InstantiateAndActivateItem(GameObject itemPrefab)
    {
        // Calculate a random offset for the X and Z coordinates within the range of -2.5 to 2.5
        float xOffset = Random.Range(-2.5f, 2.5f);
        float zOffset = Random.Range(-2.5f, 2.5f);
        Vector3 randomPos1 = new(transform.position.x + xOffset, transform.position.y, transform.position.z + zOffset);

        // Instantiate the item at the entity's position
        itemPrefab.SetActive(true);
        itemPrefab.transform.SetPositionAndRotation(randomPos1, Quaternion.identity);
    }


    /// <summary>
    /// Helper function to drop random quantity of healing potions
    /// </summary>
    /// <param name="quantity"> The potion's quantity </param>
    private void DropHealingPotions(int quantity, float spawnChance)
    {
        try
        {
            // Instantiate healing potions at the entity's position based on the given quantity
            for (int i = 0; i < quantity; i++)
            {
                // Generate a random value between 0 and 1
                float randomValue = Random.value;

                // Check if the random value is less than or equal to the spawnChance
                if (randomValue <= spawnChance)
                {
                    Instantiate(healingPotions, transform.position, Quaternion.identity);
                }
            }
        }
        catch { }
    }


    public CurrnetEnemyType CurrentEnemyType
    {
        get { return currnetEnemyType; }
        set { currnetEnemyType = value; }
    }

    public bool IsEnemyBoss
    {
        get { return isEnemyBoss; }
        set { isEnemyBoss = value; }
    }

    public bool IsEnraged
    {
        get { return isEnraged; }
        set { isEnraged = value; }
    }
    public bool IsFinalBoss
    {
        get { return isFinalBoss; }
        set { isFinalBoss = value; }
    }
}
public enum CurrnetEnemyType
{
    Goblin,
    Dragon,
    Skeleton
}
