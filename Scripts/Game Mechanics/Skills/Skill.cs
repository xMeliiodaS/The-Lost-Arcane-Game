using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill")]
[System.Serializable]

public class Skill : ScriptableObject
{
    public int id;                      // ID field for serialization
    new public string name;             // The Item name
    public int damageModifier;          // Damage modifier for this skill
    public Sprite icon = null;          // Icon for the item
    public bool isDefaultItem = false;  // if the items is default
    public string content;              // The content of the item
    public Sprite skillIcon;            // The skill's icon
    public string equipType;            


    public virtual void use()
    {
        // Use the item
        // Something might happen
        Debug.Log("Using " + name);
    }
}
