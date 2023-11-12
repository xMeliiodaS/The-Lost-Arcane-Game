using UnityEngine;

public class WeaponAttributes : MonoBehaviour
{
    private CharacterStats character;
    [SerializeField] private Skill skill;

    private int totalDamage = 0;
    private int baseDamage = 0;

    private PlayerStat player = null;
    private CharacterStats targetStats = null;

    private int skillDamage = 0;
    private bool isCritical = false;

    private readonly string enemy = "Enemy";
    private readonly string boss = "Boss";


    /// <summary>
    /// Called when another collider enters the trigger zone of this object.
    /// Applies damage to enemies and bosses based on player's attack and skill modifiers.
    /// </summary>
    /// <param name="other">The collider that entered the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        character = FindPlayer.Instance.Player.GetComponent<CharacterStats>();

        if (other.CompareTag(enemy) || other.CompareTag(boss))
        {
            if (character is PlayerStat)
            {
                // Reset the skill's damage
                skillDamage = 0;
                try
                {
                    skillDamage = skill.damageModifier;
                }
                catch { }

                targetStats = other.GetComponent<CharacterStats>();     // Reference to the target
                player = FindAnyObjectByType<PlayerStat>();             // Reference to the player

                // The base damage before modifications
                baseDamage = character.Damage.GetValue();
                isCritical = Random.value < ((float)player.CriticalRate / 100f);    // Check if the attack is a critical hit

                // Calculate the damage with critical hit bonus if applicable
                totalDamage = isCritical ? (int)(baseDamage *
                                   (1f + (float)player.CriticalDamage / 100f)) : baseDamage;


                if (other.name.Contains(boss))
                {
                    float bossDamageBonus = ((float)player.BossDamage / 100f) * baseDamage;
                    totalDamage += (int)bossDamageBonus;

                    targetStats.TakeDamage(totalDamage + skillDamage, isCritical);
                }
                else if (other.CompareTag(enemy))
                {
                    targetStats.TakeDamage(totalDamage + skillDamage, isCritical);
                }

                IsNormalAttack();
            }
        }
    }

    /// <summary>
    /// Disables the collider after a normal attack to hit only one enemy.
    /// </summary>
    private void IsNormalAttack()
    {
        // If there SkillInfo is null that mean this is a normal attack,
        var hasSkill = GetComponent<SkillInfo>();
        if (hasSkill == null)
        {
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }
}
