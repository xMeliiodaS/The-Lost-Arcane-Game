using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private GameObject weapon;     // The weapon for the root game object (Player / enemy)

    /// <summary>
    /// Enables or disables the collider of the player's held weapon based on the 'isEnable' parameter.
    /// If 'isEnable' is 1, the collider is enabled and a swing sound effect is played.
    /// If 'isEnable' is 0, the collider is disabled.
    /// </summary>
    /// <param name="isEnable">An integer representing whether to enable (1) or disable (0) the collider.</param>
    public void EnableWeaponCollider(int isEnable)
    {
        // Check if the player is holding a weapon
        if (weapon != null)
        {
            var boxCollider = weapon.GetComponent<BoxCollider>();
            var meshCollider = weapon.GetComponent<MeshCollider>();

            // Check if the weapon has a BoxCollider
            if (boxCollider != null)
            {
                if (isEnable == 1)
                {
                    boxCollider.enabled = true;
                    try
                    {
                        AudioManager.Instance.Play(GetRandomSwingEffect());
                    }
                    catch { }
                }
                else
                {
                    boxCollider.enabled = false;
                }
            }

            // Check if the weapon has a MeshCollider
            if (meshCollider != null)
            {
                if (isEnable == 1)
                {
                    meshCollider.enabled = true;

                    try
                    {
                        AudioManager.Instance.Play(GetRandomSwingEffect());
                    }
                    catch { }
                }
                else
                {
                    meshCollider.enabled = false;
                }

            }
        }
    }


    /// <summary>
    /// Gets a randomly selected swing effect SFX name from the array of swing effect names.
    /// </summary>
    /// <returns>A string representing the name of a randomly selected swing effect SFX.</returns>
    public static string GetRandomSwingEffect()
    {
        string[] swingEffects = { "SwingEffect1", "SwingEffect2" };
        return swingEffects[Random.Range(0, swingEffects.Length)];
    }

    public GameObject Weapon
    {
        get { return weapon; }
        set { weapon = value; }
    }
}
