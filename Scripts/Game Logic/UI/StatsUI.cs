using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerLevel;
    [SerializeField] private TextMeshProUGUI playerTotalEXP;
    [SerializeField] private TextMeshProUGUI playerHP;
    [SerializeField] private TextMeshProUGUI playerDamage;
    [SerializeField] private TextMeshProUGUI playerBossDamage;
    [SerializeField] private TextMeshProUGUI playerCriticalRate;
    [SerializeField] private TextMeshProUGUI playerCriticalDamage;
    [SerializeField] private TextMeshProUGUI playerDefense;


    private void Update()
    {
        if(gameObject.activeSelf)
        {
            PlayerStat playerStat = FindAnyObjectByType<PlayerStat>();
            playerLevel.text = playerStat.CurrentLevel.ToString();
            playerTotalEXP.text = playerStat.TotalExp.ToString();
            playerHP.text = playerStat.CurrentHealth.ToString();
            playerDamage.text = playerStat.Damage.GetValue().ToString();
            playerBossDamage.text = playerStat.BossDamage.ToString() + "%";
            playerCriticalRate.text = playerStat.CriticalRate.ToString() + "%";
            playerCriticalDamage.text = playerStat.CriticalDamage.ToString() + "%";
            playerDefense.text = playerStat.Armor.GetValue().ToString();
        }

    }


}
