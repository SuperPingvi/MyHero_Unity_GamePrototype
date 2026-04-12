using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotUI : MonoBehaviour
{
    public Image abilityIcon;
    public Image cooldownOverlay;

    private Ability ability;

    public void SetAbility(Ability ability)
    {
        this.ability = ability;

        if (ability != null)
        {
            abilityIcon.sprite = ability.abilityIcon;
        }
    }

    public void UpdateUI(Ability currentAbility, CharacterStats caster)
    {
        if (ability == null) return;
        
        bool isSelected = currentAbility == ability;
        bool canUse = ability.CanUse() && caster.currentMana >= ability.manaCost;

        if (!canUse) abilityIcon.sprite = ability.iconInactive;

        else if (isSelected) abilityIcon.sprite = ability.iconSelected;

        else abilityIcon.sprite = ability.abilityIcon;

        // кулдаун
        float cd = ability.GetRemainingCooldown();
        float fill = ability.cooldown > 0 ? cd / ability.cooldown : 0f;
        cooldownOverlay.fillAmount = fill;
    }
}
