using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControllerFSM : MonoBehaviour
{
    NavMeshAgent agent;

    //States
    public enum State {Wandering, Chasing, Attacking, Stunned}
    public State state;

    //Wandering
    public float wanderingRadius = 5;
    Vector3 wanderingArea;
    Vector3 walkPoint;
    bool walkPointIsSet;

    //Finding and Chasing HERO or Player;
    public float lookRadius = 10f;
    Transform target;
    bool targetIsSet;
    public GameObject player, hero;
    bool playerIsInSightRange, heroIsInSightRange;

    //Attacking
    bool hasAttacked;
    float attackTime;
    public float attackInterval = 2f;
    public float attackRange = 1.3f;
    public float attackDelay = 0.5f;
    public GameObject hitCollider;
    public Animator enemyAnimator;
    public GameObject attackIndicator;
    public GameObject attackVFX;

    //Receive attack or being stunned
    public float stunDuration;
    float stunTimer;
    bool isStunned;
    
    //Visual effects
    EffectVisual effectVisual;
    public Sprite stunnedSprite;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        wanderingArea = transform.position;
        player = PartyManager.instance.player;
        hero = PartyManager.instance.hero;
        effectVisual = GetComponent<EffectVisual>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            default:
            case State.Wandering:
                {
                    CheckingDistances();
                    Wandering();
                }
                break;
            case State.Chasing: Chasing();

                break;
            case State.Attacking: Attacking();

                break;
            case State.Stunned: BeingStunned();
                break;
        }
    }
    void Wandering()
    {

        if (playerIsInSightRange && !targetIsSet)
        {
            target = player.transform;
            targetIsSet = true;
            walkPointIsSet = false;
            state = State.Chasing;
            playerIsInSightRange = false;
        }
        else if (heroIsInSightRange && !targetIsSet)
        {
            target = hero.transform;
            targetIsSet = true;
            walkPointIsSet = false;
            state = State.Chasing;
            heroIsInSightRange = false;
        }
        else if (!targetIsSet)
            MoveToNewWalkpoint();
    }
    void Chasing()
    {
        agent.destination = target.position;
        float targetDistance = Vector3.Distance(target.position, transform.position);
        if (targetDistance < attackRange)
            state = State.Attacking;
    }
    void Attacking()
    {
        if (!hasAttacked)
        {
            attackTime = Time.time + attackInterval;
            hasAttacked = true;
            targetIsSet = false;
            target = null;
            StartCoroutine(AnimateAttack());
            //After has attacked set off wandering for attack interval time        
            //WalkAwayAfterAttack();
        }
        MoveToNewWalkpoint();
        if (hasAttacked && Time.time > attackTime)
        {
            agent.destination = transform.position;
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

    public void TriggerStun(float duration)
    {
        stunDuration = duration;
        state = State.Stunned;
    }
    
    void BeingStunned()
    {
        hasAttacked = false;
        walkPointIsSet = false;
        targetIsSet = false;
        target = null;
        agent.isStopped=true;

        if (!isStunned)
        {
            stunTimer = Time.time + stunDuration;
            isStunned = true;
            effectVisual?.ShowEffect(stunnedSprite);
        }
        else if(isStunned && Time.time > stunTimer)
        {
            agent.isStopped = false;
            state = State.Wandering;
            isStunned = false;
            effectVisual?.ClearEffect();
        }
    }
    /*void WalkAwayAfterAttack()
    {
        targetIsSet = false;
        target = null;
        MoveToNewWalkpoint();
        
    }*/
    IEnumerator AnimateAttack()
    {
        // Windup
        attackIndicator.SetActive(true);
        enemyAnimator.SetTrigger("Attack");
        agent.isStopped = true;
        yield return new WaitForSeconds(attackDelay);
    
        // Hit window
        attackIndicator.SetActive(false);
        hitCollider.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        hitCollider.SetActive(false);
    
        // Wait for rest of animation to finish
        yield return new WaitForSeconds(enemyAnimator.GetCurrentAnimatorStateInfo(0).length - attackDelay - 0.2f);
    
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
            {
                agent.SetDestination(walkPoint);
            }
            else
            {
                walkPointIsSet = false;
            } 
        }
        if (distanceToWalkpoint < 0.3f)
        {
            walkPointIsSet = false;
        }     
    }


}
