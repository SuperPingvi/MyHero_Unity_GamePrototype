using UnityEngine;

public class HealAbility : Ability
{
    public int healAmount = 50;

    protected override void Apply(AbilityContext context)
    {
        Debug.Log($"Apply: target={context.target}, healAmount={healAmount}");
        if (context.target == null) return;
        context.target.ModifyHealth(healAmount);
        Debug.Log($"ModifyHealth called with {healAmount}");
    }

    protected override bool IsValid(AbilityContext context)
    {
        if (context.target == null) return false;

        // только союзники
        if (context.target.faction != context.caster.faction)
            return false;

        if (context.target.currentHealth >= context.target.maxHealth)
            return false;
        
        return true;
    }
}
