using System.Collections;
using UnityEngine;

namespace StarterAssets
{

    public class SwiftSlasher : MonoBehaviour
    {
        [SerializeField] private float speedModifier;       // Speed when using the ability
        [SerializeField] private int jumpDistanceModifier;  // Jump distance when using the ability
        [SerializeField] private int gravityModifier;       // Jump distance when using the ability
        [SerializeField] private float activeTime;

        [SerializeField] private GameObject speedEffect;    // Reference to the speed effect
        private GameObject clonedSpeedEffect;   // The cloned speed effect
        [SerializeField] private Transform playerSpine;

        private bool canUseSwift = true;
        private bool isUsingSwift;

        [SerializeField] private ThirdPersonController tpc;
        [SerializeField] private TimeAbility timeAbility;

        private float characterSpeed;   // Reference to the speed property
        private float characterSprint;  // Reference to the sprint property
        private float jumpHeight;       // Reference to the jump property
        private float gravity;          // Reference to the gravity property

        private Transform weapon;

        // Coordinates and rotation values to use when equipping items
        [SerializeField] private float pX;
        [SerializeField] private float pY;
        [SerializeField] private float pZ;
        [SerializeField] private float pr1;
        [SerializeField] private float pr2;
        [SerializeField] private float pr3;


        private void Start()
        {
            characterSpeed = GetComponent<ThirdPersonController>().CharacterMoveSpeed;
            characterSprint = GetComponent<ThirdPersonController>().CharacterSprintSpeed;
            jumpHeight = GetComponent<ThirdPersonController>().CharacterJumpHeight;
            gravity = GetComponent<ThirdPersonController>().CharacterGravity;
        }

        private void Update()
        {
            // For me !!
            if (Input.GetKeyDown(KeyCode.L))
            {
                SpeedAbility();
            }

            if(clonedSpeedEffect != null)
            {
                clonedSpeedEffect.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, 0, 0));
                clonedSpeedEffect.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }


        /// <summary>
        /// Activate the ability and make the player go Swift
        /// </summary>
        public void SpeedAbility()
        {
            if (canUseSwift)
            {
                timeAbility.enabled = false;

                clonedSpeedEffect = Instantiate(speedEffect);
                clonedSpeedEffect.transform.SetParent(playerSpine);
                clonedSpeedEffect.transform.position = new Vector3 (0, 0, 0);
                clonedSpeedEffect.transform.localRotation = Quaternion.Euler(0, 90, 0);

                isUsingSwift = true;
                canUseSwift = false;

                ModifyWeaponPosRot();
                tpc.CharacterMoveSpeed = speedModifier;
                tpc.CharacterSprintSpeed = speedModifier;
                tpc.CharacterJumpHeight = jumpHeight * jumpDistanceModifier;
                tpc.CharacterGravity = gravity * gravityModifier;

                StartCoroutine(StartCooldown());
            }
        }


        /// <summary>
        /// Modify the weapon position and rotation to match the animation.
        /// </summary>
        private void ModifyWeaponPosRot()
        {
            weapon = Inventory.Instance.WeaponsInHand[8].transform;
            Vector3 newLocalPosition = new (pX, pY, pZ);
            Quaternion newLocalRotation = Quaternion.Euler(pr1, pr2, pr3);

            weapon.SetLocalPositionAndRotation(newLocalPosition, newLocalRotation);
        }

        /// <summary>
        /// When the ability is done reset the weapon position and rotation
        /// </summary>
        public void ResetWeaponPosRot()
        {
            weapon = Inventory.Instance.WeaponsInHand[8].transform;
            ItemPickUp weaponIPU = weapon.GetComponent<ItemPickUp>();

            Vector3 newLocalPosition = new (weaponIPU.PX, weaponIPU.PY, weaponIPU.PZ);
            Quaternion newLocalRotation = Quaternion.Euler(weaponIPU.PR1, weaponIPU.PR2, weaponIPU.PR3);

            weapon.SetLocalPositionAndRotation(newLocalPosition, newLocalRotation);
        }


        /// <summary>
        /// Start cooldown
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartCooldown()
        {
            yield return new WaitForSeconds(activeTime);

            StopAbility();
        }

        public void StopAbility()
        {
            Destroy(clonedSpeedEffect);

            canUseSwift = true;
            isUsingSwift = false;

            ResetWeaponPosRot();

            // Reset properties to thier original values
            tpc.CharacterMoveSpeed = characterSpeed;
            tpc.CharacterSprintSpeed = characterSprint;
            tpc.CharacterJumpHeight = jumpHeight;
            tpc.CharacterGravity = gravity;

            timeAbility.enabled = true;
        }

        public bool IsUsingSwift
        {
            get { return isUsingSwift; }
            set { isUsingSwift = value; }
        }

        public bool CanUseSwift
        {
            get { return canUseSwift; }
            set { canUseSwift = value; }
        }
    }
}