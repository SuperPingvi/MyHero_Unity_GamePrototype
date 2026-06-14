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
    [SerializeField] bool walkPointIsSet;
    public GameObject[] walkPoints;

    //Enemy chase and attack
    public float lookRadius = 10f;
    public float attackRadius = 1.5f;
    public float attackCooldown = 1f;
    public GameObject slashCollider;
    public Animator swordAnimator;
    [SerializeField] float minApproachTime = 1.5f;
    [SerializeField] float dashDistance = 2f;
    [SerializeField] float dashDuration = 0.12f;
    Transform target;
    bool isAttacking;
    bool shouldDash;
    float lastAttackTime;
    float approachTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        amoutOfWalkPoints = walkPoints.Length;
        player = PartyManager.instance.player;
        slashCollider.GetComponent<DamageCollider>().damage = GetComponent<CharacterStats>().damage;
    }

    void Update()
    {
        DecideState();
        if (state == State.Chasing || state == State.Advancing)
            approachTimer += Time.deltaTime;
        switch (state)
        {
            default:
                case State.Advancing: Advancing();
                    break;
            case State.Chasing: ChaseEnemy();
                break;
            case State.Attacking: Attack();
                break;
        }
    }

    // Single decision step: the only place that reads/writes target and picks state.
    // Skipped while a swing coroutine is in flight so Hero isn't yanked mid-attack.
    void DecideState()
    {
        if (isAttacking) return;

        // Attack tier: keep current target if still alive and in range, else find closest.
        // Cooldown check here only — chasing and advancing are freely available once isAttacking clears.
        Transform attackTarget = IsValidInRadius(target, attackRadius) ? target : GetClosest(attackRadius);
        if (attackTarget != null && Time.time >= lastAttackTime + attackCooldown)
        {
            target = attackTarget;
            if (state != State.Attacking) SetState(State.Attacking);
            return;
        }

        // Chase tier: same stickiness within lookRadius.
        Transform chaseTarget = IsValidInRadius(target, lookRadius) ? target : GetClosest(lookRadius);
        if (chaseTarget != null)
        {
            target = chaseTarget;
            if (state != State.Chasing) SetState(State.Chasing);
            return;
        }

        // No target in any tier.
        target = null;
        if (state != State.Advancing) SetState(State.Advancing);
    }

    void SetState(State newState)
    {
        state = newState;
        switch (newState)
        {
            case State.Advancing:
                walkPointIsSet = false;     // re-issue destination on next Advancing() tick
                agent.updateRotation = true;
                OnStartedAdvancing?.Invoke();
                break;
            case State.Chasing:
                agent.updateRotation = true;
                OnStartedChasing?.Invoke();
                break;
            case State.Attacking:
                shouldDash = approachTimer >= minApproachTime;
                approachTimer = 0f;
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
        if (!agent.updateRotation) agent.updateRotation = true;
        float distanceToDestination = Vector3.Distance(currentWalkPoint, transform.position);
        if (walkPointIsSet && distanceToDestination < 0.5f && walkPointsReached < amoutOfWalkPoints)
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
    }

    void ChaseEnemy()
    {
        agent.SetDestination(target.position);
    }

    void Attack()
    {
        agent.ResetPath();
        FaceTarget();
        if (!isAttacking)
        {
            isAttacking = true;
            StartCoroutine(AnimateAttack(shouldDash));
        }
    }

    IEnumerator AnimateAttack(bool dash)
    {
        swordAnimator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.15f);

        if (dash) StartCoroutine(DashCoroutine(transform.forward));
        lastAttackTime = Time.time;
        OnAttackSwing?.Invoke();
        slashCollider.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        slashCollider.SetActive(false);

        target = null;
        isAttacking = false;  // swing is visually done; DecideState unblocked

        yield return new WaitForSeconds(attackCooldown);  // cooldown runs in background
    }

    IEnumerator DashCoroutine(Vector3 direction)
    {
        agent.updatePosition = false;
        Vector3 start = transform.position;
        Vector3 end = start + direction * dashDistance;
        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, elapsed / dashDuration);
            yield return null;
        }
        transform.position = end;
        agent.Warp(end);
        agent.updatePosition = true;
    }

    void FaceTarget()
    {
        agent.updateRotation = false;
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8f);
    }

    bool IsValidInRadius(Transform t, float radius)
    {
        return t != null && Vector3.Distance(t.position, transform.position) <= radius;
    }

    Transform GetClosest(float radius)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemy);
        float closest = Mathf.Infinity;
        Transform best = null;
        foreach (var hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closest) { closest = dist; best = hit.transform; }
        }
        return best;
    }
}
