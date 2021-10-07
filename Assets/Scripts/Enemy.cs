using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    //implement this.tag, such&such for possible breeding....
    public Transform[] patrolPoints;
    public NavMeshAgent enemy;
    public Player playerScript;
    public int tunnelVision;//rayRange
    public int awareness;
    protected float actionDistance = 0.5f;
    protected int originAwareness;
    protected float originSpeed;
    protected int maxAwareness;
    protected float maxSpeed;
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
    }
    protected State state;

    protected float distanceBetweenTarget;

    protected GameObject target;
    protected Vector3 targetLastKnownPos;

    
    protected int patrolDestination;
    private int patrolPointAmount = 16;

    private Ray ray;
    private RaycastHit rayHit;

    // Start is called before the first frame update
    void Start()
    {
        originSpeed = enemy.speed;
        originAwareness = awareness;
        maxAwareness = awareness * 2;
        maxSpeed = enemy.speed * 2;

        ray = new Ray(this.transform.position, this.transform.forward);

        enemy.autoBraking = false;
        enemy = GetComponent<NavMeshAgent>();

        patrolDestination = 0;
        SwitchState(State.patrolling);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(state);
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
        if (state != State.chasing)
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
        }
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
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Wall")
        {
            target = other.gameObject;
        }
    }
}
