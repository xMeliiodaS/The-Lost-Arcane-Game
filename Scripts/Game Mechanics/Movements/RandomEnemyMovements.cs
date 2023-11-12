using UnityEngine;
using UnityEngine.AI;

public class RandomEnemyMovements : MonoBehaviour
{
    [SerializeField] private float wanderRadius = 2f;                 // Radius within which the enemy can randomly move
    [SerializeField] private float wanderTimer = 5f;                  // Time interval between each random movement
    [SerializeField] private float changePositionInterval = 10f;      // Time interval after which the enemy should change its random movement destination

    private NavMeshAgent agent;                     // Reference to the NavMeshAgent component
    private float timer;                            // Timer for wanderTimer
    private float changePositionTimer;              // Timer for changePositionInterval

    private EnemyController enemyController;        // Reference to the EnemyController script
    private RandomEnemyMovements rndMovements;      // Reference to the RandomEnemyMovements script
    private Enemy enemy;

    private Transform player;
    private EnemyController radius;
    private Vector3 originalPosition;

    // Start is called before the first frame update
    private void Start()
    {
        radius = GetComponent<EnemyController>();   
        player = FindPlayer.Instance.Player.transform;

        rndMovements = this; // Get the RandomEnemyMovements component
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component

        wanderTimer = Random.Range(3, 8);
        changePositionInterval = Random.Range(2, 5);
        timer = wanderTimer; // Set the initial value of timer to wanderTimer
        changePositionTimer = changePositionInterval; // Set the initial value of changePositionTimer to changePositionInterval

        enemyController = GetComponent<EnemyController>(); // Get the EnemyController component
        enemy = GetComponent<Enemy>();

        originalPosition = transform.position;
    }

    

    private void Update()
    {
        player = FindPlayer.Instance.Player.transform;
        wanderRadius = 10f;

        float distanceToPlayerSquared = (transform.position - player.position).sqrMagnitude;
        float rangeToAttackSquared = enemy.GetRangeToAttack() * enemy.GetRangeToAttack();

        if (distanceToPlayerSquared <= rangeToAttackSquared)
        {
            enemy.ChangeAnimaions(false, false, true);
            return;
        }


        // if the player is on the enemy look radius
        float lookRadiusSquared = radius.getLookRadius() * radius.getLookRadius();

        if (distanceToPlayerSquared <= lookRadiusSquared)
        {
            IsPlayerOnEnemyLookRadius();
        }
        else // if the player is not on the enemy look radius
        {
            PlayerNotInEnemyRadius();
        }
            
        changePositionTimer += Time.deltaTime;

        if (changePositionTimer >= changePositionInterval)
        {
            // If the player has not been detected, enable random movement and increment the change position timer
            agent.SetDestination(RandomNavmeshLocation(wanderRadius)); // Set a new random destination within wanderRadius
            changePositionTimer = 0f; // Reset the timer
        }
        

        // Decrement the wander timer and check if it has reached 0
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            agent.SetDestination(RandomNavmeshLocation(wanderRadius)); // Set a new random destination within wanderRadius
            timer = wanderTimer; // Reset the timer
        }

        // the distance between where is the enemy now and and where hos original position
        float distanceFromStartingPositionSquared = (transform.position - originalPosition).sqrMagnitude;
        float wanderRadiusSquared = wanderRadius * wanderRadius;

        if (distanceFromStartingPositionSquared > wanderRadiusSquared)
        {
            agent.SetDestination(originalPosition);  // set the enemy's destination to his original position
        }

    }

    public void IsPlayerOnEnemyLookRadius()
    {
        enemyController = GetComponent<EnemyController>(); // Get the EnemyController component
        enemy = GetComponent<Enemy>();
        rndMovements = GetComponent<RandomEnemyMovements>();

        enemy.ChangeAnimaions(false, false, true);

        enemyController.enabled = true;     // enable the EnemyController class
        rndMovements.enabled = false;       //  disable this class
        enemy.enabled = true;               // enable the Enemy class
    }

    public void PlayerNotInEnemyRadius()
    {
        enemyController = GetComponent<EnemyController>(); // Get the EnemyController component
        enemy = GetComponent<Enemy>();
        rndMovements = this;
        rndMovements.enabled = true;
        enemyController.enabled = false;
        enemy.enabled = false;

        agent = GetComponent<NavMeshAgent>();

        // Check if the enemy is not moving
        if (agent.velocity.magnitude <= 0.1f)
        {
            enemy.ChangeAnimaions(true, false, false);
        }
        else
        {
            enemy.ChangeAnimaions(false, true, false);
        }
    }



    // Generates a random location within the specified radius and returns the NavMesh path
    private Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, radius, -1);
        return navHit.position;
    }

    private void OnDrawGizmosSelected()
    {
        // Set the color of the gizmo
        Gizmos.color = Color.green;

        // Draw a wire sphere around the enemy to represent the look radius
        Gizmos.DrawWireSphere(transform.position + Vector3.up, wanderRadius);

    }

    public float WanderRadius
    {
        get { return wanderRadius; }
        set { wanderRadius = value; }
    }
}