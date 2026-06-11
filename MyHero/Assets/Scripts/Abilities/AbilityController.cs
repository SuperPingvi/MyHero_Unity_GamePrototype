using UnityEngine;

public class AbilityController : MonoBehaviour
{
    public Ability currentAbility;

    private CharacterStats caster;

    private void Awake()
    {
        caster = GetComponent<CharacterStats>();
    }

    public void SetAbility(Ability ability)
    {
        currentAbility = ability;
    }

    public void ClearAbility()
    {
        currentAbility?.GetIndicator()?.SetActive(false);
        currentAbility = null;
    }

    public bool UseOnTarget(CharacterStats target)
    {
        if (currentAbility == null) return false;

        AbilityContext context = new AbilityContext(caster, target);
        bool success = currentAbility.TryUse(context);

        if (success)
            ClearAbility();

        return success;
    }

    public bool UseOnPoint(Vector3 point)
    {
        if (currentAbility == null) return false;

        AbilityContext context = new AbilityContext(caster, null, point);
        bool success = currentAbility.TryUse(context);

        if (success)
            ClearAbility();

        return success;
    }

    public bool HasAbility()
    {
        return currentAbility != null;
    }
}