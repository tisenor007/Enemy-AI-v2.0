using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : Character
{
    //variables
    public Transform[] patrolPoints;
    public NavMeshAgent enemy;

    public int tunnelVision;//rayRange
    public int awareness; //hearing / detecting range
    public int attackDelay;
    [Header("For Health bar")]
    public Image healthBar;
    public GameObject canvas;
    public Transform camera;
    public bool isGuard;

    protected Character targetScript;
    protected int originAwareness;
    protected float originSpeed;
    protected int maxAwareness;
    protected float maxSpeed;

  
    //9 states
    protected enum State
    {
        patrolling,
        chasing,
        searching,
        attacking,
        retreating,
        fleeing,
        resting,
        guarding,
        dead
    }
    protected State state;

    protected float distanceBetweenTarget;
    protected Vector3 targetLastKnownPos;
 
    protected int patrolDestination;
    private int patrolPointAmount = 16;

    private bool isGettingBumped;
    private Ray ray;
    private RaycastHit rayHit;
    private int nextAttackDelay;
    private Vector3 originPos;
    private Vector3 originRot;
    private float originSize;

    // Start is called before the first frame update
    void Start()
    {
        //enemy crouching has not been implemented yet
        crouching = false;
        //sets max variables 
        nextHealCoolDown = healingCoolDown;
        //retrieves rigidbody for you
        rb = this.gameObject.GetComponent<Rigidbody>();
        nextAttackDelay = attackDelay;
        //position/rotation for guards
        originPos = this.transform.position;
        originRot = this.transform.eulerAngles;
        maxHealth = health;
        //is not dead on spawn
        isDead = false;
        //original speed and awareness because these variables are doubled when chasing so they need to return to original value afterwards
        originSpeed = enemy.speed;
        originAwareness = awareness;
        //more max vairables
        maxAwareness = awareness * 2;
        maxSpeed = enemy.speed * 2;

        originSize = healthBar.rectTransform.sizeDelta.x;
        ray = new Ray(this.transform.position, this.transform.forward);

        enemy = GetComponent<NavMeshAgent>();
        //resets patrol position
        patrolDestination = 0;
        //if enemy is set to be a guard it will guard else it will roam / patrol
        if (isGuard == true) { state = State.guarding; }
        else { state = State.patrolling; }
    }

    // Update is called once per frame
    void Update()
    {
        distanceBetweenHomeBase = Vector3.Distance(homeBase.transform.position, this.transform.position);
        healthBar.rectTransform.sizeDelta = new Vector2(originSize * health / maxHealth, healthBar.rectTransform.sizeDelta.y);
        canvas.transform.LookAt(canvas.transform.position + camera.forward);
        Debug.Log(state);
        //Debug.Log(distanceBetweenHomeBase);
        if (isDead == true && state != State.dead)
        {
            state = State.dead;
        }
        //will not calculate distance between target if target is not defined......
        if (target != null && isDead == false)
        {
            distanceBetweenTarget = Vector3.Distance(enemy.transform.position, target.transform.position);
        }
        //if enemy is almost dead it will flee to base
           
        switch (state)
        {
            case State.patrolling:
                enemy.autoBraking = false;
                if (!enemy.pathPending && enemy.remainingDistance < actionDistance)
                {
                    PatrolNextPoint();
                }
                GeneralBehaviorCheck();
                if (amAlmostDead() == true) { state = State.fleeing; }
                if (targetDetected() == true) { state = State.chasing; }
                if (targetWithinRange() == true) { state = State.attacking; }
                break;
            case State.fleeing:
                enemy.autoBraking = false;
                Flee();
                if (distanceBetweenHomeBase <= actionDistance)
                {
                    state = State.resting;
                }
                GeneralBehaviorCheck();
                break;
            case State.resting:
                enemy.autoBraking = true;
                if (health >= maxHealth)
                {
                    if (isGuard == true) { state = State.guarding; }
                    else { state = State.retreating; }
                }
                if (Time.time > nextHealCoolDown)
                {
                    Heal(maxHealth / 4);
                    nextHealCoolDown = Mathf.RoundToInt(Time.time) + healingCoolDown;
                }
                else if (distanceBetweenHomeBase <= actionDistance && health < maxHealth)
                {
                    state = State.resting;
                }
                GeneralBehaviorCheck();
                break;
            case State.attacking:
                enemy.autoBraking = true;
                
                if (targetScript.isDead == true)
                {
                    if (isGuard == true) { state = State.guarding; }
                    else
                    {
                        //attackDelay = nextAttackDelay;
                        state = State.retreating;
                    }
                }
                else
                {
                    if (Time.time > nextAttackDelay)
                    {
                        attack();
                        nextAttackDelay = Mathf.RoundToInt(Time.time) + attackDelay;
                    }
                    if (target != null)
                    {
                        if (distanceBetweenTarget >= actionDistance)
                        {
                            state = State.chasing;
                        }
                    }
                }
                GeneralBehaviorCheck();
                if (amAlmostDead() == true) { state = State.fleeing; }
                break;
            case State.chasing:
                enemy.autoBraking = false;
                if (distanceBetweenTarget > awareness && rayHit.transform.gameObject != target)
                {
                    state = State.searching;
                }
                if (distanceBetweenTarget <= actionDistance)
                {
                    state = State.attacking;
                }
                if (Physics.Raycast(transform.position, transform.forward, out rayHit, tunnelVision))
                {
                    if (rayHit.transform.gameObject.tag != "NonTarget" && rayHit.transform.gameObject.tag != this.gameObject.tag)
                    {
                        //Debug.Log("AYOOO");
                        target = rayHit.transform.gameObject;
                        targetScript = target.GetComponent<Character>();
                        if (distanceBetweenTarget > actionDistance && targetScript.isDead == false)
                        {
                            Chase();
                        }
                    }
                }
                if (target != null)
                {
                    if (distanceBetweenTarget <= awareness && distanceBetweenTarget > actionDistance)
                    {
                        if (target.tag != "NonTarget" && target.tag != this.gameObject.tag && targetScript.crouching == false && targetScript.isDead == false)
                        {
                            Chase();
                        }
                    }
                    if (distanceBetweenTarget <= actionDistance)
                    {
                        if (target.tag != "NonTarget" && target.tag != this.gameObject.tag && targetScript.crouching == false && targetScript.isDead == false)
                        {
                            state = State.attacking;
                        }
                    }
                }
                if (health <= (maxHealth / 4))
                {
                    state = State.fleeing;
                }
                GeneralBehaviorCheck();
                if (amAlmostDead() == true) { state = State.fleeing; }
                if (targetDetected() == true) { state = State.chasing; }
                if (targetWithinRange() == true) { state = State.attacking; }
                break;
            case State.searching:
                enemy.autoBraking = false;
                Search();
                float searchDistance = Vector3.Distance(targetLastKnownPos, enemy.transform.position);
                if (searchDistance <= actionDistance) { state = State.retreating; }
                GeneralBehaviorCheck();
                if (amAlmostDead() == true) { state = State.fleeing; }
                if (targetDetected() == true) { state = State.chasing; }
                if (targetWithinRange() == true) { state = State.attacking; }
                break;
            case State.retreating:
                enemy.autoBraking = false;
                Retreat();
                if (isGuard == true) { state = State.guarding; }
                if (targetDetected() == true) { state = State.chasing; }
                if (targetWithinRange() == true) { state = State.attacking; }
                else
                {
                    float startDistance = Vector3.Distance(patrolPoints[0].position, enemy.transform.position);
                    if (startDistance <= actionDistance) { state = State.patrolling; }
                }
                GeneralBehaviorCheck();
                if (amAlmostDead() == true) { state = State.fleeing; }
                if (targetDetected() == true) { state = State.chasing; }
                if (targetWithinRange() == true) { state = State.attacking; }
                break;
            case State.guarding:
                enemy.autoBraking = true;
                Guard();
                float originPosDistance = Vector3.Distance(originPos, enemy.transform.position);
                if (originPosDistance <= actionDistance)
                {
                    enemy.transform.eulerAngles = originRot;
                }
                GeneralBehaviorCheck();
                if (amAlmostDead() == true) { state = State.fleeing; }
                if (targetDetected() == true) { state = State.chasing; }
                if (targetWithinRange() == true) { state = State.attacking; }
                break;
            case State.dead:
                enemy.autoBraking = true;
                Die();
                break;
        }
    }
    
    public bool amAlmostDead()
    {
        if (health <= (maxHealth / 4))
        {
            //state = State.fleeing;
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool targetWithinRange()
    {
        if (target != null)
        {
            if (distanceBetweenTarget <= actionDistance)
            {
                if (target.tag != "NonTarget" && target.tag != this.gameObject.tag && targetScript.crouching == false && targetScript.isDead == false)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool targetDetected()
    {
       
        if (Physics.Raycast(transform.position, transform.forward, out rayHit, tunnelVision))
        {
            if (rayHit.transform.gameObject.tag != "NonTarget" && rayHit.transform.gameObject.tag != this.gameObject.tag)
            {
                //Debug.Log("AYOOO");
                target = rayHit.transform.gameObject;
                targetScript = target.GetComponent<Character>();
                if (distanceBetweenTarget > actionDistance && targetScript.isDead == false && state != State.fleeing)
                {
                    return true;
                }
            }
        }

        if (target != null)
        {
            if (distanceBetweenTarget <= awareness && distanceBetweenTarget > actionDistance)
            {
                if (target.tag != "NonTarget" && target.tag != this.gameObject.tag && targetScript.crouching == false && targetScript.isDead == false && state != State.fleeing)
                {
                    return true;
                }
            }
        }
        return false;
    }
    //Just background behavior put into a method for effciency, That is if they are alive
    public void GeneralBehaviorCheck()
    {
        
      
        if (state != State.fleeing && state != State.resting && state != State.dead)
        {
            if (health <= (maxHealth / 4))
            {
                state = State.fleeing;
            }
            if (Physics.Raycast(transform.position, transform.forward, out rayHit, tunnelVision))
            {
                if (rayHit.transform.gameObject.tag != "NonTarget" && rayHit.transform.gameObject.tag != this.gameObject.tag)
                {
                    //Debug.Log("AYOOO");
                    target = rayHit.transform.gameObject;
                    targetScript = target.GetComponent<Character>();
                    if (distanceBetweenTarget > actionDistance && targetScript.isDead == false && state != State.fleeing)
                    {
                        state = State.chasing;
                    }
                }
            }

            if (target != null)
            {
                if (distanceBetweenTarget <= awareness && distanceBetweenTarget > actionDistance)
                {
                    if (target.tag != "NonTarget" && target.tag != this.gameObject.tag && targetScript.crouching == false && targetScript.isDead == false && state != State.fleeing)
                    {
                        state = State.chasing;
                    }
                }
                if (distanceBetweenTarget <= actionDistance)
                {
                    if (target.tag != "NonTarget" && target.tag != this.gameObject.tag && targetScript.crouching == false && targetScript.isDead == false)
                    {
                        state = State.attacking;
                    }
                }
            }
        }
    }
    //methods that go with states for simplicity
    public void Flee()
    {
        enemy.SetDestination(homeBase.transform.position);
    }
    public void Guard()
    {
        enemy.SetDestination(originPos);
    }
    public void Die()
    {
        this.gameObject.transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, transform.eulerAngles.z);
        this.gameObject.GetComponent<Collider>().enabled = false;
        this.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        this.canvas.SetActive(false);
    }
    public void PatrolNextPoint()
    {
        enemy.speed = originSpeed;
        awareness = originAwareness;
        if (patrolPoints.Length == 0) { enabled = false; return; }
        enemy.destination = patrolPoints[patrolDestination].position;
        patrolDestination = (patrolDestination + 1) % patrolPoints.Length;
    }

    public void Retreat()
    {
        enemy.speed = originSpeed;
        awareness = originAwareness;
        enemy.SetDestination(patrolPoints[0].position);
    }
    public void Chase()
    {
        enemy.speed = maxSpeed;
        awareness = maxAwareness;
        targetLastKnownPos = target.transform.position;
        enemy.SetDestination(targetLastKnownPos);
    }
    public void Search()
    {
        enemy.speed = originSpeed;
        awareness = originAwareness;
        enemy.SetDestination(targetLastKnownPos);
    }
    public void attack()
    {
        targetScript.TakeDamage(this.attackDamage);
    }
    //targeting system
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "NonTarget" && other.gameObject.tag != this.gameObject.tag && isDead == false)
        {
            target = other.gameObject;
            targetScript = target.GetComponent<Character>();
        }
    }
    //if a possible target hits enemy and is enemy is not fleeing, it will attack
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag != "NonTarget" && other.gameObject.tag != this.gameObject.tag && isDead == false)
        {
            target = other.gameObject;
            targetScript = target.GetComponent<Character>();
            isGettingBumped = true;
        }
    }
}
