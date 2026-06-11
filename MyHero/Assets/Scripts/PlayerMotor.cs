using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMotor : MonoBehaviour
{
    Transform target;
    NavMeshAgent targetAgent;
    bool inFollowRange;

    NavMeshAgent agent;
    public float speed = 6;
    [SerializeField] float followRangePadding = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }
    public void MoveToPoint(Vector3 point)
    {
        bool success = agent.SetDestination(point);
        if (!success)
            Debug.LogWarning($"SetDestination failed for point {point}");
    }
    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            if ((target.position - agent.destination).sqrMagnitude > 0.01f)
                agent.SetDestination(target.position);
            SynchronizeSpeed();
            FaceTarget();
        }
    }
    public void FollowTarget(Interactable newTarget)
    {
        agent.stoppingDistance = newTarget.radius * .8f;
        agent.updateRotation = false;
        target = newTarget.interactionSpace;
        targetAgent = newTarget.GetComponent<NavMeshAgent>();
    }
    public void StopFollowingTarget()
    {
        agent.stoppingDistance = 0f;
        agent.updateRotation = true;
        agent.speed = speed;
        targetAgent = null;
        inFollowRange = false;
        target = null;
        agent.ResetPath();
    }
    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8f);
    }

    public void SnapFaceTarget()
    {
        if (target == null) return;
        Vector3 direction = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
    }
    void SynchronizeSpeed()
    {
        if (targetAgent == null) return;
        float dist = Vector3.Distance(transform.position, target.position);

        if (!inFollowRange && dist <= agent.stoppingDistance)
            inFollowRange = true;
        else if (inFollowRange && dist > agent.stoppingDistance + followRangePadding)
            inFollowRange = false;

        agent.speed = inFollowRange ? targetAgent.velocity.magnitude : speed;
    }
}
