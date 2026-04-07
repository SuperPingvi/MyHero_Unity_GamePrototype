using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Interactable
{
    public CharacterStats selfStats;
    public HeroController heroController;
    // Start is called before the first frame update
    void Start()
    {
        if (heroController == null) heroController = GetComponent<HeroController>();
        if (selfStats == null) selfStats = GetComponent<CharacterStats>();
    }

    public override void Interact()
    {
        base.Interact();
        var ac = player?.GetComponent<AbilityController>();
        Debug.Log($"Hero.Interact called. player={player}, ac={ac}, hasAbility={ac?.HasAbility()}");

        if (ac != null && ac.HasAbility())
        {
            Debug.Log($"Firing ability on {selfStats}");

            var targetStats = GetComponent<CharacterStats>();
            ac.UseOnTarget(targetStats);
        }
    }
}
