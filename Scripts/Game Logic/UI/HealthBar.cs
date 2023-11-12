using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [SerializeField] private Slider slider;     // Slider of any Bar (Player HP, EXP, enemy HP..)
    private TextMeshProUGUI playerNameText;     // Player name text


    private static int index = 0;
    private void Start()
    {
        if (index == 0)
        {
            StartCoroutine(GetPlayerNameAfterSecond());
            index++;
        }
        slider.value = slider.maxValue;
    }


    // Execute this function after 1 second
    IEnumerator GetPlayerNameAfterSecond()
    {
        yield return new WaitForSeconds(1f);
        if(string.IsNullOrEmpty(playerNameText.text))
        {
            Transform player = FindPlayer.Instance.Player.transform;    // Get the player's reference
            playerNameText.text = player.GetComponent<PlayerStat>().PlayerName;    // Set the name of the player
        }
    }

    public void SetMax(int health)
    {
        slider.maxValue = health;
        //slider.value = health;
    }

    public void SetSliderBar(int health)
    {
        slider.value = health;
    }

    public Slider Slider
    {
        get { return slider; }
        set { slider = value; }
    }

    public void SetSlider(Slider value)
    {
        slider = value;
    }

    public TextMeshProUGUI GetPlayerNameText()
    {
        return playerNameText;
    }

    public void SetPlayerNameText(TextMeshProUGUI value)
    {
        playerNameText = value;
    }

}
