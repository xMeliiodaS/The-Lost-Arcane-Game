using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem instance;
    public void Start()
    {
        instance = this;
    }

    public Tooltip1 tooltip;

    public static void Show(string content, Sprite icon, string rarity, string header, int damage, int armor)
    {
        instance.tooltip.SetText(content, icon, rarity, header, damage, armor);

        if(content != string.Empty)
        {
            instance.tooltip.gameObject.SetActive(true);
        }
        else
        {
            instance.tooltip.gameObject.SetActive(false);
        }
    }

    public static void Hide()
    {
        instance.tooltip.gameObject.SetActive(false);
    }
}
