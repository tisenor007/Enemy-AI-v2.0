using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
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

    protected GameObject target;
    protected Vector3 targetLastKnownPos;

    private int patrolDestination;
    private int patrolPointAmount = 16;

    // Start is called before the first frame update
    void Start()
    {
        patrolDestination = 0; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected void SwitchState(State newState, NavMeshAgent enemy, Transform[] patrolPoints)
    {
        state = newState;

        switch (state)
        {
            case State.patrolling:
                Patrol(enemy, patrolPoints);
                break;
            case State.retreating:
                Retreat(enemy, patrolPoints);
                break;
            case State.chasing:
                Chase(enemy, patrolPoints);
                break;
            case State.searchinng:
                Search(enemy, patrolPoints);
                break;
        }
    }

    public void Patrol(NavMeshAgent enemy, Transform[] patrolPoints)
    {
        enemy.autoBraking = false;
        if (patrolPoints.Length == 0) { enabled = false; return; }
        enemy.destination = patrolPoints[patrolDestination].position;
        patrolDestination = (patrolDestination + 1) % patrolPoints.Length;
    }

    public void Retreat(NavMeshAgent enemy, Transform[] patrolPoints)
    {
        patrolDestination = 0;
        enemy.autoBraking = true;
        if (this.transform.position.x == patrolPoints[0].position.x && this.transform.position.z == patrolPoints[0].position.z)
        {
            SwitchState(State.patrolling, enemy, patrolPoints);
        }
        enemy.SetDestination(patrolPoints[0].position);
    }
    public void Chase(NavMeshAgent enemy, Transform[] patrolPoints)
    {
        targetLastKnownPos = target.transform.position;
        enemy.SetDestination(targetLastKnownPos);
    }
    public void Search(NavMeshAgent enemy, Transform[] patrolPoints)
    {
        enemy.autoBraking = true;
        enemy.SetDestination(targetLastKnownPos);
        float searchDistance = Vector3.Distance(targetLastKnownPos, enemy.transform.position);
        if (searchDistance <= 1) { SwitchState(State.retreating, enemy, patrolPoints); }
    }

    
}
