using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragSkill : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [HideInInspector] private Transform parentAfterDrag;    

    [SerializeField] private Image image;       // Reference to the Skill Icon
    [SerializeField] private Image targetImage; // The target image
    [SerializeField] private GameObject skill;  // Reference to the skill itself

    [SerializeField] private bool isSkill;      // Indicates whether this slot is intended for skills
    [SerializeField] private bool isAbility;    // Indicates whether this slot is intended for abilities



    /// <summary>
    /// Called when the user begins dragging the UI element
    /// </summary>
    /// <param name="eventData">Pointer event data containing drag information.</param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Store the original parent before dragging and detach from it
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);

        // Set the dragged element as the last sibling to bring it to the front
        transform.SetAsLastSibling();

        // Disable raycast targeting on the image during drag
        image.raycastTarget = false;
    }


    /// <summary>
    /// Called while the UI element is being dragged
    /// </summary>
    /// <param name="eventData">Pointer event data containing drag information.</param>
    public void OnDrag(PointerEventData eventData)
    {
        // Update the position of the UI element to follow the mouse cursor
        transform.position = Input.mousePosition;
    }


    /// <summary>
    /// Called when the dragging of the UI element ends
    /// </summary>
    /// <param name="eventData">Pointer event data containing drag information.</param>
    public void OnEndDrag(PointerEventData eventData)
    {
        // Reset the UI element's parent and enable raycast target
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }

    // GETTERS AND SETTERS
    public GameObject Skill
    {
        get { return skill; }
        set { skill = value; }
    }

    public bool IsSkill
    {
        get { return isSkill; }
        set { isSkill = value; }
    }

    public bool IsAbility
    {
        get { return isAbility; }
        set { isAbility = value; }
    }
}

