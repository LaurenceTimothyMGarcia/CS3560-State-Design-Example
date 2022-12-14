using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;


/*** 
 * 
 * Main Class
 * Runs the agent
 * 
***/
public class EnemyAIStateDesign : MonoBehaviour
{
    //Any variable with [SeralizeField] will show up in inspector to edit values later on

    //State of the enemy agent
    private State state;

    //Variables to initialize the enemy AI
    [SerializeField] public NavMeshAgent enemy;
    [SerializeField] public Transform player;

    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private LayerMask whatIsGround;

    //Deals with enemy's current movement speed
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float chaseSpeed;
    private float speed;

    //Patrol Variables
    [SerializeField] private Vector3 walkPoint;
    [SerializeField] private float walkPointRange;
    [SerializeField] private float timeWalk;

    //Check if the player is in range for attack or chase
    [SerializeField] private float sightRange;
    [SerializeField] private bool playerInRange;

    //Attack Variables
    [SerializeField] private float attackRange;
    [SerializeField] private bool playerInAttackRange;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private GameObject projectile;

    //Debug
    [SerializeField] private TextMeshProUGUI debugWalking;

    // Update is called once per frame
    void Update()
    {
        //Checks if player is in chase range and attack range
        //Returns boolean if player is within the radius
        playerInRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        /*** 
         * 
         * State changes here
         * 
        ***/

        //Enters attack state if player is within chase range and attack range
        if (playerInRange && playerInAttackRange)
        {
            state = new AttackState(new EnemyAIStateDesign());
        }

        //Enters chase state if player is within the chase range
        //but out of attack range
        else if (playerInRange && !playerInAttackRange)
        {
            state = new ChaseState(new EnemyAIStateDesign());
        }

        //Enters patrol state if player is out of both ranges
        else if (!playerInRange && !playerInAttackRange)
        {
            state = new PatrolState(new EnemyAIStateDesign());
        }

        //Calls functions to set speed, where it goes, and if it attacks the player
        state.SetMovementSpeed();
        state.CurrentDestination();
        state.AttackPlayer();
    }

    //DEBUGGING SPHERE TO SHOW DISTANCE OF RANGES
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(transform.position, walkPointRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }


    /***
     *
     * Setter and Getter Methods
     *
     ***/
    public void SetSpeed(float s)
    {
        speed = s;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float GetPatrolSpeed()
    {
        return patrolSpeed;
    }

    public float GetChaseSpeed()
    {
        return chaseSpeed;
    }

    public void SetWalkPoint(Vector3 wp)
    {
        walkPoint = wp;
    }

    public Vector3 GetWalkPoint()
    {
        return walkPoint;
    }

    public float GetWalkPointRange()
    {
        return walkPointRange;
    }

    public float GetWalkTime()
    {
        return timeWalk;
    }

    public LayerMask GetGroundLayer()
    {
        return whatIsGround;
    }

    public Vector3 GetEnemyPos()
    {
        return this.transform.position;
    }

    public GameObject Bullet()
    {
        return projectile;
    }

    public float GetTimeBetweenAttacks()
    {
        return timeBetweenAttacks;
    }
}

//Abstract class for state class
abstract class State : MonoBehaviour
{
    protected EnemyAIStateDesign enemyAI;

    public State(EnemyAIStateDesign enemy)
    {
        this.enemyAI = enemy;
    }

    //Movement speed of enemy changes depending on current state
    abstract public void SetMovementSpeed();

    //Sets the destination of where the enemy agent should go
    abstract public void CurrentDestination();

    //Attacks the player
    abstract public void AttackPlayer();
}

/*** 
 * 
 * Patrol State
 *  If player is out of range, will get a random point and patrol to that point
 * 
***/
class PatrolState : State
{
    private float timerWalking;
    private bool walkPointSet;

    public PatrolState(EnemyAIStateDesign enemy) : base(enemy)
    {
        this.enemyAI = enemy;
    }

    public override void SetMovementSpeed()
    {
        enemyAI.SetSpeed(enemyAI.GetPatrolSpeed());
    }

    //Obtains random patrol points to travel to
    public override void CurrentDestination()
    {
        RandomPatrol();
    }

    public override void AttackPlayer()
    {
        //Nothing
    }

    //Selects random point and patrols to said point
    //Timer has been added if the player gets to the point early to reset the new point
    private void RandomPatrol()
    {
        Vector3 walkPoint;

        if (!walkPointSet)
        {
            RandomWalkPoint();
        }

        walkPoint = enemyAI.GetWalkPoint();

        if (walkPointSet)
        {
            enemyAI.enemy.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (timerWalking > 0f)
        {
            timerWalking -= Time.deltaTime;
        }

        if (distanceToWalkPoint.magnitude < 1f || timerWalking <= 0f)
        {
            walkPointSet = false;
            timerWalking = enemyAI.GetWalkTime();
        }
    }

    //Sets the random point
    private void RandomWalkPoint()
    {
        float wpRange = enemyAI.GetWalkPointRange();

        float randomZ = Random.Range(-wpRange, wpRange);
        float randomX = Random.Range(-wpRange, wpRange);

        float enemyX = enemyAI.GetEnemyPos().x;
        float enemyY = enemyAI.GetEnemyPos().y;
        float enemyZ = enemyAI.GetEnemyPos().z;

        Vector3 newTransform = new Vector3(enemyX + randomX, enemyY, enemyZ + randomZ);

        enemyAI.SetWalkPoint(newTransform);
    
        if (Physics.Raycast(enemyAI.GetWalkPoint(), -transform.up, 2f, enemyAI.GetGroundLayer()))
        {
            walkPointSet = true;
        }
    }
}

/*** 
 * 
 * Chase State
 *  If player is in range, it will chase the player
 * 
***/
class ChaseState : State
{
    public ChaseState(EnemyAIStateDesign enemy) : base(enemy)
    {
        this.enemyAI = enemy;
    }

    public override void SetMovementSpeed()
    {
        enemyAI.SetSpeed(enemyAI.GetChaseSpeed());
    }

    //Sets destination to the player's position
    //Allows it to "chase" player
    public override void CurrentDestination()
    {
        enemyAI.enemy.SetDestination(enemyAI.player.position);
    }

    public override void AttackPlayer()
    {
        //Nothing
    }
}

/*** 
 * 
 * Attack State
 *  If player is in range, it will shoot a ball at the player
 * 
***/
class AttackState : State
{
    private bool alreadyAttacked;

    public AttackState(EnemyAIStateDesign enemy) : base(enemy)
    {
        this.enemyAI = enemy;
    }

    public override void SetMovementSpeed()
    {
        enemyAI.SetSpeed(0);
    }

    //Makes enemy stand still while it fires a shot
    public override void CurrentDestination()
    {
        enemyAI.enemy.SetDestination(enemyAI.GetEnemyPos());
    }

    //Enemy will shoot a sphere towards player
    //Physics instantiate with forward force
    //Due to rigid body, might push back the enemy agent
    public override void AttackPlayer()
    {
        enemyAI.enemy.SetDestination(transform.position);

        transform.LookAt(enemyAI.player);

        if (!alreadyAttacked)
        {
            Rigidbody rb = Instantiate(enemyAI.Bullet(), transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), enemyAI.GetTimeBetweenAttacks());
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}
