using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip1 : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public Image icon;
    public LayoutElement layoutElement;
    public TextMeshProUGUI rarity;
    public Image typeColor;
    public int characterWrapLimit;

    [SerializeField] private TextMeshProUGUI damage;
    [SerializeField] private TextMeshProUGUI armor;


    public void SetText(string content, Sprite icon, string rarity , string header, int damage, int armor)
    {
        AudioManager.Instance.Play("OnItemHoverEffect");

        typeColor.color = Color.white;
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        contentField.text = content;
        this.icon.sprite = icon;

        if(rarity != null)
        {
            this.rarity.text = rarity;
        }


        this.damage.text = "Damage: " + damage.ToString();

        if(armor > 0)
        {
            this.armor.text = "Defense:  " + armor.ToString();
        }
        else
        {
            this.armor.text = "";
        }
        
        SetTypeColor();
    }
    public void SetTypeColor()
    {
        switch (rarity.text)
        {
            case "Common":
                rarity.color = Color.gray;
                break;
            case "Rare":
                rarity.color = new Color(0.1019608f, 0.8f, 1f); ;
                break;
            case "Epic":
                rarity.color = new Color(0.8841986f, 0.4766821f, 0.9811321f); // Purple color
                break;
            case "Legendary":
                rarity.color = new Color(1f, 0.665265f, 0f); // Orange color
                break;
            default:
                rarity.color = Color.white; // Default color if none of the cases match
                break;
        }

    }


    private void Update()
    {
        if(!Cursor.visible)
        {
            TooltipSystem.Hide();
        }

        if (Application.isEditor)
        {
            int headerLength = headerField.text.Length; // Get the length of the text in the headerField
            int contentLength = contentField.text.Length; // Get the length of the text in the contentField
            layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit); // Enable or disable the layoutElement based on the text length
        }

        Vector2 position = Input.mousePosition; // Get the current mouse position
        transform.position = position; // Set the position of the transform to the mouse position
    }
}
