using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobotMovement : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    private Vector3 playerPos = Vector3.zero;
    [SerializeField]
    private List<Transform> waypoints;
    private int currWaypoint = 0;
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
        playerPos = player.position;
        Move(waypoints[currWaypoint]);
    }
    void Update()
    {
        //Debug.Log(meshAgent.pathStatus);
        Debug.Log(currentState);
        Debug.Log(playerPos);
        //Debug.Log(meshAgent.steeringTarget);
        if (Vector3.Distance(playerPos, player.position) >= 4f)
        {
            playerPos = player.position;
        }
    }
    void FixedUpdate()
    {
        //Update player saved position when changed
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
                    Move(playerPos);
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
    private void Move(Vector3 position)
    {
        meshAgent.SetDestination(position);
    }
    public void ReceiveCommand(string command)
    {
        switch (command)
        {
            case "GO":
                meshAgent.speed = 0.5f;
                meshAgent.stoppingDistance = 0.5f;
                currentState = States.DELIVER;
                break;
            case "STOP":
                meshAgent.speed = 0;
                currentState = States.STOP;
                break;
            case "FOLLOW":
                meshAgent.speed = 0.5f;
                meshAgent.stoppingDistance = 2.0f;
                currentState = States.FOLLOW;
                break;
            case "STATUS":
                
                break;
            default:
                break;
        }
    }
}
