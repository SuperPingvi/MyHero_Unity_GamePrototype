using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityController : MonoBehaviour
{
    private PlayerInputActions input;
    
    public Ability currentAbility;
    Ability[] abilities;

    void Awake()
    {
        input = new PlayerInputActions();
        abilities = GetComponents<Ability>();
    }

    void OnEnable()
    {
        input.Enable();

        input.Player.SelectAbility_Q.performed += OnSelectAbility;
    }

    void OnDisable()
    {
        input.Player.SelectAbility_Q.performed -= OnSelectAbility;
        
        input.Disable();
    }

    void OnSelectAbility(InputAction.CallbackContext ctx)
    {
        currentAbility = abilities[0];
        Debug.Log("Selected ability: " + currentAbility.name);
    }

    public void UseAbility(GameObject target)
    {
        if (currentAbility == null)
            return;
        
        currentAbility.TryUse(target);
        
        currentAbility = null;
    }

    public bool HasAbility()
    {
        return currentAbility != null;
    }

    public void ClearAbility()
    {
        currentAbility = null;
    }
}
