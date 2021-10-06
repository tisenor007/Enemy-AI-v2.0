using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int tunnelVision;
    public int awarenessRange;
    public int setAwareness;
    public NavMeshAgent enemy;
    public SphereCollider awareness;
    protected enum State
    {
        patrolling,
        chasing,
        searchinng,
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

    
    private int patrolDestination;
    private int patrolPointAmount = 16;

    // Start is called before the first frame update
    void Start()
    {
        setAwareness = awarenessRange;
        patrolDestination = 0; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected void SwitchState(State newState, Transform[] patrolPoints)
    {
        state = newState;

        switch (state)
        {
            case State.patrolling:
                Patrol(patrolPoints);
                break;
            case State.retreating:
                Retreat(patrolPoints);
                break;
            case State.chasing:
                Chase(patrolPoints);
                break;
            case State.searchinng:
                Search(patrolPoints);
                break;
        }
    }

    public void Patrol(Transform[] patrolPoints)
    {
        enemy.autoBraking = false;
        if (patrolPoints.Length == 0) { enabled = false; return; }
        enemy.destination = patrolPoints[patrolDestination].position;
        patrolDestination = (patrolDestination + 1) % patrolPoints.Length;
    }

    public void Retreat( Transform[] patrolPoints)
    {
        patrolDestination = 0;
        enemy.autoBraking = true;
        if (this.transform.position.x == patrolPoints[0].position.x && this.transform.position.z == patrolPoints[0].position.z)
        {
            SwitchState(State.patrolling, patrolPoints);
        }
        enemy.SetDestination(patrolPoints[0].position);
    }
    public void Chase(Transform[] patrolPoints)
    {
        targetLastKnownPos = target.transform.position;
        enemy.SetDestination(targetLastKnownPos);
    }
    public void Search(Transform[] patrolPoints)
    {
        enemy.autoBraking = true;
        enemy.SetDestination(targetLastKnownPos);
        float searchDistance = Vector3.Distance(targetLastKnownPos, enemy.transform.position);
        if (searchDistance <= 1) { SwitchState(State.retreating, patrolPoints); }
    }
   
    //somehow make hearing float related to trigger radius......

}
