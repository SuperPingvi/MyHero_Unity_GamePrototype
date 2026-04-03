using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Interactable
{
    public CharacterStats selfStats;
    public CharacterStats playerStats;
    public HeroController heroController;
    // Start is called before the first frame update
    void Start()
    {
        if (heroController == null) heroController = GetComponent<HeroController>();
        playerStats = PartyManager.instance.player.GetComponent<CharacterStats>();
        if (selfStats == null) selfStats = GetComponent<CharacterStats>();
    }

    public override void Interact()
    {
        base.Interact();
        var ac = playerStats.GetComponent<AbilityController>();
        if (ac != null && ac.HasAbility())
        {
            ac.UseAbility(gameObject);
        }
    }
}
