using UnityEngine;

public class WeaponAttributesForEnemy : MonoBehaviour
{
    [SerializeField] private CharacterStats character;
    private readonly string player = "Player";
    [SerializeField] private Skill skill;

    /// <summary>
    /// When the enemy weapon hit the player collider he take damage accoriding to the enemy's Damage
    /// </summary>
    /// <param name="other"> The Player's Collider </param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(player))
        {
            if (character is CharacterStats)
            {
                int skillDmg = SkillDamage();
                other.GetComponent<CharacterStats>().TakeDamage(character.Damage.GetValue() + skillDmg);
            }
        }
    }

    /// <summary>
    /// If there is skill attached then we sum the atm and skill damage
    /// </summary>
    /// <returns> Skill's damage </returns>
    private int SkillDamage()
    {
        int damage = 0;
        try
        {
            damage = skill.damageModifier;
        }
        catch { }

        return damage;
    }

    public void SetATM(CharacterStats value)
    {
        character = value;
    }

}
