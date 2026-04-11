using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HeroController : MonoBehaviour
{
    public NavMeshAgent agent;
    public LayerMask enemy, ground;
    public GameObject player;

    //States
    enum State { Advancing, Chasing, Attacking}
    [SerializeField] State state;

    //Advancing
    public Vector3 currentWalkPoint;
    int walkPointsReached = 0;
    int amoutOfWalkPoints;
    [SerializeField]bool walkPointIsSet = false;
    public GameObject[] walkPoints;

    //Enemy chase and attack
    public float lookRadius = 10f;
    public float attackRadius = 1.5f;
    public float attackCooldown = 1f;
    float attackTimer;
    public GameObject slashCollider;
    public Animator swordAnimator;
    Transform target;
    [SerializeField]bool targetIsSet = false;
    
 
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        amoutOfWalkPoints = walkPoints.Length;
        player = PartyManager.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            default:
                case State.Advancing: CheckForTargets(); Advancing();
                    break;
            case State.Chasing: ChaseEnemy();
                break;
            case State.Attacking: Attack();
                break;
        }
    }
    void SetWalkPoint()
    {
        int i = walkPointsReached;
        if (i < amoutOfWalkPoints)
            currentWalkPoint = walkPoints[i].transform.position;
    }
    void Advancing()
    {
        agent.updateRotation = true;
        float distanceToDestination = Vector3.Distance(currentWalkPoint, transform.position);
        if(walkPointIsSet && distanceToDestination < 0.5f && walkPointsReached < amoutOfWalkPoints)
        {
            walkPointsReached++;
            walkPointIsSet = false;
        }
        if (!walkPointIsSet)
        {
            SetWalkPoint();
            agent.SetDestination(currentWalkPoint);
            walkPointIsSet = true;
        }
        //Stop HERO when Player is far away.
        /*float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if(distanceToPlayer > lookRadius)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }*/
    }
    void ChaseEnemy()
    {
        if (walkPointIsSet) walkPointIsSet = false;
        float targetDistance = Vector3.Distance(target.position, transform.position);
        if(targetDistance > attackRadius)
        { 
            agent.SetDestination(target.position);
        }
        else if(targetDistance <= attackRadius)
        { 
            agent.SetDestination(transform.position);
            state = State.Attacking;
        }  
    }
    void Attack()
    {
        FaceTarget();
        if (targetIsSet)
        {
            attackTimer = Time.time + attackCooldown;
            swordAnimator.SetTrigger("Attack");
            slashCollider.SetActive(true);
            DoDelay();

            //Attack animation, sound etc.   
            targetIsSet = false;
        }
        else if (Time.time >= attackTimer)
        { 
            target = null;
            state = State.Advancing;
        }
    }
    void CheckForTargets()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, lookRadius, enemy);
        int targetsInSight = targets.Length;
        
        if(targetsInSight > 0 && !targetIsSet)
        {
            target = targets[0].gameObject.transform;
            targetIsSet = true;
            state = State.Chasing;
        }
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.3f);
        slashCollider.SetActive(false);
    }
    void DoDelay()
    {
        StartCoroutine(Delay());
    }
    void FaceTarget()
    {
        agent.updateRotation = false;
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8f);
    }
}
