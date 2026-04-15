using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    public LayerMask clickMaskRMB;
    public LayerMask clickMaskLMB;
    public Interactable focus;
    public Camera cam;
    PlayerMotor motor;
    [HideInInspector] public bool canMove = true;
    
    [SerializeField] private AbilityController abilityController;
    [SerializeField] private Ability[] abilities;
    
    public Ability[] GetAbilities()
    {
        return abilities;
    }
    
    private bool previouslyHadAbility;
    
    private PlayerInputActions inputActions;

    void Awake()
    {
                motor = GetComponent<PlayerMotor>();
                abilities = GetComponents<Ability>();
                Debug.Log($"Found {abilities.Length} abilities on GameObject");
                inputActions = new PlayerInputActions();
    }
    
    void Start()
    {
        inputActions.Enable();

        inputActions.Player.SelectAbility_Q.performed += ctx => SelectAbility(0);
        inputActions.Player.SelectAbility_W.performed += ctx => SelectAbility(1);
        // inputActions.Player.SelectAbility_E.performed += ctx => SelectAbility(ability_E);
        // inputActions.Player.SelectAbility_R.performed += ctx => SelectAbility(ability_R);
    }

    void SelectAbility(int index)
    {
        Debug.Log($"SelectAbility called, index: {index}, abilities count: {abilities?.Length}");

        if (abilities == null || index >= abilities.Length) return;
        
        var ability = abilities[index];

        if (ability == null)
        {
            Debug.Log("No ability in slot {index}");
            return;
        }
        abilityController.SetAbility(abilities[index]);
        Debug.Log($"Ability selected: {abilities[index].abilityName}");
    }

    // Update is called once per frame
    void Update()
    {
        bool hasAbility = abilityController.HasAbility();

        if (previouslyHadAbility && !hasAbility)
        {
            if (focus != null)
                motor.SnapFaceTarget();
            RemoveFocus();
        }
        
        previouslyHadAbility = hasAbility;
        
        if (Input.GetMouseButton(1) && Time.timeScale != 0f && canMove)
        {
            var ac = GetComponent<AbilityController>();
            if (ac != null && ac.HasAbility())
            {
                ac.ClearAbility();
                Debug.Log("Ability cancelled");
            }
            
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, clickMaskRMB))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    SetFocus(interactable);
                }
                else
                { 
                    RemoveFocus();
                    motor.MoveToPoint(hit.point);
                }
            }

        }
        if (Input.GetMouseButton(0) && Time.timeScale != 0f && canMove)
        {
            var ac = GetComponent<AbilityController>();
            if (ac == null || !ac.HasAbility())
                return;
            
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, clickMaskLMB))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                
                if (interactable != null)
                {
                    SetFocus(interactable);
                }
            }
        }
    }
    void SetFocus(Interactable newFocus)
    {
        if (newFocus != focus)
        {
            if (focus != null)
                focus.OnDefocused();

            focus = newFocus;
            motor.FollowTarget(newFocus);
            newFocus.OnFocused(transform);
        }
    }
        void RemoveFocus(){
        if (focus != null)
            focus.OnDefocused();
            
        focus = null;
        motor.StopFollowingTarget();
        }
    public void StopMovement()
    {
        canMove = false;
    }
    public void EnableMovement()
    {
        canMove = true;
    }

    public void UnlockAbility(int index, Ability ability)
    {
        abilities[index] = ability;
    }
}
