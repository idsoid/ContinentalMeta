using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
public class RobotController : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField]
    private Transform rackArea, deliveryArea;
    [SerializeField]
    private Transform rackPoint, deliverypoint;
    [SerializeField]
    private GameObject rackAreaObj, deliveryAreaObj, rackPrefab;
    private GameObject rackObj;
    private int rackNearestPoint = 0;
    private Transform goToPoint;
    [SerializeField]
    private Transform player;
    private bool rackOn = false;
    private Transform customPackage, customDeliveryArea;
    public enum States
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
    public States currentState;
    private States previousState;
    private NavMeshAgent meshAgent;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private GameObject statusCanvas;
    [SerializeField]
    private TMP_Text statusText;

    void Start()
    {
        gameManager = GameManager.Instance;
        meshAgent = GetComponent<NavMeshAgent>();
        if (rackOn)
        {
            Move(deliverypoint);
        }
        else
        {
            Move(rackPoint);
        }
        statusCanvas.SetActive(false);
    }
    void Update()
    {
        //Debug.Log(meshAgent.pathStatus);
        //Debug.Log(currentState);
        //Debug.Log("distance: " + Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(player.position.x, player.position.z)));
    }
    void FixedUpdate()
    {
        //Update player saved position when changed
        StartCoroutine(FSM());
    }

    private IEnumerator FSM()
    {
        switch (currentState)
        {
            case States.DELIVER:
                if (rackOn)
                {
                    Move(deliverypoint);
                }
                else
                {
                    Move(rackPoint);
                }
                yield return new WaitUntil(() => !meshAgent.pathPending);

                //Check if near/at destination
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    //Path blocked
                    if (meshAgent.pathStatus == NavMeshPathStatus.PathPartial)
                    {
                        previousState = currentState;
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
                yield return new WaitUntil(() => !meshAgent.pathPending);
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    animator.SetInteger("LiftPhase", 1);
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("IdleUp"))
                    {
                        rackOn = true;
                        rackObj = Instantiate(rackPrefab, rackArea);
                        rackObj.transform.SetParent(transform);
                        customPackage = rackObj.transform;
                        StartCoroutine(EnableRack());
                        currentState = States.DELIVER;
                        Move(deliverypoint);
                    }
                }
                break;
            case States.PUTDOWN:
                Move(deliveryArea);
                yield return new WaitUntil(() => !meshAgent.pathPending);
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
                        Move(deliverypoint);
                    }
                }
                break;
            case States.BACKUP:
                Move(deliverypoint);
                yield return new WaitUntil(() => !meshAgent.pathPending);
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    currentState = States.DELIVER;
                    meshAgent.updateRotation = true;
                    Move(rackPoint);
                }
                break;
            case States.STOP:
                meshAgent.speed = 0;
                break;
            case States.FOLLOW:
                Debug.Log("follow path status: " + meshAgent.pathStatus);
                //Path blocked
                if (meshAgent.pathStatus == NavMeshPathStatus.PathPartial || meshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    previousState = currentState;
                    currentState = States.STUCK;
                    meshAgent.speed = 0;
                }
                else
                {
                    Move(player);
                    yield return new WaitUntil(() => !meshAgent.pathPending);
                    if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                    {
                        Vector3 lookPos = player.position - transform.position;
                        lookPos.y = 0;
                        Quaternion rotation = Quaternion.LookRotation(lookPos);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
                    }
                }
                break;
            case States.MANUALGOTO:
                Move(goToPoint);
                yield return new WaitUntil(() => !meshAgent.pathPending);
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    if (rackOn)
                    {
                        currentState = States.MANUALPUTDOWN;
                        Move(customDeliveryArea);
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
                yield return new WaitUntil(() => !meshAgent.pathPending);
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    animator.SetInteger("LiftPhase", 1);
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("IdleUp"))
                    {
                        rackOn = true;
                        customPackage.SetParent(transform);
                        gameManager.PlayerNavObstacle().enabled = true;
                        meshAgent.stoppingDistance = 2.0f;
                        currentState = States.FOLLOW;
                        Move(player);
                    }
                }
                break;
            case States.MANUALPUTDOWN:
                Move(customDeliveryArea);
                yield return new WaitUntil(() => !meshAgent.pathPending);
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    animator.SetInteger("LiftPhase", 0);
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("IdleDown"))
                    {
                        rackOn = false;
                        customPackage.SetParent(null);
                        currentState = States.MANUALBACKUP;
                        meshAgent.updateRotation = false;
                        Move(goToPoint);
                    }
                }
                break;
            case States.MANUALBACKUP:
                Move(goToPoint);
                yield return new WaitUntil(() => !meshAgent.pathPending);
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    customPackage.GetComponent<NavMeshObstacle>().enabled = true;
                    Destroy(customPackage.gameObject, 15f);
                    customPackage = null;
                    currentState = States.FOLLOW;
                    meshAgent.updateRotation = true;
                    gameManager.PlayerNavObstacle().enabled = true;
                    meshAgent.stoppingDistance = 2.0f;
                    Move(player);
                }
                break;
            case States.STUCK:
                //Check if path is free
                if (meshAgent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    currentState = previousState;
                    meshAgent.speed = 0.5f;
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
        if (InvalidCommand(command))
        {
            return;
        }

        switch (command)
        {
            case "GO":
                DefaultAgentSettings();
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
                meshAgent.stoppingDistance = 1.5f;
                meshAgent.autoBraking = false;
                currentState = States.FOLLOW;
                break;
            case "MANUALPICKUP":
                DefaultAgentSettings();
                StartCoroutine(NearestPoint(customPackage));
                break;
            case "MANUALPUTDOWN":
                DefaultAgentSettings();
                StartCoroutine(NearestPoint(customDeliveryArea));
                break;
            case "STATUS":
                StartCoroutine(StatusCheck());
                break;
            default:
                break;
        }
    }
    private bool InvalidCommand(string command)
    {
        if (currentState == States.MANUALGOTO || currentState == States.MANUALPICKUP || currentState == States.MANUALPUTDOWN || currentState == States.MANUALBACKUP || 
            currentState == States.PICKUP || currentState == States.PUTDOWN || currentState == States.BACKUP)
        {
            return true;
        }
        else if (rackOn && command == "MANUALPICKUP")
        {
            return true;
        }
        else if (!rackOn && command == "MANUALPUTDOWN")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void DefaultAgentSettings()
    {
        gameManager.PlayerNavObstacle().enabled = true;
        meshAgent.speed = 0.5f;
        meshAgent.stoppingDistance = 0;
        meshAgent.autoBraking = true;
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
        yield return new WaitForSecondsRealtime(10f);
        deliveryAreaObj.SetActive(false);
    }
    private IEnumerator EnableRack()
    {
        rackAreaObj.SetActive(false);
        yield return new WaitForSecondsRealtime(10f);
        rackAreaObj.SetActive(true);
    }
    private IEnumerator NearestPoint(Transform mainItem)
    {
        int pointCount = 0;
        List<int> invalidPoints = new();
        float distance = 0f;
        for (int i = 0; i < mainItem.transform.childCount; i++)
        {
            if (!mainItem.transform.GetChild(i).CompareTag("Point"))
            {
                continue;
            }
            pointCount++;

            Vector3 pointPos = mainItem.transform.GetChild(i).position;
            Collider[] pointColliders = Physics.OverlapSphere(pointPos, 0.25f, 1 << 6);
            Debug.Log(mainItem.transform.GetChild(i).name + ": player too close");
            if (NavMesh.SamplePosition(pointPos, out NavMeshHit hit, 0.25f, NavMesh.AllAreas))
            {
                meshAgent.SetDestination(hit.position);
            }
            else
            {
                Debug.Log("invalid: " + mainItem.transform.GetChild(i).name);
                invalidPoints.Add(i);
                continue;
            }

            yield return new WaitUntil(() => !meshAgent.pathPending);
            if (meshAgent.path.corners.Length != 0)
            {
                for (int j = 0; j < meshAgent.path.corners.Length - 1; j++)
                {
                    distance += Vector3.Distance(meshAgent.path.corners[j], meshAgent.path.corners[j + 1]);
                }
            }
            else
            {
                distance = 100f;
            }
            Debug.Log(mainItem.transform.GetChild(i).name + " distance: " + distance);

            if (pointColliders.Length > 0 || distance >= 5.0f)
            {
                Debug.Log("invalid: " + mainItem.transform.GetChild(i).name);
                invalidPoints.Add(i);
            }
        }
        if (invalidPoints.Count >= pointCount)
        {
            Debug.Log("invalid points on item");
            gameManager.PlayerNavObstacle().enabled = false;
            meshAgent.speed = 0.5f;
            meshAgent.stoppingDistance = 1.5f;
            meshAgent.autoBraking = false;
            currentState = States.FOLLOW;
            yield break;
        }

        for (int i = 0; i < mainItem.transform.childCount; i++)
        {
            if (!mainItem.transform.GetChild(i).CompareTag("Point") || invalidPoints.Contains(i))
            {
                continue;
            }
            
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
        currentState = States.MANUALGOTO;
    }
    private IEnumerator StatusCheck()
    {
        statusCanvas.SetActive(true);
        statusText.text = "" + currentState;
        meshAgent.speed = 0;
        yield return new WaitForSecondsRealtime(5f);
        statusCanvas.SetActive(false);
        meshAgent.speed = 0.5f;
    }
}
