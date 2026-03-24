using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController hud;
    public Text heroHP;
    public Text myHP;
    public Text mana;

    public CharacterStats heroStats, playerStats;
    // Start is called before the first frame update
    void Start()
    {
        hud = this.GetComponent<HUDController>();
        heroStats = PartyManager.instance.hero.GetComponent<CharacterStats>();
        playerStats = PartyManager.instance.player.GetComponent<CharacterStats>();
        UpdateHP();
    }

    public void UpdateHP()
    {
        heroHP.text = heroStats.currentHealth.ToString();
        myHP.text = playerStats.currentHealth.ToString();
        mana.text = heroStats.currentMana.ToString();
    }
}
