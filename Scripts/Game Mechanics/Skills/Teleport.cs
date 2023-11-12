using UnityEngine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;

public class Teleport : MonoBehaviour
{

    [Header("Teleport Fields")]
    private ThirdPersonController playerController;     // Reference to the player's ThirdPersonController component
    [SerializeField] private float teleportDistance;    // The distance the player will teleport
    [SerializeField] private GameObject vfx;            // The visual effect to display during teleportation
    private GameObject teleportEffect = null;           // Reference to the instantiated teleportation effect
    [Space]

    [Header("Dodge Fields")]
    [SerializeField] private float dodgeCooldown = 4.0f; // The cooldown time for performing a dodge

    // Variables to track key presses and dodge timings
    private bool aKeyPressed = false;   // Tracks if the 'A' key is pressed
    private bool dKeyPressed = false;   // Tracks if the 'D' key is pressed

    private float lastDodgeTime;        // Records the time of the last dodge
    private float lastAKeyPressTime;    // Records the time of the last 'A' key press
    private float lastDKeyPressTime;    // Records the time of the last 'D' key press
    private readonly float doubleClickTimeThreshold = 0.35f; // The time threshold for detecting double-clicks

    [Space]
    [Header("RiftWalker Fields")]

    private bool canRift = true;       // Can us the rift ability
    [SerializeField] private float shiftCooldown = 7f;  // Cooldown between rifts
    private float lastShiftTime;

    [SerializeField] private GameObject riftPrefab; // Prefab for the rift object

    [SerializeField] private LayerMask layerMask;   // Reference to the layer mask
    [SerializeField] private List<GameObject> rifts = new();    // All the currently instantiated rifts
    private static bool deleteAllCooldown;    // If can start the cooldown to delete all the rifts

    [SerializeField] private bool isEnemy;

    [SerializeField] private float minRandomPos = 35f;
    [SerializeField] private float maxRandomPos = 35f;
    void Start()
    {
        playerController = gameObject.GetComponent<ThirdPersonController>();
    }


    /// <summary>
    /// Activates the RiftWalker ability if conditions are met.
    /// </summary>
    /// <returns>Returns true if the RiftWalker ability was activated, otherwise false.</returns>
    public bool RiftWalker()
    {
        // Check if the RiftWalker ability is available and cooldown time has passed
        if (canRift && Time.time - lastShiftTime >= shiftCooldown)
        {
            Vector3 randomOffset = CalculateDestinationRiftPosition();

            // Maximum distance for the raycast
            float maxDistance = 110f;

            AudioManager.Instance.Play("RiftEffect");

            if (!isEnemy)
            {
                GetComponent<Animator>().SetTrigger("Rift");
            }

            // Check if the raycast hits a suitable position for the destination rift
            if (Physics.Raycast(randomOffset, Vector3.down, out RaycastHit hit, maxDistance, layerMask))
            {
                // The position where the ray hits the layer
                Vector3 layerPosition = hit.point;
                Vector3 adjustedRandomOffset = new(randomOffset.x, layerPosition.y, randomOffset.z);


                // Create the original and destination rift
                StartCoroutine(CreateRift(transform.position + transform.forward * 2f, adjustedRandomOffset));

                lastShiftTime = Time.time;
                canRift = false;

                if(isEnemy)
                {
                    gameObject.transform.position = adjustedRandomOffset;
                }

                // If deleteAllCooldown is not active, start coroutine to delete all rifts
                StartCoroutine(ResetShiftCooldown());

                if(!deleteAllCooldown)
                {
                    StartCoroutine(DeleteAllRifts());
                }
                return true;
            }
            return true;
        }
        return false;
    }


    private Vector3 CalculateDestinationRiftPosition()
    {
        Vector3 randomOffset;
        if (!isEnemy)
        {
            var randomPos = Random.Range(minRandomPos, maxRandomPos);
            float randomXZOffset = randomPos;
            randomOffset = new(transform.position.x + randomXZOffset, 100f, transform.position.z + randomXZOffset);
        }
        else
        {
            GameObject player = FindPlayer.Instance.Player;
            //GameObject player = GameObject.FindWithTag("Player");

            float playerXOffset = player.transform.position.x;

            // Fixed y-coordinate
            float yOffset = 100f;
            float playerZOffset = player.transform.position.z;

            // Distance behind the player
            float distanceBehind = 2f;

            // Get the opposite of the player's forward direction
            Vector3 playerDirection = -player.transform.forward;
            Vector3 newPosition = new Vector3(playerXOffset, yOffset, playerZOffset) + playerDirection * distanceBehind;

            randomOffset = newPosition;
        }
        return randomOffset;
    }

    /// <summary>
    /// Reset the cooldown of the RiftWalker ability after a specified time
    /// </summary>
    private IEnumerator ResetShiftCooldown()
    {
        yield return new WaitForSeconds(shiftCooldown);
        canRift = true;
    }


    /// <summary>
    /// Create a pair of rifts after a short delay to match animation.
    /// </summary>
    /// <param name="position">The position for the original rift</param>
    /// <param name="adjustedRandomOffset">The position for the destination rift</param>
    private IEnumerator CreateRift(Vector3 position, Vector3 adjustedRandomOffset)
    {
        // Cooldown just for the player
        if(!isEnemy)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // Instantiate and activate the original and destination rifts
        GameObject originalRift = Instantiate(riftPrefab, position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        GameObject destinationRift = Instantiate(riftPrefab, adjustedRandomOffset, Quaternion.identity);

        originalRift.SetActive(true);
        destinationRift.SetActive(true);

        // Add the rifts to the list
        rifts.Add(originalRift);
        rifts.Add(destinationRift);
    }


    /// <summary>
    /// After the cooldown ends, delete all the rifts.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeleteAllRifts()
    {
        deleteAllCooldown = true;
        yield return new WaitForSeconds(20f);
        foreach (GameObject rift in rifts)
        {
            Destroy(rift);
        }
        rifts = new List<GameObject>();
        deleteAllCooldown = false;
    }


    /// <summary>
    /// Ability: Teleports the player a short distance forward, accompanied by a visually appealing effect.
    /// </summary>
    /// <returns>IEnumerator for coroutine functionality.</returns>
    public IEnumerator TeleportForward()
    {
        // Disable the player movement
        if(playerController != null)
        {
            playerController.Disabled = true;
        }

        yield return new WaitForSeconds(0.01f);

        // Use the forward vector for forward movement
        Vector3 forwardMovement = transform.forward * teleportDistance;

        // Adjusting the Y-coordinate
        Vector3 teleportPosition = transform.position + forwardMovement + new Vector3(0f, 0.5f, 0f);

        Ray ray = new(transform.position, forwardMovement);

        if (!Physics.Raycast(ray, forwardMovement.magnitude))
        {
            // Calculate the forward offset
            Vector3 forwardOffset = transform.forward * 1.3f;
            AudioManager.Instance.Play("TeleportEffect");

            // Instantiate and play the VFX
            teleportEffect = Instantiate(vfx, transform.position + forwardOffset + new Vector3(0f, 1.2f, 0f), transform.rotation);
            teleportEffect.SetActive(true);


            yield return new WaitForSeconds(0.1f);
            gameObject.transform.position = teleportPosition;

        }

        yield return new WaitForSeconds(0.01f);
        if (playerController != null)
        {
            playerController.Disabled = false;
        }

        yield return new WaitForSeconds(1f);

        try
        {
            teleportEffect.SetActive(false);
        }
        catch { }
    }


    // Update is called once per frame
    private void Update()
    {
        HandleDodgeInput(KeyCode.A, ref aKeyPressed, ref lastAKeyPressTime);
        HandleDodgeInput(KeyCode.D, ref dKeyPressed, ref lastDKeyPressTime);

        if(teleportEffect != null)
        {
            teleportEffect.transform.position = transform.position;
        }
    }


    /// <summary>
    /// Handles the input for performing a dodge action based on the specified key code.
    /// </summary>
    /// <param name="keyCode">The key code representing the dodge input key.</param>
    /// <param name="keyPressedFlag">A reference to the flag that indicates whether the key has been pressed.</param>
    /// <param name="lastKeyPressTime">A reference to the time of the last key press.</param>
    private void HandleDodgeInput(KeyCode keyCode, ref bool keyPressedFlag, ref float lastKeyPressTime)
    {
        if (Input.GetKeyDown(keyCode))
        {
            float timeSinceLastKeyPress = Time.time - lastKeyPressTime;

            if (keyPressedFlag && timeSinceLastKeyPress <= doubleClickTimeThreshold && Time.time - lastDodgeTime >= dodgeCooldown)
            {
                // Perform the dodge action
                Animator animator = GetComponent<Animator>();
                animator.SetTrigger("Dodge");

                // Update the last dodge time
                lastDodgeTime = Time.time;
            }
            else
            {
                keyPressedFlag = true;
            }

            lastKeyPressTime = Time.time;
        }

        // Reset the keyPressedFlag after a certain time
        if (keyPressedFlag && Time.time - lastKeyPressTime > doubleClickTimeThreshold)
        {
            keyPressedFlag = false;
        }
    }
    public List<GameObject> Rifts
    {
        get { return rifts; }
        set { rifts = value; }
    }
}
