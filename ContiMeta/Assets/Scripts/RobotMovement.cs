using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobotMovement : MonoBehaviour
{
    [SerializeField]
    private List<Transform> waypoints;
    private int currWaypoint = 0;
    [SerializeField]
    private Transform player;
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
        Debug.Log(meshAgent.pathStatus);
        Debug.Log(currentState);
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
                //Check if near/at destination
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    //Path blocked
                    if (meshAgent.pathStatus == NavMeshPathStatus.PathPartial)
                    {
                        currentState = States.STUCK;
                        meshAgent.speed = 0;
                    }
                    //Continue path
                    else
                    {
                        currWaypoint++;
                        if (currWaypoint == waypoints.Count)
                        {
                            currWaypoint = 0;
                        }
                    }
                }
                Move(waypoints[currWaypoint]);
                break;
            case States.STOP:
                Debug.Log("stopped");
                break;
            case States.STUCK:
                //Check if path is free
                if (meshAgent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    currentState = States.DELIVER;
                    meshAgent.speed = 2;
                }
                break;
            case States.FOLLOW:
                //Path blocked
                if (meshAgent.pathStatus == NavMeshPathStatus.PathPartial)
                {
                    currentState = States.STUCK;
                    meshAgent.speed = 0;
                }
                else
                {
                    Move(player);
                }
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
            case "GO":
                meshAgent.speed = 1;
                currentState = States.DELIVER;
                break;
            case "STOP":
                meshAgent.speed = 0;
                currentState = States.STOP;
                break;
            case "FOLLOW ME":
                meshAgent.speed = 1;
                currentState = States.FOLLOW;
                break;
            default:
                break;
        }
    }
}
