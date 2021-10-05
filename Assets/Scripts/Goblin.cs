using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Goblin : Enemy
{
    public NavMeshAgent goblin;
    public Transform[] patrolPoints;
    private float remainingDistance = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        goblin.autoBraking = false;
        goblin = GetComponent<NavMeshAgent>();
        SwitchState(State.patrolling, goblin, patrolPoints);
    }

    // Update is called once per frame
    void Update()
    {
        if (!goblin.pathPending && goblin.remainingDistance < remainingDistance)
        {
            SwitchState(State.patrolling, goblin, patrolPoints);
        }
    }
}
