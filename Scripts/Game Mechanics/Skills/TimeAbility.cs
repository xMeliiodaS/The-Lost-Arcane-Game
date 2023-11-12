using StarterAssets;
using System.Collections;
using UnityEngine;

public class TimeAbility : MonoBehaviour
{
    [SerializeField] private float duration = 15f;  // Duration of the time-slowing effect in seconds
    [SerializeField] private GameObject effect;     // Cool effect to the ability

    private bool isSlowingTime = false;             // Is the ability activated or not

    private readonly float slowdownFactor = 0.5f;   // Control the slowdown effect
    private float playerSpeedMultiplier = 2f;       // Control the player's speed when time is slowed down

    private float originalAnimationSpeed;           // Store the original animation speed
    private float playerSpeedMul;                   

    private float originalPlayerSpeed;              // Store the original animation speed
    private float originalPlayerSprintSpeed;        // Store the original player's sprint speed
    private float gravity;                          // Store the original player's Gravity

    private Animator playerAnimator;                // Reference to the player's Animator component
    private ThirdPersonController controller;

    private AudioManager audioInstance;

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();

        // Store the original animation speed
        originalAnimationSpeed = playerAnimator.speed;

        // Store the original player's speed
        playerSpeedMul = playerSpeedMultiplier;

        controller = GetComponent<ThirdPersonController>();

        originalPlayerSpeed = controller.CharacterMoveSpeed;
        originalPlayerSprintSpeed = controller.CharacterSprintSpeed;
        gravity = controller.CharacterGravity;

        audioInstance = AudioManager.Instance;
    }


    /// <summary>
    /// Check if the ability is active, and adjust the time based on that.
    /// </summary>
    private void Update()
    {
        // Check if the time slowdown effect is active
        if (isSlowingTime)
        {
            // Adjust the player's animation speed based on the slowdown factor
            playerAnimator.speed = originalAnimationSpeed / slowdownFactor;

            // Adjust the player's speed based on the playerSpeedMultiplier
            playerSpeedMultiplier = playerSpeedMul / slowdownFactor * 2;

            controller.CharacterMoveSpeed = playerSpeedMultiplier;
            controller.CharacterSprintSpeed = playerSpeedMultiplier + 4f;

        }
        else
        {
            // Restore the original animation speed
            playerAnimator.speed = originalAnimationSpeed;

            // Restore the original player's speed
            playerSpeedMultiplier = playerSpeedMul;

            controller.CharacterMoveSpeed = originalPlayerSpeed;
            controller.CharacterSprintSpeed = originalPlayerSprintSpeed;

            var gravity = GetComponent<ThirdPersonController>();
            gravity.CharacterGravity = this.gravity;
        }
    }


    /// <summary>
    /// Play animation with sound, the animation has animation trigger event to call the AbilityAnimation() 
    /// function and activate the ability.
    /// </summary>
    public void SlowDownTime()
    {
        effect.SetActive(true);
        effect.transform.position = transform.position;

        audioInstance.Play("TimeAbilityEffect");
        playerAnimator.SetTrigger("Ability1");

        DisablePlayerComponents(false, false);
    }


    /// <summary>
    /// Make everything slower except the player, make his time normal.
    /// </summary>
    public void AbilityAnimation()
    {
        DisablePlayerComponents(true, true);
        effect.SetActive(false);
        // Start a coroutine to reset the time-slowing effect after the specified duration
        StartCoroutine(ResetTimeAfterDuration());

        if (!isSlowingTime)
        {
            isSlowingTime = true;
            Time.timeScale = slowdownFactor;
            Time.fixedDeltaTime = Time.timeScale * 0.02f; // Adjusting the fixed delta time to maintain physics simulation

            Rigidbody playerRigidbody = GetComponent<Rigidbody>();

            if (playerRigidbody != null)
            {
                playerRigidbody.velocity /= slowdownFactor;
                playerRigidbody.angularVelocity /= slowdownFactor;

                var gravity = GetComponent<ThirdPersonController>();
                gravity.CharacterGravity *= 2f;
            }
        }
    }


    /// <summary>
    /// Reset the timer after the duration ends
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetTimeAfterDuration()
    {
        yield return new WaitForSeconds(duration - 3f);

        audioInstance.Play("TimeUpEffect");

        // Wait for the remaining 3 seconds, then reset the time
        yield return new WaitForSeconds(3f);
        ResetTime();
    }


    /// <summary>
    /// Reset the time
    /// </summary>
    private void ResetTime()
    {
        isSlowingTime = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f; // Reset the fixed delta time to its default value
    }


    /// <summary>
    /// Disable the player controller and camera in order to disable the movement
    /// while doing the animation
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="camera"></param>
    private void DisablePlayerComponents(bool controller, bool camera)
    {
        GetComponent<ThirdPersonController>().enabled = controller;
        GetComponent<PlayerCamera>().enabled = camera;
    }
}
