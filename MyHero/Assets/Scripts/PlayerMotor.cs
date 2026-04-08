using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMotor : MonoBehaviour
{
    Transform target;
    
    NavMeshAgent agent;
    public float speed = 6;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }
    public void MoveToPoint(Vector3 point)
    {
        agent.SetDestination(point);
    }
    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
            FaceTarget();
        }
        
    }
    public void FollowTarget(Interactable newTarget)
    {
        agent.stoppingDistance = newTarget.radius * .8f;
        
        // agent.updateRotation = false;
        
        target = newTarget.interactionSpace;
    }
    public void StopFollowingTarget()
    {
        agent.stoppingDistance = 0f;
        agent.updateRotation = true;
        
        target = null;
        agent.ResetPath();
    }
    public void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8f);
    }

    public void SnapFaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
    }
    public void SynchronizeSpeed()
    {
        //if target moves, set speed equals to target's speed, to follow smoothly
        //get target's speed
        //set our speed
        //reset speed upon clearing target
    }
}
