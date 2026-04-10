using UnityEngine;

public class AbilityUIManager : MonoBehaviour
{
    public AbilitySlotUI[] slots;

    public PlayerController playerController;
    public AbilityController abilityController;

    private CharacterStats caster;

    private void Start()
    {
        caster = playerController.GetComponent<CharacterStats>();

        var abilities = playerController.GetAbilities();

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < abilities.Length)
            {
                slots[i].SetAbility(abilities[i]);
            }
        }
    }

    private void Update()
    {
        var current = abilityController.currentAbility;

        foreach (var slot in slots)
        {
            slot.UpdateUI(current, caster);
        }
    }
}
