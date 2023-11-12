using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float rangeToChasePlayer;  // The distance at which the enemy starts chasing the player
    [SerializeField] private float rangeToAttack;       // The distance at which the enemy can attack the player

    private Animator animator;                      // Reference to the enemy's Animator component
    private RandomEnemyMovements rndMovements;      // Reference to the random movement script for the enemy
    private EnemyController enemyController;        // Reference to the main enemy controller script

    [SerializeField] private float pushBackForce = 10f;     // Force applied to push the enemy away from an object/player
    [SerializeField] private float distanceThreshold = 3f;  // Minimum distance for actions between enemy and player

    [SerializeField] private bool isUseMagic;           // Indicates if the enemy uses magic attacks
    [SerializeField] private GameObject magicEffect;    // The visual effect for magic attacks
    [SerializeField] private float waitSecToEffect;     // Time to wait before applying magic effect

    private bool isAttacking = false;           // Flag indicating if the enemy is currently attacking
    private Transform player;                   // Reference to the player's Transform
    private string randomAttack;                // The type of random attack to perform
    private bool isCooldown = false;            // Flag indicating if the attack is on cooldown
    private bool isEffectCooldown = false;      // Flag indicating if effect is on cooldown

    [SerializeField] private float attackCooldown = 4.5f;           // Delay between attacks
    [SerializeField] private List<GameObject> bossEffect;           // List of boss effects for special attacks
    [SerializeField] private float effectCooldownDuration = 10f;    // Cooldown duration for boss effects
        
    private bool isEnemyBoss;       // Flag indicating if the enemy is a boss
    private bool isFinalBoss;

    [Tooltip("Minimum inclusive Attack number. Every enemy type has different Attack Animation, and this value indicates which " +
        "Attack animation to use.")]
    [SerializeField] private int minAttackRange;

    [Tooltip("Maximum inclusive Attack number. Every enemy type has different Attack Animation, and this value indicates which " +
        "Attack animation to use.")]
    [SerializeField] private int maxAttackRange = 2;

    public readonly float tooCloseDistance = 1.0f; // The distance at which the enemy considers the player too close
    public readonly float moveBackDistance = 0.5f; // The distance the enemy moves back when the player is too close
    public readonly float moveBackSpeed = 1.0f; // Speed at which the enemy moves back
    private Vector3 targetPosition;



    // Start is called before the first frame update
    void Start()
    {
        player = FindPlayer.Instance.Player.transform;
        animator = GetComponent<Animator>();
        enemyController = GetComponent<EnemyController>();
        rndMovements = GetComponent<RandomEnemyMovements>();
        isEnemyBoss = gameObject.GetComponent<EnemyStats>().IsEnemyBoss;

        isFinalBoss = GetComponent<EnemyStats>().IsFinalBoss;

    }


    /// <summary>
    /// This method is called when the object is enabled.
    /// It checks if the enemy is a boss and adjusts its speed accordingly.
    /// </summary>
    private void OnEnable()
    {
        isEnemyBoss = gameObject.GetComponent<EnemyStats>().IsEnemyBoss;
        if (isEnemyBoss)
        {
            NavMeshAgent agentRef = GetComponent<NavMeshAgent>();
            agentRef.speed = 6.5f;
        }
    }


    /// <summary>
    /// Checks for various conditions such as boss enrage state, player proximity,
    /// attack cooldowns, and triggers appropriate actions based on those conditions.
    /// </summary>
    void Update()
    {
        animator = GetComponent<Animator>();
        enemyController.FaceTarget();

        // Check if the enemy is enraged
        var isEnraged = GetComponent<EnemyStats>().IsEnraged;

        if (isFinalBoss)
        {
            if(!isEffectCooldown)
            {
                IsPlayerOnEnemyAttackRadius(isUseMagic);
                //InstantiateRandomBossEffect(false);
                StartCooldown();
            }
        }
        // Check if the enemy is not a boss, or if it's not enraged or the effect cooldown is active
        if ((!isEnemyBoss) || (!isEnraged || isEffectCooldown))
        {
            IsPlayerOnEnemyAttackRadius(isUseMagic);
        }
        // Check if the enemy is enraged and the effect cooldown is not active
        else if (isEnraged && !isEffectCooldown)
        {
            IsPlayerOnEnemyAttackRadius(isUseMagic);
            //InstantiateRandomBossEffect(false);
            StartCooldown();
        }

        // For magic enemies
        // Check if the enemy can use magic and is not an enemy boss
        else if (isUseMagic && !isEnemyBoss)
        {
            IsPlayerOnEnemyAttackRadius(isUseMagic);
            StartCooldown();
        }
        // Check if the enemy can use magic and is an enemy boss
        else if (isUseMagic && isEnemyBoss)
        {
            IsPlayerOnEnemyAttackRadius(isUseMagic);
            //InstantiateRandomBossEffect(true);
            StartCooldown();
        }

        PushEnemyAwayIfTooClose();
    }


    /// <summary>
    /// Coroutine to reset the attacking animation parameters after a short delay.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetAttackingAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.1f);
        animator.SetBool("isAttacking", false);
        animator.ResetTrigger("Attack0");
        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");
    }


    /// <summary>
    /// Instantiate a random boss effect from the bossEffect list and activate it.
    /// </summary>
    /// <param name="useMagic">Flag indicating if the boss should use magic effects.</param>
    private void InstantiateRandomBossEffect(bool useMagic)
    {
        if (bossEffect.Count == 0)
        {
            Debug.LogWarning("The bossEffect list is empty. Add GameObjects to the list.");
            return;
        }

        // Calculate forward offset for effect placement
        Vector3 forwardOffset = transform.forward * 0.3f;

        // Generate a random index within the range of the bossEffect list
        int randomIndex = Random.Range(0, bossEffect.Count);

        // Play animation corresponding to the chosen effect
        AnimationForEffect(bossEffect[randomIndex].name);

        // Instantiate the selected boss effect
        GameObject randomBossEffect = Instantiate(bossEffect[randomIndex], transform.position + forwardOffset + new Vector3(0f, 1f, 0f), Quaternion.identity);

        StartCoroutine(ActivateEffect(randomBossEffect));

        StartCoroutine(DeactivateEffect(randomBossEffect));

        // Set the boss effect as a child of the enemy
        randomBossEffect.transform.SetParent(transform);
    }


    /// <summary>
    /// Play a corresponding animation trigger based on the given effect name.
    /// </summary>
    /// <param name="effectName">Name of the effect to determine the animation.</param>
    private void AnimationForEffect(string effectName)
    {
        switch (effectName)
        {
            case "Sword Slash 6": animator.SetTrigger("Spin");
                break;

            case "Spatial section":
                animator.SetTrigger("Spin");
                break;

            case "Prick 1":
                animator.SetTrigger("Prick");
                break;

            case "Sword Slash 1":
                animator.SetTrigger("Slash");
                break;

            default:
                animator.SetTrigger("");
                break;

        }
    }


    /// <summary>
    /// Start the cooldown period for the boss effect activation.
    /// </summary>
    private void StartCooldown()
    {
        isEffectCooldown = true;
        StartCoroutine(ResetCooldown1());
    }


    /// <summary>
    /// Coroutine to reset the cooldown state for boss effect activation after a specified duration.
    /// </summary>
    private IEnumerator ResetCooldown1()
    {
        yield return new WaitForSeconds(effectCooldownDuration);
        isEffectCooldown = false;
    }


    /// <summary>
    /// Coroutine to activate a boss effect after a short delay.
    /// </summary>
    /// <param name="randomBossEffect">The GameObject representing the boss effect.</param>
    private IEnumerator ActivateEffect(GameObject randomBossEffect)
    {
        yield return new WaitForSeconds(0.6f);
        randomBossEffect.SetActive(true);

        try
        {
            randomBossEffect.GetComponent<CapsuleCollider>().enabled = true;
        }
        catch 
        { 
            Debug.LogWarning("No capsule collider found"); 
        }
    }


    /// <summary>
    /// Coroutine to deactivate a boss effect after a delay.
    /// </summary>
    /// <param name="randomBossEffect">The GameObject representing the boss effect.</param>
    private IEnumerator DeactivateEffect(GameObject randomBossEffect)
    {
        yield return new WaitForSeconds(3f);

        randomBossEffect.SetActive(false);

        try
        {
            randomBossEffect.GetComponent<CapsuleCollider>().enabled = false;
        }
        catch 
        { 
            Debug.LogWarning("No capsule collider found"); 
        }
    }


    /// <summary>
    /// Initiates an attack based on the specified parameters.
    /// </summary>
    /// <param name="randomAttack">The name of the random attack to perform.</param>
    /// <param name="useMagic">Indicates if the attack uses magic.</param>
    public void Attack(string randomAttack, bool useMagic)
    {
        if (isCooldown)
        {
            return;
        }

        // Set the cooldown and attacking flags
        isCooldown = true;
        isAttacking = true;

        // Disable enemy controller and random movements
        enemyController.enabled = false;
        rndMovements.enabled = false;

        // Change animations based on attack type
        ChangeAnimaions(false, false, false);

        // Perform regular attack
        if (!useMagic)
        {
            animator.SetTrigger(randomAttack);
        }
        // Perform magic attack
        else
        {
            animator.SetTrigger("Attack");
            StartCoroutine(DoEffect());
        }

        // Reset attacking animation and cooldown
        StartCoroutine(ResetAttackingAnimation());
        StartCoroutine(ResetCooldown());

        // Disable root motion during attack
        animator.applyRootMotion = false;
    }


    /// <summary>
    /// Initiates a magical effect for the boss attack based on height difference with the player.
    /// </summary>
    private IEnumerator DoEffect()
    {
        yield return new WaitForSeconds(waitSecToEffect);

        // Calculate the offset in the forward direction of the enemy
        Vector3 forwardOffset = transform.forward * 0.35f;
        GameObject effect;

        // Check the height difference between the enemy and the player
        float heightDifference = transform.position.y - player.TransformPoint(Vector3.zero).y;


        // Check if the enemy is higher or lower than the player
        if (heightDifference > 2f)
        {
            // Enemy is higher than the player, set the x rotation of the magic effect to 40
             effect = Instantiate(magicEffect, transform.position + new Vector3(0f, 1f, 0f) + forwardOffset, transform.rotation * Quaternion.Euler(15, 0f, 0f));
        }
        else if (heightDifference < -2f)
        {
            // Enemy is lower than or at the same height as the player, set the x rotation of the magic effect to -40
            effect = Instantiate(magicEffect, transform.position + new Vector3(0f, 1.3f, 0f) + forwardOffset, transform.rotation * Quaternion.Euler(-30, 0f, 0f));
        }
        else
        {
            effect = Instantiate(magicEffect, transform.position + new Vector3(0f, 1f, 0f) + forwardOffset, transform.rotation);
        }

        // Get the component to Assign the skill to which enemy
        var attributes = effect.transform.GetChild(0).GetChild(0).GetComponent<WeaponAttributesForEnemy>();
        attributes.SetATM(gameObject.GetComponent<CharacterStats>());

        effect.SetActive(true);                     // Activate the effect
        StartCoroutine(DeactivateEffect(effect));   // Dectivate the effect after some time
    }


    /// <summary>
    /// Initiates the chasing behavior of the enemy.
    /// </summary>
    public void Chase()
    {
        isAttacking = false;

        ChangeAnimaions(false, false, true);

        animator.SetBool("isAttacking", false);
        enemyController.enabled = true;
        enabled = false;
    }


    /// <summary>
    /// Resets the attack cooldown after a specified duration.
    /// </summary>
    /// <returns>IEnumerator coroutine</returns>
    IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isCooldown = false;
    }


    /// <summary>
    /// Checks if the player is within the boss's attack radius and triggers appropriate actions.
    /// </summary>
    /// <param name="useMagic">Flag to determine if a magic attack is being used.</param>
    /// <returns>True if the player is within attack radius, otherwise false.</returns>
    public bool IsPlayerOnEnemyAttackRadius(bool useMagic)
    {
        if(player == null)
        {
            player = FindPlayer.Instance.Player.transform;
        }

        Vector3 playerPosition = player.transform.position; // The current position of the player

        // Calculate the distance between the enemy and the player
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);
        if (distanceToPlayer <= rangeToAttack && !isAttacking)
        {
            if(!isCooldown)
            {
                if (!useMagic)
                {
                    randomAttack = GetRandomAttack();
                    Attack(randomAttack, useMagic);
                }
                else if(useMagic && !isEnemyBoss) // Else if magic attack
                {
                    Attack(randomAttack, useMagic);
                }
                else if(useMagic && isEnemyBoss)
                {
                    Attack(randomAttack, useMagic);
                    MagicEffectBoss();
                }
            }

            // If there is a cooldown
            else if(isCooldown && distanceToPlayer < rangeToAttack)
            {
                ChangeAnimaions(true, false, false);
            }

            // If the player far from the enemy, chase him
            if (distanceToPlayer > rangeToAttack)
            {
                ChangeAnimaions(false, false, true);
            }

            return true;
        }
        else
        {
            Chase();
            return false;
        }
    }


    /// <summary>
    /// Triggers a magic effect for the boss, applying visual and cooldown effects.
    /// </summary>
    private void MagicEffectBoss()
    {
        isCooldown = true;
        // Generate a random index within the range of the bossEffect list
        int randomIndex = Random.Range(0, bossEffect.Count);

        // Instantiate the random bossEffect GameObject
        Vector3 forwardOffset = transform.forward * 0.4f;

        //GameObject randomBossEffect = Instantiate(bossEffect[randomIndex], transform.position, Quaternion.identity);
        GameObject randomBossEffect = Instantiate(bossEffect[randomIndex], transform.position + new Vector3(0f, 1f, 0f) + forwardOffset, transform.rotation);

        animator.SetTrigger("Cast");
        StartCoroutine(ActivateEffect(randomBossEffect));

        StartCoroutine(DeactivateEffect(randomBossEffect));

        StartCoroutine(ResetCooldown());
    }


    /// <summary>
    /// Changes the animator parameters to control the enemy's animation states.
    /// </summary>
    /// <param name="idle">Whether the enemy should be in idle animation state.</param>
    /// <param name="walking">Whether the enemy should be in walking animation state.</param>
    /// <param name="chasing">Whether the enemy should be in chasing animation state.</param>
    public void ChangeAnimaions(bool idle, bool walking, bool chasing)
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isIdle", idle);
        animator.SetBool("isWalking", walking);
        animator.SetBool("isChasing", chasing);
    }


    /// <summary>
    /// Generates and returns a random attack name from a predefined list of attack names.
    /// </summary>
    /// <returns>A randomly selected attack name.</returns>
    public string GetRandomAttack()
    {
        string[] attackNames = { "Attack0", "Attack1", "Attack2", "Attack3" };
        int randomIndex = Random.Range(minAttackRange, maxAttackRange + 1);
        return attackNames[randomIndex];
    }


    /// <summary>
    /// Pushes the enemy away from the player if it's too close, based on the specified distance threshold and push-back force.
    /// </summary>
    public void PushEnemyAwayIfTooClose()
    {
        /*float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < tooCloseDistance) // Check if the enemy is too close to the player
        {
            //Calculate the direction from the enemy to the player
            Vector3 directionFromPlayer = Vector3.Normalize(transform.position - player.transform.position);

            // Calculate the movement vector
            Vector3 movement = directionFromPlayer * moveBackDistance;

            // Move the enemy
            transform.Translate(movement, Space.World);
        }*/
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < tooCloseDistance)
        {
            Vector3 dirFromPlayer = Vector3.Normalize(transform.position - player.transform.position);
            targetPosition = transform.position + dirFromPlayer * moveBackDistance;
            animator.SetTrigger("Backward");
        }

        if (distanceToPlayer < tooCloseDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveBackSpeed);
            animator.SetTrigger("Backward");
        }

    }


    void OnAnimatorMove()
    {
        // Update the position and rotation of the collider based on the animation
        transform.SetPositionAndRotation(animator.rootPosition, animator.rootRotation);
    }

    private void OnDrawGizmosSelected()
    {
        // Set the color of the gizmo
        Gizmos.color = Color.blue;

        // Draw a wire sphere around the enemy to represent the look radius
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), rangeToAttack);
    }

    public float GetRangeToChasePlayer()
    {
        return rangeToChasePlayer;
    }

    public void SetRangeToChasePlayer(float value)
    {
        rangeToChasePlayer = value;
    }

    public float GetRangeToAttack()
    {
        return rangeToAttack;
    }

    public void SetRangeToAttack(float value)
    {
        rangeToAttack = value;
    }

    public bool GetisUseMagic()
    {
        return isUseMagic;
    }
    public void GoChase(int i)
    {
        if (i == 0)
        {
            animator.SetBool("isChasing", false);
        }
        else
        {
            animator.SetBool("isChasing", true);
        }
    }

}
/*NavMeshAgent agentRef = GetComponent<NavMeshAgent>();
        //int attackingAnimationHash = Animator.StringToHash("Attack");
        bool isAttackingAnimationPlaying = animator.GetCurrentAnimatorStateInfo(0).IsTag("isAttacking");
        if (isAttackingAnimationPlaying || isFirstAttack)
        {
            // Stop the agent's movement
            agentRef.isStopped = true;
            agentRef.velocity = Vector3.zero;

            // Disable agent's auto-braking to prevent sudden stop
            agentRef.autoBraking = false;

            // Set agent's speed to 0 for immediate stop
            agentRef.speed = 0f;

            // Update animator parameters for attacking animation
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", false);
            animator.SetBool("isChasing", false);
            animator.SetBool("isAttacking", true);

            isFirstAttack = false;
            StartCoroutine(ResetAttackingAnimation());

        }
        else
        {
            Debug.Log("STOP!");
            // Resume the agent's movement
            agentRef.isStopped = false;

            // Enable agent's auto-braking for smoother deceleration
            agentRef.autoBraking = true;

            // Set agent's speed back to the desired value
            agentRef.speed = 8f;

            // Update animator parameters for movement animation
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", false);
            animator.SetBool("isAttacking", false);

            animator.SetBool("isChasing", true);
        }*/