using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    enum State
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
    static State state;

    private int patrolDestination;
    private int patrolPointAmount = 16;
    // Start is called before the first frame update
    void Start()
    {
        //goblin.autoBraking = false;
        //goblin = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SwitchState(State newState, NavMeshAgent enemy, Transform[] patrolPoints)
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

        }

    }

    
}
