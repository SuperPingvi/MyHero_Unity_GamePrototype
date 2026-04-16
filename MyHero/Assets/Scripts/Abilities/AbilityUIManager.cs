using UnityEngine;

public class AbilityUIManager : MonoBehaviour
{
    public AbilitySlotUI[] slots;

    public PlayerController playerController;
    public AbilityController abilityController;

    private CharacterStats caster;

    public void Start()
    {
        caster = playerController.GetComponent<CharacterStats>();
        playerController.OnAbilitiesChanged += Refresh;
        Refresh();
    }

    private void Update()
    {
        var current = abilityController.currentAbility;

        foreach (var slot in slots)
        {
            slot.UpdateUI(current, caster);
        }
    }

    public void Refresh()
    {
        var abilities = playerController.GetAbilities();

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < abilities.Length)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].SetAbility(abilities[i]);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }
}
