using UnityEngine;

public class WeaponAttribiutesForNPC : MonoBehaviour
{
    private readonly string enemy = "Enemy";

    /// <summary>
    /// When the enemy weapon hit the player collider he take damage accoriding to the enemy's Damage
    /// </summary>
    /// <param name="other"> The Player's Collider </param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enemy))
        {
            other.GetComponent<CharacterStats>().TakeDamage(20);
        }
    }
}
