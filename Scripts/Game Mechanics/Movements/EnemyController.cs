using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    // public----------------------------------------------------------
    [SerializeField] private float lookRadius = 2f;           // Distance at which enemy starts chasing the player
    [SerializeField] private float stoppingDistance = 2f;     // Distance at which enemy stops moving towards the player
    [SerializeField] private float maxVerticalDistance = 5f;  // Maximum height difference between player and enemy for enemy to chase player
    private float currentSpeed;     // Current speed of the enemy
    private bool chase;             // Is the enemy chasing the player?
    private bool isFighting;

    private Transform target;         // Target object to follow
    private NavMeshAgent agent;       // Navigation mesh agent for movement
    private Animator animator;        // Animator for enemy animations

    private bool isChasing = false;   // Is the enemy currently chasing the p layer?

    // private----------------------------------------------------------
    private Vector3 originalPosition;        // The original position for the enemy to return to
    private RandomEnemyMovements rndMovements;
    private Enemy enemy;
    private bool useMagic;      // Does enemy use magic

    private float originalLookRadius;

    private bool isFinalBoss;   // Indicated if this is the final boss

    private bool canRift = true;    // Can the boss use the rift ability
    private bool canTeleport = true;// Can the boss teleport


    void Start()
    {
        rndMovements = GetComponent<RandomEnemyMovements>();
        animator = GetComponent<Animator>();                    // Get the animator component attached to the enemy game object
        target = FindPlayer.Instance.Player.transform;
        agent = GetComponent<NavMeshAgent>();                   // Get the navigation mesh agent component attached to the enemy game object
        originalPosition = transform.position;                  // Set the original position to the enemy's current position
        enemy = GetComponent<Enemy>();
        useMagic = GetComponent<Enemy>().GetisUseMagic();

        originalLookRadius = lookRadius;

        isFinalBoss = GetComponent<EnemyStats>().IsFinalBoss;
    }


    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);
        IsChaseingOrIdle();

        // Get the isChasing bool from the animator
        chase = animator.GetBool("isChasing");

        if(isFinalBoss)
        {
            if(distance > 25f && canRift)
            {
                canRift = false;
                StartCoroutine(RiftAbility());
                StartCoroutine(RiftCooldown());
                return;
            }
            else if(distance > 10f && distance < 15f && canTeleport)
            {
                canTeleport = false;
                TeleportAbility();
                StartCoroutine(TeleportCooldown());
            }
        }

        // Calculate the distance between the enemy and the player

        // Calculate the vertical distance between the enemy and the player
        float verticalDistance = target.position.y - transform.position.y;

        // Check if the enemy is higher than the player
        bool isHigherThanPlayer = (verticalDistance > maxVerticalDistance);

        IfChasing(distance);
        Chasing();
        IsOutOfRange(distance, isHigherThanPlayer);
        IsInRadius(distance);

        // If the look radius is bigger than the distance between the enemy and the target (Target on enemy look radius)
        if(distance < lookRadius)
        {
            FaceTarget();
            agent.SetDestination(target.position);
        }

        if (enemy.IsPlayerOnEnemyAttackRadius(useMagic))
        {
            enemy.enabled = true;
            enabled = false;
        }
    }

    private IEnumerator RiftAbility()
    {
        yield return new WaitForSeconds(0.3f);
        Teleport ability = GetComponent<Teleport>();
        ability.RiftWalker();
    }

    private IEnumerator RiftCooldown()
    {
        yield return new WaitForSeconds(8f);
        canRift = true;
    }

    private void TeleportAbility()
    {
        Teleport ability = GetComponent<Teleport>();
        StartCoroutine(ability.TeleportForward());
    }

    private IEnumerator TeleportCooldown()
    {
        yield return new WaitForSeconds(2f);
        canTeleport = true;
    }



    /// <summary>
    /// Checks if the entity is engaged in combat based on its current position and behavior.
    /// Adjusts movement and behavior settings accordingly.
    /// </summary>
    public void IsInRadius(float distance)
    {
        if(distance > lookRadius)
        {
            rndMovements.enabled = true;
            enabled = false;
            return;
        }

        // if the destination between the original position and the current position is 
        float distanceSquaredToOriginal = (originalPosition - transform.position).sqrMagnitude;
        float lookRadiusSquared = lookRadius * lookRadius;

        if (distanceSquaredToOriginal > lookRadiusSquared)
        {
            // Adjust settings for combat behavior
            if (!isFinalBoss)
            {
                lookRadius = originalLookRadius;
                rndMovements.WanderRadius = 3f;

                UpdateAnimator(false, true);

                isFighting = true;
            }
        }
        else if (IsEnemyOnHisOriginalPosition(originalPosition) && isFighting)
        {
            // Restore settings for non-combat behavior
            lookRadius = originalLookRadius;
            rndMovements.WanderRadius = 10f;
            isFighting = false;
            rndMovements.enabled = true;
            enabled = false;
        }
    }

    private void UpdateAnimator(bool isIdle, bool isChasing)
    {
        animator.SetBool("isIdle", isIdle);
        animator.SetBool("isChasing", isChasing);

    }

    /// <summary>
    /// Determines if the enemy is within the specified distance of the target or out of range.
    /// Adjusts chasing behavior based on the distance and obstruction conditions.
    /// </summary>
    /// <param name="distance">Distance between the enemy and the target.</param>
    /// <param name="isHigherThanPlayer">Indicates if the target is at a higher position than the player.</param>
    public void IsOutOfRange(float distance, bool isHigherThanPlayer)
    {
        NavMeshPath path = new();
        bool hasPath = NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);

        // If the enemy is chasing the player but the player is out of range or obstructed by an obstacle
        if ((distance > lookRadius || (isHigherThanPlayer && !hasPath)) && isChasing)
        {

            // Stop chasing and go back to idle state
            isChasing = false;

            bool isIdle = animator.GetBool("isIdle");
            UpdateAnimator(isIdle, false);

            // Stop chasing by setting the agent's destination to its current position
            agent.SetDestination(originalPosition);

        }

        // If the player is within range and not obstructed by an obstacle, and the enemy is not currently chasing
        if ((distance <= lookRadius || (isHigherThanPlayer && hasPath)) && !isChasing)
        {

            // Start chasing the player
            isChasing = true;

            UpdateAnimator(false, true);

            // Set the agent's destination to the player's position
            agent.SetDestination(target.position);
        }
    }


    /// <summary>
    /// Adjusts enemy behavior based on combat and chase conditions.
    /// Manages enemy's detection radius, vertical distance, and speed.
    /// </summary>
    public void IsChaseingOrIdle()
    {
        if (!isFighting)
        {
            lookRadius = originalLookRadius;
            maxVerticalDistance = 30f;
        }

        if (enemy.IsPlayerOnEnemyAttackRadius(useMagic))
        {
            enemy.enabled = true;
            enabled = false;
        }
        else
        {
            enemy.enabled = false;
        }

        // Get the current speed of the enemy
        currentSpeed = agent.velocity.magnitude;
    }


    /// <summary>
    /// Manages enemy behavior while chasing the player.
    /// Updates animator states and agent destinations based on distance.
    /// </summary>
    /// <param name="distance">Distance between enemy and player.</param>
    public void IfChasing(float distance)
    {
        // If the enemy is currently chasing the player
        if (isChasing)
        {
            // Set the animator's "isChasing" bool to true and "isIdle" bool to false

            UpdateAnimator(false, true);

            // Set the agent's destination to the player's position
            agent.SetDestination(target.position);

            // If the enemy is within stopping distance of the player
            if (distance <= stoppingDistance)
            {
                // Face the player
                FaceTarget();

                UpdateAnimator(true, false);
            }
            // If the agent has reached its destination and isn't trying to find a new one
            else if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                // Stop chasing the player
                isChasing = false;

                // Set the animator's "isChasing" bool to false and "isIdle" bool to true
                UpdateAnimator(true, false);
            }
        }
        else
        {
            // If the enemy is not chasing the player, set it to idle and stop moving
            bool isIdle = animator.GetBool("isIdle");
            UpdateAnimator(isIdle, false);
        }
    }


    /// <summary>
    /// Manages enemy movement while chasing the player.
    /// Updates agent destination and returns to the original destination if close to target.
    /// </summary>
    public void Chasing()
    {
        // If the enemy is chasing the player
        if (animator.GetBool("isChasing"))
        {
            agent.SetDestination(target.position);

            // Check if the agent has reached its destination with a small tolerance value
            if (agent.remainingDistance <= agent.stoppingDistance + 0.1f && !agent.pathPending)
            {
                // Stop the character from moving and return to the original destination
                agent.SetDestination(originalPosition);
            }
        }
    }


    /// <summary>
    /// Checks if the enemy is close to their original position.
    /// </summary>
    /// <param name="originalPosition">The original position of the enemy.</param>
    /// <param name="tolerance">The distance tolerance for determining if the enemy is close.</param>
    /// <returns>True if the enemy is close to their original position, otherwise false.</returns>
    public bool IsEnemyOnHisOriginalPosition(Vector3 originalPosition, float tolerance = 7f)
    {

        float distanceToOriginalSquared = (transform.position - originalPosition).sqrMagnitude;
        float toleranceSquared = tolerance * tolerance;

        if (distanceToOriginalSquared <= toleranceSquared)
        {
            return true;
        }
        else  // else if there are less than 10 units
        {
            return false;
        }
    }


    /// <summary>
    /// Rotate the enemy to face the target (Player)
    /// </summary>
    public void FaceTarget()
    {
        try
        {
            if(target == null)
            {
                target = FindPlayer.Instance.Player.transform;
            }
            // Calculate the direction to the target
            Vector3 direction = (target.position - transform.position).normalized;

            // Calculate the rotation needed to face the target and ignore rotation in y-axis
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            // Smoothly rotate the enemy towards the target
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
        catch { }
    }

    // known function----------------------------------------------------------
    private void OnDrawGizmosSelected()
    {
        // Set the color of the gizmo
        Gizmos.color = Color.red;

        // Draw a wire sphere around the enemy to represent the look radius
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    // function----------------------------------------------------------
    public void getOriginalPosition()
    {
        originalPosition = transform.position;
    }

    // function----------------------------------------------------------
    public float getLookRadius()
    {
        return this.lookRadius;
    }

    public float GetLookRadius()
    {
        return lookRadius;
    }

    public void SetLookRadius(float value)
    {
        lookRadius = value;
    }

    public float GetStoppingDistance()
    {
        return stoppingDistance;
    }

    public void SetStoppingDistance(float value)
    {
        stoppingDistance = value;
    }

    public float GetMaxVerticalDistance()
    {
        return maxVerticalDistance;
    }

    public void SetMaxVerticalDistance(float value)
    {
        maxVerticalDistance = value;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public void SetCurrentSpeed(float value)
    {
        currentSpeed = value;
    }

    public bool GetIsChasing()
    {
        return chase;
    }

    public void SetIsChasing(bool value)
    {
        chase = value;
    }

    public bool GetIsFighting()
    {
        return isFighting;
    }

    public void SetIsFighting(bool value)
    {
        isFighting = value;
    }


}
