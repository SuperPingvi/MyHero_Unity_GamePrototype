using UnityEngine;

public class HealAbility : Ability
{
    public int healAmount = 50;

    protected override void Use(GameObject target)
    {
        var stats = target.GetComponent<CharacterStats>();
        if (stats != null)
        {
            stats.ModifyHealth(healAmount);
            Debug.Log("Healed " + target.name);
        }
    }
     
    protected override bool IsValidTarget(GameObject target)
    {
        var stats = target.GetComponentInParent<CharacterStats>();
        if (stats == null)
            return false;

        return stats.canBeHealed && !stats.isDead;
    }
}
