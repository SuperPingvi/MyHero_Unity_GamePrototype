using System.Collections;
using System.Collections.Generic;
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
   

    // Start is called before the first frame update
    void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    // Update is called once per frame
    void Update()
    {
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
        }
        newFocus.OnFocused(transform);
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
