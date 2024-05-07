using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobotMovement : MonoBehaviour
{
    [SerializeField]
    private Transform rack, deliveryArea;
    [SerializeField]
    private List<Transform> rackPoints, deliverypoints; 
    [SerializeField]
    private Transform player;
    //private List<Transform> waypoints;
    //private int currWaypoint = 0;
    private bool rackOn = false;
    private enum States
    {
        DELIVER,
        PICKUP,
        PUTDOWN,
        STOP,
        FOLLOW,
        STUCK,
    }
    private States currentState = States.DELIVER;
    private NavMeshAgent meshAgent;
    [SerializeField]
    private Animator animator;

    void Start()
    {
        meshAgent = GetComponent<NavMeshAgent>();
        meshAgent.stoppingDistance = 0;
    }
    void Update()
    {
        //Debug.Log(meshAgent.pathStatus);
        Debug.Log(currentState);
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
                if (rackOn)
                {
                    Move(deliverypoints[0]);
                }
                else
                {
                    Move(rackPoints[0]);
                }

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
                        if (rackOn)
                        {
                            currentState = States.PUTDOWN;
                            Move(deliveryArea);
                        }
                        else
                        {
                            rack.GetComponent<NavMeshObstacle>().enabled = false;
                            currentState = States.PICKUP;
                            Move(rack);
                        }
                    }
                }
                break;
            case States.PICKUP:
                Move(rack);
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    animator.SetInteger("LiftPhase", 1);
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("IdleUp"))
                    {
                        rackOn = true;
                        rack.SetParent(transform);
                        currentState = States.DELIVER;
                    }
                }
                break;
            case States.PUTDOWN:
                Move(deliveryArea);
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    animator.SetInteger("LiftPhase", 0);
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("IdleDown"))
                    {
                        rackOn = false;
                        rack.SetParent(null);
                        currentState = States.DELIVER;
                    }
                }
                break;
            case States.STOP:
                Debug.Log("stopped");
                break;
            case States.FOLLOW:
                //Path blocked
                if (meshAgent.pathStatus == NavMeshPathStatus.PathPartial)
                {
                    currentState = States.STUCK;
                    meshAgent.speed = 0;
                }
                else if (Vector3.Distance(transform.position, player.position) >= 4f)
                {
                    Move(player);
                }
                break;
            case States.STUCK:
                //Check if path is free
                if (meshAgent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    currentState = States.DELIVER;
                    meshAgent.speed = 2;
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
    public void ReceiveCommand(string command)
    {
        switch (command)
        {
            case "GO":
                meshAgent.speed = 0.5f;
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


    //for pickup and carry
    //stopping distance = 0
    //turn nav mesh obstacle off
    //when switching from carry to pickup, immediately use Move(rack) to change agent destination
}
