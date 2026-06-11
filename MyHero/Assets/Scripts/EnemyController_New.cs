using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController_New : MonoBehaviour
{
    NavMeshAgent agent;

    // States
    public enum State { Wandering, Chasing, Attacking, Stunned }
    public State state;
    
    public enum AttackType { Detached, Attached }
    public AttackType attackType;

    // Wandering
    public float wanderingRadius = 5f;
    Vector3 wanderingArea;
    Vector3 walkPoint;
    bool walkPointIsSet;

    // Finding and Chasing
    public float lookRadius = 10f;
    Transform target;
    bool targetIsSet;
    public GameObject player, hero;
    bool playerIsInSightRange, heroIsInSightRange;
    public float targetCommitDuration = 2f;
    float targetCommitTimer;

    // Attacking
    private Coroutine attackCoroutine;
    bool hasAttacked;
    float attackTime;
    public float attackInterval = 3f;
    public float attackRange = 8f;
    public float telegraphDuration = 1.2f; // How long the indicator shows before firing
    [Range(0f, 1f)]
    public float aimToLockRatio = 0.4f; // 0 = all aim, 1 = all lock
    public float rotationSpeed = 8f;
    
    [Header ("Detached Indicator")] // Ground circle shown during telegraph
    public AttackIndicator attackIndicator;
    
    [Header("Detached")]
    public GameObject mortarPrefab;
    
    [Header ("HitCollider (Attached Hit)")]
    public GameObject hitCollider;
    public float hitWindow = 0.2f;

    // Stun
    public float stunDuration;
    float stunTimer;
    bool isStunned;
    
    //Visual effects
    EffectVisual effectVisual;
    public Sprite stunnedSprite;
    public GameObject attackVFX;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        wanderingArea = transform.position;
        player = PartyManager.instance.player;
        hero = PartyManager.instance.hero;
        if (hitCollider != null)
            hitCollider.GetComponent<DamageCollider>().damage = GetComponent<CharacterStats>().damage;
        effectVisual = GetComponent<EffectVisual>();
    }

    void Update()
    {
        switch (state)
        {
            default:
            case State.Wandering:
                CheckingDistances();
                Wandering();
                break;
            case State.Chasing:
                Chasing();
                break;
            case State.Attacking:
                Attacking();
                break;
            case State.Stunned:
                BeingStunned();
                break;
        }
    }

    void Wandering()
    {
        if (playerIsInSightRange && !targetIsSet)
        {
            target = player.transform;
            targetIsSet = true;
            targetCommitTimer = 0f;
            walkPointIsSet = false;
            state = State.Chasing;
            playerIsInSightRange = false;
        }
        else if (heroIsInSightRange && !targetIsSet)
        {
            target = hero.transform;
            targetIsSet = true;
            targetCommitTimer = 0f;
            walkPointIsSet = false;
            state = State.Chasing;
            heroIsInSightRange = false;
        }
        else if (!targetIsSet)
            MoveToNewWalkpoint();
    }

    void Chasing()
    {
        if (target == null) { state = State.Wandering; return; }

        agent.destination = target.position;
        float targetDistance = Vector3.Distance(target.position, transform.position);

        if (targetDistance < attackRange)
        {
            state = State.Attacking;
            return;
        }

        targetCommitTimer += Time.deltaTime;
        if (targetCommitTimer < targetCommitDuration)
            return;

        // Commitment window expired — re-evaluate
        if (targetDistance > lookRadius)
        {
            target = null;
            targetIsSet = false;
            targetCommitTimer = 0f;
            walkPointIsSet = false;
            state = State.Wandering;
            return;
        }

        // Check if the other unit is closer and within range
        GameObject other = (target.gameObject == player) ? hero : player;
        float otherDistance = Vector3.Distance(other.transform.position, transform.position);
        if (otherDistance < targetDistance && otherDistance < lookRadius)
        {
            target = other.transform;
            targetCommitTimer = 0f;
        }
    }

    void Attacking()
    {
        if (!hasAttacked)
        {
            attackTime = Time.time + attackInterval;
            hasAttacked = true;
            attackCoroutine = StartCoroutine(AnimateAttack());
            // target stays alive during coroutine, nulled inside it after aim phase
        }

        MoveToNewWalkpoint();

        if (hasAttacked && Time.time > attackTime)
        {
            agent.ResetPath();
            walkPointIsSet = false;
            state = State.Wandering;
            hasAttacked = false;
        }
    }

    void CheckingDistances()
    {
        float heroDistance = Vector3.Distance(hero.transform.position, transform.position);
        float playerDistance = Vector3.Distance(player.transform.position, transform.position);
        if (!targetIsSet && heroDistance < lookRadius && heroDistance <= playerDistance)
            heroIsInSightRange = true;
        else if (!targetIsSet && playerDistance < lookRadius && playerDistance < heroDistance)
            playerIsInSightRange = true;
    }

    void BeingStunned()
    {
        hasAttacked = false;
        walkPointIsSet = false;
        targetIsSet = false;
        target = null;
        agent.isStopped = true;

        if (!isStunned)
        {
            stunTimer = Time.time + stunDuration;
            isStunned = true;
            effectVisual?.ShowEffect(stunnedSprite);
        }
        else if (isStunned && Time.time > stunTimer)
        {
            agent.isStopped = false;
            state = State.Wandering;
            isStunned = false;
            effectVisual?.ClearEffect();
        }
    }

    IEnumerator AnimateAttack()
    {
        agent.isStopped = true;
        
        attackIndicator?.Show(telegraphDuration);

        // Phase 1 — Aim (target still alive, track real position)
        float elapsed = 0f;
        float aimDuration = telegraphDuration * (1f - aimToLockRatio);

        while (elapsed < aimDuration)
        {
            if (target == null) break;
            Vector3 livePos = target.position;

            if (attackType == AttackType.Detached)
                attackIndicator.transform.position = new Vector3(livePos.x, 1.5f, livePos.z);

            Vector3 dir = (livePos - transform.position).normalized;
            if (dir != Vector3.zero)
            {
                Quaternion look = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * rotationSpeed);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Lock — snapshot and null target
        if (target == null)
        {
            agent.isStopped = false;
            attackIndicator?.Hide();
            yield break;
        }
        Vector3 targetPoint = new Vector3(target.position.x, 0.5f, target.position.z);
        targetIsSet = false;
        target = null;

        // Phase 2 — Lock
        yield return new WaitForSeconds(telegraphDuration * aimToLockRatio);

        // Phase 3 — Fire
        attackIndicator?.Hide();

        if (attackType == AttackType.Detached)
        {
            GameObject impact = Instantiate(mortarPrefab, targetPoint, Quaternion.identity);
            impact.GetComponent<DamageCollider>().damage = GetComponent<CharacterStats>().damage;
        }
        else if (attackType == AttackType.Attached)
        {
            hitCollider.SetActive(true);
            attackVFX?.SetActive(true);
            yield return new WaitForSeconds(hitWindow);
            hitCollider.SetActive(false);
            attackVFX?.SetActive(false);
        }

        agent.isStopped = false;
    }

    void MoveToNewWalkpoint()
    {
        float randomX = Random.Range(-wanderingRadius, wanderingRadius);
        float randomZ = Random.Range(-wanderingRadius, wanderingRadius);
        float distanceToWalkpoint = Vector3.Distance(walkPoint, transform.position);
        if (!walkPointIsSet)
        {
            walkPoint = new Vector3(wanderingArea.x + randomX, transform.position.y, wanderingArea.z + randomZ);
            walkPointIsSet = true;
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(walkPoint, path);
            if (path.status == NavMeshPathStatus.PathComplete)
                agent.SetDestination(walkPoint);
            else
                walkPointIsSet = false;
        }
        if (distanceToWalkpoint < 0.3f)
            walkPointIsSet = false;
    }
    
    public void TriggerStun(float duration)
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
            attackIndicator?.Hide();
            hitCollider?.SetActive(false);
            attackVFX?.SetActive(false);
            hasAttacked = false;
            agent.isStopped = false;
        }

        stunDuration = duration;
        state = State.Stunned;
    }
}
