using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobotMovement : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField]
    private Transform rackArea, deliveryArea;
    [SerializeField]
    private List<Transform> rackPoints, deliverypoints;
    [SerializeField]
    private GameObject rackAreaObj, deliveryAreaObj, rackPrefab;
    private GameObject rackObj;
    private int rackNearestPoint = 0;
    private Transform goToPoint;
    [SerializeField]
    private Transform player;
    private bool rackOn = false;

    private Transform customPackage, customDeliveryArea;
    private enum States
    {
        DELIVER,
        PICKUP,
        PUTDOWN,
        BACKUP,
        STOP,
        FOLLOW,
        MANUALGOTO,
        MANUALPICKUP,
        MANUALPUTDOWN,
        MANUALBACKUP,
        STUCK,
    }
    private States currentState = States.DELIVER;
    private NavMeshAgent meshAgent;
    [SerializeField]
    private Animator animator;

    void Start()
    {
        gameManager = GameManager.Instance;
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
                    Move(deliverypoints[1]);
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
                            rackArea.GetComponent<NavMeshObstacle>().enabled = false;
                            currentState = States.PICKUP;
                            Move(rackArea);
                        }
                    }
                }
                break;
            case States.PICKUP:
                Move(rackArea);
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    animator.SetInteger("LiftPhase", 1);
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("IdleUp"))
                    {
                        rackOn = true;
                        rackObj = Instantiate(rackPrefab, rackArea);
                        rackObj.transform.SetParent(transform);
                        StartCoroutine(EnableRack());
                        currentState = States.DELIVER;
                        Move(deliverypoints[1]);
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
                        deliveryAreaObj.transform.rotation = rackObj.transform.rotation;
                        Destroy(rackObj);
                        StartCoroutine(DisableDelivery());
                        deliveryAreaObj.SetActive(true);
                        currentState = States.BACKUP;
                        meshAgent.updateRotation = false;
                        Move(deliverypoints[1]);
                    }
                }
                break;
            case States.BACKUP:
                Move(deliverypoints[1]);
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    currentState = States.DELIVER;
                    meshAgent.updateRotation = true;
                    Move(rackPoints[0]);
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
                else
                {
                    Move(player);
                }
                break;
            case States.MANUALGOTO:
                Move(goToPoint);
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    if (rackOn)
                    {
                        currentState = States.MANUALPUTDOWN;
                    }
                    else
                    {
                        customPackage.GetComponent<NavMeshObstacle>().enabled = false;
                        currentState = States.MANUALPICKUP;
                        Move(customPackage);
                    }
                }
                break; 
            case States.MANUALPICKUP:
                Move(customPackage);
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    animator.SetInteger("LiftPhase", 1);
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("IdleUp"))
                    {
                        rackOn = true;
                        customPackage.SetParent(transform);
                        currentState = States.FOLLOW;
                        Move(player);
                    }
                }
                break;
            case States.MANUALPUTDOWN:
                Move(customDeliveryArea);
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    animator.SetInteger("LiftPhase", 0);
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("IdleDown"))
                    {
                        rackOn = false;
                        customPackage.SetParent(null);
                        Destroy(customPackage.gameObject, 15f);
                        customPackage = null;
                        currentState = States.MANUALBACKUP;
                        meshAgent.updateRotation = false;
                        Move(goToPoint);
                    }
                }
                break;
            case States.MANUALBACKUP:
                Move(goToPoint);
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    currentState = States.FOLLOW;
                    meshAgent.updateRotation = true;
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
                gameManager.PlayerNavObstacle().enabled = true;
                meshAgent.speed = 0.5f;
                meshAgent.stoppingDistance = 0;
                currentState = States.DELIVER;
                break;
            case "STOP":
                meshAgent.speed = 0;
                meshAgent.stoppingDistance = 0;
                currentState = States.STOP;
                break;
            case "FOLLOW":
                gameManager.PlayerNavObstacle().enabled = false;
                meshAgent.speed = 0.5f;
                meshAgent.stoppingDistance = 2.0f;
                currentState = States.FOLLOW;
                break;
            case "MANUALPICKUP":
                gameManager.PlayerNavObstacle().enabled = true;
                meshAgent.speed = 0.5f;
                meshAgent.stoppingDistance = 0;
                NearestPoint(customPackage);
                currentState = States.MANUALGOTO;
                break;
            case "MANUALPUTDOWN":
                gameManager.PlayerNavObstacle().enabled = true;
                meshAgent.speed = 0.5f;
                meshAgent.stoppingDistance = 0;
                NearestPoint(customDeliveryArea);
                currentState = States.MANUALGOTO;
                break;
            case "STATUS":
                
                break;
            default:
                break;
        }
    }
    public void ReceivePackage(Transform package)
    {
        customPackage = package;
        Debug.Log(package.name);
    }
    public void ReceiveDeliveryArea(Transform deliveryArea)
    {
        customDeliveryArea = deliveryArea;
        Debug.Log(deliveryArea.name);
    }
    private IEnumerator DisableDelivery()
    {
        deliveryAreaObj.SetActive(true);
        yield return new WaitForSecondsRealtime(15f);
        deliveryAreaObj.SetActive(false);
    }
    private IEnumerator EnableRack()
    {
        rackAreaObj.SetActive(false);
        yield return new WaitForSecondsRealtime(15f);
        rackAreaObj.SetActive(true);
    }
    private void NearestPoint(Transform mainItem)
    {
        for (int i = 0; i < mainItem.transform.childCount; i++)
        {
            if (i == 0)
            {
                rackNearestPoint = 0;
            }
            else
            {
                if (Vector3.Distance(transform.position, mainItem.transform.GetChild(i).position) < Vector3.Distance(transform.position, mainItem.transform.GetChild(rackNearestPoint).position))
                {
                    rackNearestPoint = i;
                }
            }
        }
        goToPoint = mainItem.transform.GetChild(rackNearestPoint);
    }
}
