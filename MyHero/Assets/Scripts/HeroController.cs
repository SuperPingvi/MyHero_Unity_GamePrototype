using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class HeroController : MonoBehaviour
{
    public event System.Action OnStartedAdvancing;
    public event System.Action OnStartedChasing;
    public event System.Action OnAttackSwing;

    public NavMeshAgent agent;
    public LayerMask enemy, ground;
    public GameObject player;

    //States
    enum State { Advancing, Chasing, Attacking}
    [SerializeField] State state;

    //Advancing
    public Vector3 currentWalkPoint;
    int walkPointsReached;
    int amoutOfWalkPoints;
    [SerializeField]bool walkPointIsSet;
    public GameObject[] walkPoints;

    //Enemy chase and attack
    public float lookRadius = 10f;
    public float attackRadius = 1.5f;
    public float attackCooldown = 1f;
    float attackTimer;
    public GameObject slashCollider;
    public Animator swordAnimator;
    Transform target;
    [SerializeField]bool targetIsSet;
    
 
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        amoutOfWalkPoints = walkPoints.Length;
        player = PartyManager.instance.player;
        slashCollider.GetComponent<DamageCollider>().damage = GetComponent<CharacterStats>().damage;
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

    void SetState(State newState)
    {
        state = newState;
        switch (newState)
        {
            case State.Advancing: OnStartedAdvancing?.Invoke(); break;
            case State.Chasing:   OnStartedChasing?.Invoke();   break;
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
        if (!agent.updateRotation) agent.updateRotation = true;
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
        Transform closeTarget = GetClosestTargetInAttackRange();

        if (closeTarget != null /* && closeTarget != target */)
        {
            target = closeTarget;
            state = State.Attacking; // State.Chasing; To stop Attacking on the go.
            return;
        }

        if (target == null)
        {
            SetState(State.Advancing);
            return;
        }

        if (walkPointIsSet) walkPointIsSet = false;

        float targetDistance = Vector3.Distance(target.position, transform.position);

        if (targetDistance > attackRadius)
        { 
            agent.SetDestination(target.position);
        }
        else
        {
            agent.ResetPath();
            state = State.Attacking;
        }  
    }
    void Attack()
    {
        if (target == null) { SetState(State.Advancing); return; }
        FaceTarget();
        if (!targetIsSet) return;

        targetIsSet = false;
        StartCoroutine(AnimateAttack());
    }

    IEnumerator AnimateAttack()
    {
        swordAnimator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.15f);
    
        OnAttackSwing?.Invoke();
        slashCollider.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        slashCollider.SetActive(false);

        yield return new WaitForSeconds(attackCooldown);
        target = null;
        SetState(State.Advancing);
    }
    
    void CheckForTargets()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, lookRadius, enemy);
        int targetsInSight = targets.Length;
        
        if(targetsInSight > 0 && !targetIsSet)
        {
            target = targets[0].gameObject.transform;
            targetIsSet = true;
            SetState(State.Chasing);
        }
    }
    
    void FaceTarget()
    {
        agent.updateRotation = false;
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8f);
    }
    
    // New "hit what's in your face" check
    Transform GetClosestTargetInAttackRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, enemy);

        float closestDistance = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (var hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);

            if (dist < closestDistance)
            {
                closestDistance = dist;
                bestTarget = hit.transform;
            }
        }

        return bestTarget;
    }
}
