using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlotDrag : MonoBehaviour, IDropHandler
{
    private bool isAssigned;    // Flag to track if there is a skill assigned

    [SerializeField] private Sprite defaultImage;   // The default sprite to be set on the Image component
    [SerializeField] private bool isSkill;          // Flag to determine if this slot related to a skill
    [SerializeField] private bool isAbility;        // Flag to determine if this slot related to an ability

    [SerializeField] private GameObject cantAssignSkill;



    /// <summary>
    /// Handles the drop event when a skill or ability is dragged into the skill slot
    /// </summary>
    /// <param name="eventData">Event data containing information about the drop.</param>
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;

        Transform skillAttributes = dropped.GetComponent<FindSkillAttributes>().SkillAttributes;

        DragSkill draggableSkill = dropped.GetComponent<DragSkill>();
        bool cantSwitchSkill;

        cantSwitchSkill = gameObject.GetComponent<SkillKeybindings>().IsSkillCooldown;

        // Check if the draggable skill has a cooldown
        if (draggableSkill.Skill.GetComponent<SkillInfo>().isCooldown)
        {
            // Don't process the drop if the skill is on cooldown
            return;
        }

        // If cant switch the skill due to cooldown, don't switch skills
        if (cantSwitchSkill)
        {
            return;
        }

        else if((draggableSkill.IsSkill && !isAbility) || (draggableSkill.IsAbility && !isSkill))
        {
            try
            {
                if(IsSkillLevelZero(skillAttributes))
                {
                    return;
                }
            }
            catch { }

            // Update the skill slot with the dropped skill's image
            this.gameObject.GetComponentInChildren<Image>().sprite = dropped.GetComponentInChildren<Image>().sprite;

            // Check if the skill is already assigned and handle accordingly
            IsSkillAlreadyAssigned(draggableSkill);

            // Swap skills if the skill being dropped already exists in another slot
            SwapIfSkillExist(draggableSkill);

            // Assign the skill to the slot after dragging
            AssignSkillWhenDrag(draggableSkill);

        }
    }


    private bool IsSkillLevelZero(Transform skillAttributes)
    {
        int skillLevel = int.Parse(skillAttributes.GetComponent<SkillSlots>().SkillLevelText.text);

        if (skillLevel == 0)
        {
            cantAssignSkill.SetActive(true);
            AudioManager.Instance.Play("CantUseSKillEffect");
            StartCoroutine(DeactivateText());
            return true;
        }
        return false;
    }


    /// <summary>
    /// Checks if a skill is already assigned to a slot and resets the previous slot's image if needed
    /// </summary>
    /// <param name="draggableSkill">The dragged skill to be checked.</param>
    public void IsSkillAlreadyAssigned(DragSkill draggableSkill)
    {
        // If the skill is already assigned on a slot
        if (draggableSkill.Skill.GetComponent<SkillInfo>().isAssigned == true)
        {
            try
            {
                // Try to get the first's child image if there is
                Image prevSlot = draggableSkill.Skill.transform.parent.GetChild(0).gameObject.GetComponent<Image>();
                prevSlot.sprite = defaultImage;
            }
            catch { }
        }
    }


    /// <summary>
    /// Swaps skills if a skill is already assigned to the slot.
    /// </summary>
    /// <param name="draggableSkill">The dragged skill to be swapped.</param>
    public void SwapIfSkillExist(DragSkill draggableSkill)
    {
        // If there is already a skill assigned to this slot 
        if (transform.childCount == 2)
        {
            // If there is already a skill there it swaps between the two skills
            try
            {
                // Try to get the second child if there is and set its parent to null (doesn't have a parent)
                gameObject.transform.GetChild(1).GetComponent<SkillInfo>().isAssigned = false;
                gameObject.transform.GetChild(1).SetParent(null);
            }
            catch { }
            
            // Put the new dragged skill as a child to this gameObject
            draggableSkill.Skill.transform.SetParent(this.gameObject.transform);
            gameObject.GetComponent<SkillKeybindings>().Skiill = draggableSkill.Skill;
        }
    }


    /// <summary>
    /// Assigns a skill to the slot when it is dragged onto it for the first time.
    /// </summary>
    /// <param name="draggableSkill">The dragged skill to be assigned.</param>
    public void AssignSkillWhenDrag(DragSkill draggableSkill)
    {
        // If this slot got a skill assigned for the first time
        if (transform.childCount == 1)
        {
            // If there is still no skill assigned to that slot (first time assigning)
            draggableSkill.Skill.transform.SetParent(this.gameObject.transform);

            // Get the specific cooldown for that skill
            gameObject.GetComponent<SkillKeybindings>().Cooldown = draggableSkill.Skill.GetComponent<SkillInfo>().cooldownSlot;

            // Get the specific skill gameObject and assign it to this skill
            gameObject.GetComponent<SkillKeybindings>().Skiill = draggableSkill.Skill;

            // Mark the skill as assigned
            draggableSkill.Skill.GetComponent<SkillInfo>().isAssigned = true;

            // Markt his skill's slot is assigned
            isAssigned = true;  
        }
    }

    IEnumerator DeactivateText()
    {
        yield return new WaitForSeconds(2f);
        cantAssignSkill.SetActive(false);
    }

    // Getters and Setters
    public bool IsAssigned
    {
        get { return isAssigned; }
        set { isAssigned = value; }
    }
}