using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : Character
{
    //implement this.tag, such&such for possible breeding....
    public Transform[] patrolPoints;
    public NavMeshAgent enemy;
    public Player playerScript;

    public int tunnelVision;//rayRange
    public int awareness;
    [Header("For Health bar")]
    public Image healthBar;
    public GameObject canvas;
    public Transform camera;

    protected Character targetScript;
    protected int originAwareness;
    protected float originSpeed;
    protected int maxAwareness;
    protected float maxSpeed;

    private float originSize;
    protected enum State
    {
        patrolling,
        chasing,
        searching,
        attacking,
        retreating,
        breeding,
        resting,
        guarding,
        dead
    }
    protected State state;

    protected float distanceBetweenTarget;

    protected Vector3 targetLastKnownPos;

    
    protected int patrolDestination;
    private int patrolPointAmount = 16;

    private Ray ray;
    private RaycastHit rayHit;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;
        isDead = false;
        originSpeed = enemy.speed;
        originAwareness = awareness;
        maxAwareness = awareness * 2;
        maxSpeed = enemy.speed * 2;

        originSize = healthBar.rectTransform.sizeDelta.x;
        ray = new Ray(this.transform.position, this.transform.forward);

        enemy.autoBraking = false;
        enemy = GetComponent<NavMeshAgent>();

        patrolDestination = 0;
        SwitchState(State.patrolling);
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.rectTransform.sizeDelta = new Vector2(originSize * health / maxHealth, healthBar.rectTransform.sizeDelta.y);
        canvas.transform.LookAt(canvas.transform.position + camera.forward);
        Debug.Log(state);
        if (isDead == true && state != State.dead)
        {
            SwitchState(State.dead);
        }
        else if (isDead == false)
        {
            if (target != null)
            {
                distanceBetweenTarget = Vector3.Distance(enemy.transform.position, target.transform.position);
            }
            if (state == State.patrolling)
            {
                if (!enemy.pathPending && enemy.remainingDistance < actionDistance)
                {
                    SwitchState(State.patrolling);
                }
            }
            if (state == State.attacking)
            {
                if (targetScript.isDead == true)
                {
                    SwitchState(State.patrolling);
                }
            }
            //Chasing-----------------------------------------------------------------------------------
            if (state != State.chasing && state != State.attacking)
            {
                if (Physics.Raycast(transform.position, transform.forward, out rayHit, tunnelVision))
                {
                    if (rayHit.transform.gameObject.tag == "Player")
                    {
                        target = rayHit.transform.gameObject;
                        SwitchState(State.chasing);
                    }
                }

                if (target != null)
                {
                    if (distanceBetweenTarget <= awareness)
                    {
                        if (target.tag == "Player" && playerScript.crouching == false)
                        {
                            SwitchState(State.chasing);
                        }
                    }
                }
            }
            else if (state == State.chasing)
            {
                if (distanceBetweenTarget > awareness)
                {
                    SwitchState(State.searching);
                }
                if (distanceBetweenTarget <= actionDistance)
                {
                    SwitchState(State.attacking);
                }
                if (Physics.Raycast(transform.position, transform.forward, out rayHit, tunnelVision))
                {
                    if (rayHit.transform.gameObject.tag == "Player")
                    {
                        target = rayHit.transform.gameObject;
                        SwitchState(State.chasing);
                    }
                }

                if (target != null)
                {
                    if (distanceBetweenTarget <= awareness)
                    {
                        if (target.tag == "Player" && playerScript.crouching == false)
                        {
                            SwitchState(State.chasing);
                        }
                    }
                }
            }
            //searching------------------------------------------------------------------------------------------------
            if (state == State.searching)
            {
                float searchDistance = Vector3.Distance(targetLastKnownPos, enemy.transform.position);
                if (searchDistance <= 1) { SwitchState(State.retreating); }
            }
            if (state == State.retreating)
            {
                float startDistance = Vector3.Distance(patrolPoints[0].position, enemy.transform.position);
                if (startDistance <= 2) { SwitchState(State.patrolling); }
            }
        }
    }
    protected void SwitchState(State newState)
    {
        state = newState;

        switch (state)
        {
            case State.patrolling:
                Patrol();
                break;
            case State.retreating:
                Retreat();
                break;
            case State.chasing:
                Chase();
                break;
            case State.searching:
                Search();
                break;
            case State.attacking:
                attack();
                break;
            case State.dead:
                die();
                break;
        }
    }
    public void die()
    {
        this.gameObject.transform.Rotate(90, 0, 0);
        this.gameObject.GetComponent<Collider>().enabled = false;
        this.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        this.canvas.SetActive(false);
    }
    public void Patrol()
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
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "NonTarget" && other.gameObject.tag != this.gameObject.tag)
        {
            target = other.gameObject;
            targetScript = target.GetComponent<Character>();
        }
    }
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag != "NonTarget" && other.gameObject.tag != this.gameObject.tag)
        {
            SwitchState(State.attacking);
        }
    }
    public void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag != "NonTarget" && other.gameObject.tag != this.gameObject.tag)
        {
            SwitchState(State.chasing);
        }
    }
}
