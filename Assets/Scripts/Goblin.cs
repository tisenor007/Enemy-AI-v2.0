using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Goblin : Enemy
{
    public Transform[] patrolPoints;

    private float remainingDistance = 0.5f;

    private Ray ray;
    private RaycastHit rayHit;
    // Start is called before the first frame update
    void Start()
    {
        ray = new Ray(this.transform.position, this.transform.forward);
        enemy.autoBraking = false;
        enemy = GetComponent<NavMeshAgent>();
        SwitchState(State.patrolling, patrolPoints);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(distanceBetweenTarget);
        awareness.radius = awarenessRange;
        if (target == null){ return; }
        else
        {
            distanceBetweenTarget = Vector3.Distance(target.transform.position, enemy.transform.position);
        }
        if (Physics.Raycast(transform.position, transform.forward, out rayHit, tunnelVision))
        {
            if (rayHit.transform.gameObject.tag == "Player")
            {
                target = rayHit.transform.gameObject;
                SwitchState(State.chasing, patrolPoints);
            }
            return;
        }
        if (distanceBetweenTarget <= awarenessRange)
        {
            SwitchState(State.chasing, patrolPoints);
        }
        else
        {
            if (!enemy.pathPending && enemy.remainingDistance < remainingDistance)
            {
                SwitchState(State.patrolling, patrolPoints);
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall") { return; }
        else
        {
            awareness.radius = 10;
            target = other.gameObject;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        awareness.radius = setAwareness;
    }

}
