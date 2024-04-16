using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobotMovement : MonoBehaviour
{
    [SerializeField]
    private List<Transform> waypoints;
    private int currWaypoint = 0;
    private Transform target;
    private enum States
    {
        DELIVER,
        STOP,
        STUCK,
        FOLLOW
    }
    private States currentState = States.DELIVER;
    private NavMeshAgent meshAgent;

    void Start()
    {
        meshAgent = GetComponent<NavMeshAgent>();

        Move(waypoints[currWaypoint]);
    }
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        FSM();
    }

    private void FSM()
    {
        switch (currentState)
        {
            case States.DELIVER:
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    currWaypoint++;
                    if (currWaypoint >= waypoints.Count)
                    {
                        currWaypoint = 0;
                    }
                }
                Move(waypoints[currWaypoint]);
                break;
            case States.STOP:
                break;
            case States.STUCK:
                break;
            default:
                break;
        }
    }
    private void Move(Transform destination)
    {
        meshAgent.SetDestination(destination.position);
    }
    public void SetState(string state)
    {
        switch (state)
        {
            
            default:
                break;
        }
    }
}
