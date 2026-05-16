using UnityEngine;
using System.Linq;

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
    
    public event System.Action OnAbilitiesChanged;
    
    private bool previouslyHadAbility;
    private bool isAbilityAOE;
    
    private PlayerInputActions inputActions;

    void Awake()
    {
        motor = GetComponent<PlayerMotor>();
        abilities = GetAbilities();
        Debug.Log($"Found {abilities.Length} abilities on GameObject");
        inputActions = new PlayerInputActions();
    }
    
    void Start()
    {
        inputActions.Enable();
        inputActions.Player.SelectAbility_Q.performed += ctx => SelectAbility(0);
        inputActions.Player.SelectAbility_W.performed += ctx => SelectAbility(1);
        inputActions.Player.SelectAbility_E.performed += ctx => SelectAbility(2);
        // inputActions.Player.SelectAbility_R.performed += ctx => SelectAbility(3);
    }
    
    public Ability[] GetAbilities()
    {
        return GetComponents<Ability>().Where(a => a.enabled).ToArray();
    }

    public void RefreshAbilities()
    {
        abilities = GetAbilities();
        OnAbilitiesChanged?.Invoke();
    }

    void SelectAbility(int index)
    {
        Debug.Log($"SelectAbility called, index: {index}, abilities count: {abilities?.Length}");

        if (abilities == null || index >= abilities.Length)
        {
            Debug.Log($"No ability in slot {index}");
            return;
        }
        
        if (!abilities[index].CanSelect())
            return;
        
        abilityController.SetAbility(abilities[index]);
        Debug.Log($"Ability selected: {abilities[index].abilityName}");

        if (abilities[index].castType == Ability.AbilityCastType.AOE)
        {
            isAbilityAOE = true;
            abilities[index].GetIndicator()?.SetActive(true);
            Debug.Log("AOE ability selected, indicator should be active");
        }
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
            isAbilityAOE = false;
        }
        
        previouslyHadAbility = hasAbility;

        if (hasAbility)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, clickMaskLMB))
            {
                Vector3 dir = (hit.point - transform.position).normalized;
                dir.y = 0f;
                if (dir != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(dir);

                if (isAbilityAOE)
                    abilityController.currentAbility.GetIndicator().transform.position =
                        new Vector3(hit.point.x, hit.point.y + 2.1f, hit.point.z);
            }
        }
        
        if (Input.GetMouseButton(1) && Time.timeScale != 0f && canMove)
        {
            if (abilityController != null && abilityController.HasAbility())
            {
                abilityController.ClearAbility();
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
            if (abilityController == null || !abilityController.HasAbility())
                return;

            if (isAbilityAOE)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, clickMaskLMB))
                {
                    abilityController.UseOnPoint(hit.point);
                    isAbilityAOE = false;
                }
            }

            else
            {
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
}
