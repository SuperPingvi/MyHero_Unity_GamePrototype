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

    public void UseOnTarget(CharacterStats target)
    {
        if (currentAbility == null) return;

        AbilityContext context = new AbilityContext(caster, target);

        bool success = currentAbility.TryUse(context);

        if (success)
        {
            ClearAbility();
        }
    }

    public void UseOnPoint(Vector3 point)
    {
        if (currentAbility == null) return;

        AbilityContext context = new AbilityContext(caster, null, point);

        bool success = currentAbility.TryUse(context);

        if (success)
        {
            ClearAbility();
        }
    }

    public bool HasAbility()
    {
        return currentAbility != null;
    }
}