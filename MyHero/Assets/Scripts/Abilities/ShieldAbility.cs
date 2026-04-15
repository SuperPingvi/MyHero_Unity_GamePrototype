using System.Collections;
using UnityEngine;

public class ShieldAbility : Ability
{
    public int shieldValue;
    public float duration;

    protected override bool IsValid(AbilityContext context)
    {
        if (context.target == null) return false;

        // Check if Ally
        if (context.target.faction != context.caster.faction)
            return false;

        return true;
    }

    protected override void Apply(AbilityContext context)
    {
        context.target.shieldAmount = shieldValue;
        context.target.OnShieldStateChanged?.Invoke(true);

        // Duration timer
        context.target.StartCoroutine(RemoveShieldAfterTime(context.target));
    }

    private IEnumerator RemoveShieldAfterTime(CharacterStats target)
    {
        yield return new WaitForSeconds(duration);

        target.shieldAmount = 0;
        target.OnShieldStateChanged?.Invoke(false);
    }
}
