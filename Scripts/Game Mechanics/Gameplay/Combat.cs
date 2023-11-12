using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;
using System.Collections;

public class Combat : MonoBehaviour
{
    private bool isAttacking = false;

    ThirdPersonController thirdPersonController;  // Reference to the thirdPersonController
    Animator animator;                            // Reference to the Animator
    private bool isInConv;                        // Is the player currently on a conversation
    [SerializeField] private Transform cam;       // Reference to the camera transform              

    [Header("UI")]
    [SerializeField] private GameObject inventoryGameObject;          // Reference to the inventory's gameobject
    [SerializeField] private GameObject equipmentsGameObject;         // Reference to the equipment's gameobject
    [SerializeField] private GameObject skillsObject;                 // Reference to the skill's gameobject
    [SerializeField] private GameObject questObject;                  // Reference to the quest's gameobject
    [SerializeField] private GameObject dialogueObject;               // Reference to the dialogue's gameobject
    [SerializeField] private GameObject statsObject;                  // Reference to the stat's gameobject
    [SerializeField] private GameObject pauseObject;                  // Reference to the stat's gameobject
    [SerializeField] private GameObject helpObject;

    [SerializeField] private NPCInteraction currentNPC;


    private InputAction iaPlayer;                                     // Reference to the Input Action for the player
    private @StarterAssets41 controls;                                // Reference to the player controls


    [SerializeField] private GameObject cameraRotationObject;         // Reference to the camera follow player
    private Quaternion initialRotation;                               // Reference to the camera follow player rotation
    [SerializeField] private bool inventoryActive;                    // If the inventory's window is now opened
    [SerializeField] private bool equipmentsActive1;                  // If the equipment's window is now opened
    [SerializeField] private bool skillActive;                        // If the skill's window is now opened
    [SerializeField] private bool questActive;                        // If the quest's window is now opened
    [SerializeField] private bool statsAcive;                         // If the stat's window is now opened
    [SerializeField] private bool dialogueActive;                     // If the quest's window is now opened
    [SerializeField] private bool pauseActive;                        // If the quest's window is now opened
    [SerializeField] private bool helpActive;                         // If the quest's window is now opened
    [SerializeField] private bool flagDialog = true;                  // If the quest's window is now opened

    private readonly int weaponInHandIndex = 8;

    private bool canBlock = true;
    [SerializeField] private float cooldownTime = 10f;

    private void Start()
    {
        try
        {
            // Assign the isInConv
            isInConv = GameObject.Find("ConversationMap1").GetComponent<ConversationMap1>().GetIsInConversation();
        }
        // Not all scenes have the ConversationMap1 component
        catch { }


        animator = GetComponent<Animator>(); // Get the component to make animation
        thirdPersonController = GetComponent<ThirdPersonController>();
        Cursor.visible = true;

    }

    private void Awake()
    {
        controls = new @StarterAssets41();
    }


    /// <summary>
    /// Enables input actions related to player controls when the script is enabled.
    /// Binds various input actions to their corresponding functions.
    /// </summary>
    private void OnEnable()
    {
        var playerControlls = controls.Player;
        iaPlayer = controls.Player.DrawSword;
        iaPlayer.Enable();

        playerControlls.DrawSword.performed += drawSword;
        playerControlls.DrawSword.Enable();
        
        playerControlls.SheathSword.performed += sheathSword;
        playerControlls.SheathSword.Enable();

        playerControlls.SwingSword.performed += SwingSword;
        playerControlls.SwingSword.Enable();

        playerControlls.Blocking.performed += BlockSword;
        playerControlls.Blocking.Enable();

        playerControlls.OpenInv.performed += OpenInv;
        playerControlls.OpenInv.Enable();

        playerControlls.OpenEqu.performed += OpenEqu;
        playerControlls.OpenEqu.Enable();

        playerControlls.Skillss.performed += OpenCloseSkill;
        playerControlls.Skillss.Enable();

        playerControlls.OpenQuest.performed += OpenQuest;
        playerControlls.OpenQuest.Enable();

        playerControlls.NPCInteract.performed += NpcInteract;
        playerControlls.NPCInteract.Enable();

        playerControlls.PlayerStats.performed += OpenStats;
        playerControlls.PlayerStats.Enable();

        playerControlls.PauseMenu.performed += PauseGame;
        playerControlls.PauseMenu.Enable();

        playerControlls.Help.performed += HelpMenu;
        playerControlls.Help.Enable();

    }




    /// <summary>
    /// Opens or closes a UI GameObject and adjusts game settings accordingly.
    /// </summary>
    /// <param name="ui">The UI GameObject to open or close.</param>
    /// <param name="isActive">Determines whether the UI should be set to active or inactive.</param>
    public void OpenCloseUI(GameObject ui, bool isActive)
    {
        // If the player is on conversation he can't open or close
        if (isInConv)
        {
            return;
        }

        isActive = !ui.activeSelf;
        ui.SetActive(isActive);

        if (isActive)
        {
            AudioManager.Instance.Play("OpenUIEffect");
            LockCameraa.GetInstance().ShowMouseCursor();
            Time.timeScale = 0; // freeze the game world
        }
        else
        {
            AudioManager.Instance.Play("CloseUIEffect");
            LockCameraa.GetInstance().HideMouseCursor();
            Time.timeScale = 1; // unfreeze the game world
        }
        LockOrUnlockCamera();
    }

    /// <summary>
    /// Callback method to toggle the help menu UI.
    /// Calls the OpenCloseUI method to open or close the help menu UI.
    /// </summary>
    /// <param name="obj">The InputAction.CallbackContext representing the input action.</param>

    private void HelpMenu(InputAction.CallbackContext obj)
    {
        OpenCloseUI(helpObject, helpActive);
    }

    /// <summary>
    /// Callback method to pause or resume the game.
    /// Calls the OpenCloseUI method to open or close the pause menu UI.
    /// </summary>
    /// <param name="obj">The InputAction.CallbackContext representing the input action.</param>
    private void PauseGame(InputAction.CallbackContext obj)
    {
        OpenCloseUI(pauseObject, pauseActive);
    }

    /// <summary>
    /// Callback method to open or close the statistics UI.
    /// Calls the OpenCloseUI method to open or close the stats menu UI.
    /// </summary>
    /// <param name="obj">The InputAction.CallbackContext representing the input action.</param>
    private void OpenStats(InputAction.CallbackContext obj)
    {
        OpenCloseUI(statsObject, statsAcive);
    }


    /// <summary>
    /// Callback method to open or close the quest UI.
    /// Calls the OpenCloseUI method to open or close the quest menu UI.
    /// </summary>
    /// <param name="obj">The InputAction.CallbackContext representing the input action.</param>
    private void OpenQuest(InputAction.CallbackContext obj)
    {
        OpenCloseUI(questObject, questActive);
    }


    /// <summary>
    /// Callback method to open or close the equipment UI.
    /// Calls the OpenCloseUI method to open or close the equipment menu UI.
    /// </summary>
    /// <param name="obj">The InputAction.CallbackContext representing the input action.</param>
    private void OpenEqu(InputAction.CallbackContext obj)
    {
        OpenCloseUI(equipmentsGameObject, equipmentsActive1);
    }


    /// <summary>
    /// Callback method to open or close the inventory UI.
    /// Calls the OpenCloseUI method to open or close the inventory menu UI.
    /// </summary>
    /// <param name="obj">The InputAction.CallbackContext representing the input action.</param>
    private void OpenInv(InputAction.CallbackContext obj)
    {
        OpenCloseUI(inventoryGameObject, inventoryActive);
    }


    /// <summary>
    /// Callback method to open or close the skill UI.
    /// Calls the OpenCloseUI method to open or close the skill menu UI.
    /// </summary>
    /// <param name="obj">The InputAction.CallbackContext representing the input action.</param>
    private void OpenCloseSkill(InputAction.CallbackContext obj)
    {
        OpenCloseUI(skillsObject, skillActive);
    }


    /// <summary>
    /// Callback method to initiate NPC interaction and start conversations.
    /// Handles opening and closing of the dialogue UI and camera settings.
    /// </summary>
    /// <param name="obj">The InputAction.CallbackContext representing the input action.</param>
    private void NpcInteract(InputAction.CallbackContext obj)
    {
        //OnTriggerEnter(gameObject.GetComponent<Collider>());
        if (currentNPC == null || isInConv)
        {
            return;
        }

        // If the current NPC the player standing next to him don't want to talk, then return.
        var npc = currentNPC.GetComponent<NPCInteraction>();
        if (!npc.Dialogue[0].wantToTalk)
        {
            return;
        }

        //DisableUI.GetInstance().GetUiGameplay().SetActive(false);
        DisableUI.GetInstance().HideAllUIElements();

        dialogueActive = !dialogueObject.activeSelf;
        dialogueObject.SetActive(dialogueActive);
        currentNPC.StartConversation();
        
        if (dialogueActive)
        {
            LockCameraa.GetInstance().LockCameraRotation();
            LockCameraa.GetInstance().ShowMouseCursor();
        }
        else
        {
            LockCameraa.GetInstance().HideMouseCursor();
            dialogueActive = false;
        }

        LockOrUnlockCamera();
        dialogueActive = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            currentNPC = other.GetComponent<NPCInteraction>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            currentNPC = null;
            //DisableUI.GetInstance().GetUiGameplay().SetActive(true);
            DisableUI.GetInstance().ShowAllUIElements();
        }
    }


    /// <summary>
    /// Locks or unlocks camera rotation based on UI and dialogue activity.
    /// Locks camera rotation if any relevant UI or dialogue is active; otherwise, unlocks it.
    /// </summary>
    private void LockOrUnlockCamera()
    {
        if (inventoryGameObject.activeSelf || equipmentsGameObject.activeSelf || skillsObject.activeSelf || questObject.activeSelf
                    || dialogueActive || statsObject.activeSelf || pauseObject.activeSelf || helpObject.activeSelf)
        {
            LockCameraa.GetInstance().LockCameraRotation();
        }
        else
        {
            if (!inventoryActive && !equipmentsActive1 && !skillActive && !questActive && !dialogueActive
                 && !statsAcive && !pauseActive && !helpActive)
                LockCameraa.GetInstance().UnlockCameraRotation();
        }
    }


    /// <summary>
    /// Initiates a sword swing action if a sword is equipped in the appropriate slot.
    /// Triggers the swing animation and sets the attacking state to true.
    /// </summary>
    /// <param name="obj">The InputAction.CallbackContext representing the input action.</param>
    private void SwingSword(InputAction.CallbackContext obj)
    {
        if (!IsGrounded())
        {
            return;
        }

        if(!IsWeaponInHand())
        {
            return;
        }

        try
        {
            if(IsAttackSwift())
            {
                return;
            }
        }
        catch { }

        animator.SetTrigger("Swing");
        isAttacking = true;
    }


    /// <summary>
    /// If the attack is the Swift ability Attack
    /// </summary>
    /// <returns></returns>
    private bool IsAttackSwift()
    {
        SwiftSlasher swift = null;
        try
        {
            swift = GetComponent<SwiftSlasher>();
        }
        catch { }

        if (swift.IsUsingSwift)
        {
            swift.ResetWeaponPosRot();
            swift.CanUseSwift = false;

            animator.SetTrigger("Swift");
            isAttacking = true;
            swift.StopAbility();
            Time.timeScale = 0.5f;
            StartCoroutine(ResetTime());

            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsGrounded()
    {
        // Can't attack if not grounded
        return GetComponent<ThirdPersonController>().IsGrounded;
    }

    private bool IsWeaponInHand()
    {
        // If there is no weapon on the player hand, can't swing
        if (Inventory.Instance.WeaponsInHand[weaponInHandIndex] == null)
        {
            return false;
        }
        return true;
    }

    IEnumerator ResetTime()
    {
        yield return new WaitForSecondsRealtime(0.7f);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Initiates a sword block action if a sword is equipped in the appropriate slot and the block is not on cooldown.
    /// Triggers the block animation and starts the block cooldown timer.
    /// </summary>
    /// <param name="obj">The InputAction.CallbackContext representing the input action.</param>
    private void BlockSword(InputAction.CallbackContext obj)
    {
        if (!IsGrounded())
        {
            return;
        }

        if (!IsWeaponInHand())
        {
            return;
        }

        if (canBlock)
        {
            StartCoroutine(BlockCooldown());
            animator.SetTrigger("Block");
        }
    }


    /// <summary>
    /// Starts a cooldown timer for the sword block action.
    /// Prevents further blocking during the cooldown period.
    /// </summary>
    /// <returns>An IEnumerator used for the coroutine.</returns>
    private IEnumerator BlockCooldown()
    {
        canBlock = false;
        float timer = cooldownTime;

        while (timer > 0)
        {
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }
        canBlock = true;
    }


    /// <summary>
    /// Disables player input actions when the component is disabled.
    /// </summary>
    private void OnDisable()
    {
        iaPlayer.Disable();
        var playerControlls = controls.Player;
        playerControlls.DrawSword.Disable();
        playerControlls.SheathSword.Disable();
        playerControlls.SwingSword.Disable();
        playerControlls.Blocking.Disable();

        playerControlls.Interacting.Disable();
        playerControlls.Skillss.Disable();
        playerControlls.Inventoryy.Disable();
        playerControlls.Equipmentss.Disable();


        playerControlls.NPCInteract.Disable();
        playerControlls.FirstSkill.Disable();
        playerControlls.SecondSkill.Disable();
        playerControlls.ThirdSkill.Disable();
        playerControlls.FourthSkill.Disable();
        playerControlls.FivthSkill.Disable();
        playerControlls.SixthSkill.Disable();
        playerControlls.SeventhSkill.Disable();
        playerControlls.EigthSkill.Disable();

        playerControlls.FirstAbility.Disable();
        playerControlls.SecondAbility.Disable();
        playerControlls.ThirdAbility.Disable();
        playerControlls.FourthAbility.Disable();

        controls.Player.Help.Disable();
    }


    /// <summary>
    /// Disables or enables player movements based on the animation event parameter 'isAttacking'.
    /// If 'isAttacking' is 1, disables movements and enables blocking.
    /// If 'isAttacking' is 0, enables movements and disables blocking.
    /// </summary>
    /// <param name="isAttacking">An integer representing the current attacking state (1 for attacking, 0 for not attacking).</param>
    public void DisableMovements(int isAttacking)
    {
        if (isAttacking == 1)
        {
            thirdPersonController.enabled = false;
            GetComponent<PlayerCamera>().enabled = false;

            GetComponent<PlayerStat>().IsBlocking = true;
            return;
        }
        if (isAttacking == 0)
        {
            thirdPersonController.enabled = true;
            GetComponent<PlayerCamera>().enabled = true;

            GetComponent<PlayerStat>().IsBlocking = false;
            return;
        }
    }


    private void drawSword(InputAction.CallbackContext obj)
    {
    }

    private void sheathSword(InputAction.CallbackContext obj)
    {
    }
    public bool GetIsAttacking()
    {
        return isAttacking;
    }

    public void SetIsAttacking(bool value)
    {
        isAttacking = value;
    }

    public Transform GetCam()
    {
        return cam;
    }

    public void SetCam(Transform value)
    {
        cam = value;
    }


    public void SetCurrentNPC(NPCInteraction value)
    {
        currentNPC = value;
    }

    public InputAction GetIAPlayer()
    {
        return iaPlayer;
    }

    public void SetIAPlayer(InputAction value)
    {
        iaPlayer = value;
    }

    public StarterAssets41 GetControls()
    {
        return controls;
    }

    public void SetControls(StarterAssets41 value)
    {
        controls = value;
    }

    public GameObject GetCameraRotationObject()
    {
        return cameraRotationObject;
    }

    public void SetCameraRotationObject(GameObject value)
    {
        cameraRotationObject = value;
    }

    public Quaternion GetInitialRotation()
    {
        return initialRotation;
    }

    public void SetInitialRotation(Quaternion value)
    {
        initialRotation = value;
    }

    public bool GetInventoryActive()
    {
        return inventoryActive;
    }

    public void SetInventoryActive(bool value)
    {
        inventoryActive = value;
    }

    public bool GetEquipmentsActive1()
    {
        return equipmentsActive1;
    }

    public void SetEquipmentsActive1(bool value)
    {
        equipmentsActive1 = value;
    }

    public bool GetSkillActive()
    {
        return skillActive;
    }

    public void SetSkillActive(bool value)
    {
        skillActive = value;
    }

    public bool GetQuestActive()
    {
        return questActive;
    }

    public void SetQuestActive(bool value)
    {
        questActive = value;
    }

    public bool GetStatsActive()
    {
        return statsAcive;
    }

    public void SetStatsActive(bool value)
    {
        statsAcive = value;
    }

    public bool GetDialogueActive()
    {
        return dialogueActive;
    }

    public void SetDialogueActive(bool value)
    {
        dialogueActive = value;
    }

    public bool GetPauseActive()
    {
        return pauseActive;
    }

    public void SetPauseActive(bool value)
    {
        pauseActive = value;
    }

    public bool GetHelpActive()
    {
        return helpActive;
    }

    public void SetHelpActive(bool value)
    {
        helpActive = value;
    }

    public bool GetFlagDialog()
    {
        return flagDialog;
    }

    public void SetFlagDialog(bool value)
    {
        flagDialog = value;
    }
    public NPCInteraction GetCurrentNPC()
    {
        return this.currentNPC;
    }

    public void SetCurrentNPCNull()
    {
        this.currentNPC = null;
    }
}
