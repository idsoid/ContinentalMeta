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
    public States previousState;
    private Vector3 lastTarget;
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
        Debug.Log("pathstatus: " + meshAgent.pathStatus);
    }
    void FixedUpdate()
    {
        FSM();
    }

    private void FSM()
    {
        Vector3 targetPos = new();
        switch (currentState)
        {
            case States.DELIVER:
                if (rackOn)
                {
                    Move(deliverypoint);
                    targetPos = deliverypoint.position;
                }
                else
                {
                    Move(rackPoint);
                    targetPos = rackPoint.position;
                }
                //Check if near/at destination
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    //Path blocked
                    if (meshAgent.pathStatus == NavMeshPathStatus.PathPartial || meshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                    {
                        previousState = currentState;
                        currentState = States.STUCK;
                        meshAgent.speed = 0;
                        if (rackOn)
                        {
                            lastTarget = deliverypoint.position;
                        }
                        else
                        {
                            lastTarget = rackPoint.position;
                        }
                    }
                    //Continue path
                    else if (Vector3.Distance(targetPos, transform.position) <= 2.0f)
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
                        meshAgent.avoidancePriority = 1;
                    }
                }
                break;
            case States.PICKUP:
                Move(rackArea);
                targetPos = rackArea.position;
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance && Vector3.Distance(targetPos, transform.position) <= 2.0f)
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
                        meshAgent.avoidancePriority = 50;
                        Move(deliverypoint);
                    }
                }
                break;
            case States.PUTDOWN:
                Move(deliveryArea);
                targetPos = deliveryArea.position;
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance && Vector3.Distance(targetPos, transform.position) <= 2.0f)
                {
                    animator.SetInteger("LiftPhase", 0);
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("IdleDown"))
                    {
                        if (rackObj != null)
                        {
                            rackOn = false;
                            deliveryAreaObj.transform.rotation = rackObj.transform.rotation;
                            customPackage = null;
                        }
                        StartCoroutine(DisableDelivery());
                        deliveryAreaObj.SetActive(true);
                        Destroy(rackObj);
                        currentState = States.BACKUP;
                        meshAgent.updateRotation = false;
                        Move(deliverypoint);
                    }
                }
                break;
            case States.BACKUP:
                Move(deliverypoint);
                targetPos = deliverypoint.position;
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance && Vector3.Distance(targetPos, transform.position) <= 2.0f)
                {
                    currentState = States.DELIVER;
                    meshAgent.updateRotation = true;
                    meshAgent.avoidancePriority = 50;
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
                    targetPos = player.position;
                    if (Vector3.Distance(targetPos, transform.position) <= 2.0f)
                    {
                        meshAgent.speed = 0;
                        Vector3 lookPos = player.position - transform.position;
                        lookPos.y = 0;
                        Quaternion rotation = Quaternion.LookRotation(lookPos);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
                    }
                    else
                    {
                        meshAgent.speed = 0.75f;
                    }
                }
                break;
            case States.MANUALGOTO:
                Move(goToPoint);
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    gameManager.PlayerNavObstacle.enabled = false;
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
                targetPos = customPackage.position;
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance && Vector3.Distance(targetPos, transform.position) <= 0.5f)
                {
                    animator.SetInteger("LiftPhase", 1);
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("IdleUp"))
                    {
                        rackOn = true;
                        customPackage.SetParent(transform);
                        gameManager.PlayerNavObstacle.enabled = true;
                        meshAgent.speed = 0.75f;
                        meshAgent.stoppingDistance = 2.5f;
                        meshAgent.avoidancePriority = 50;
                        currentState = States.FOLLOW;
                        Move(player);
                    }
                }
                break;
            case States.MANUALPUTDOWN:
                Move(customDeliveryArea);
                targetPos = customDeliveryArea.position;
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance && Vector3.Distance(targetPos, transform.position) <= 0.5f)
                {
                    animator.SetInteger("LiftPhase", 0);
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("IdleDown"))
                    {
                        rackOn = false;
                        customPackage.SetParent(null);
                        customPackage.tag = "Delivered";
                        currentState = States.MANUALBACKUP;
                        meshAgent.updateRotation = false;
                        Move(goToPoint);
                    }
                }
                break;
            case States.MANUALBACKUP:
                Move(goToPoint);
                targetPos = goToPoint.position;
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance && Vector3.Distance(targetPos, transform.position) <= 2.0f)
                {
                    if (customPackage != null)
                    {
                        customPackage.GetComponent<NavMeshObstacle>().enabled = true;
                        Destroy(customPackage.gameObject, 15f);
                        customPackage = null;
                    }
                    currentState = States.FOLLOW;
                    meshAgent.updateRotation = true;
                    gameManager.PlayerNavObstacle.enabled = true;
                    meshAgent.speed = 0.75f;
                    meshAgent.stoppingDistance = 2.5f;
                    meshAgent.avoidancePriority = 50;
                    Move(player);
                }
                break;
            case States.STUCK:
                meshAgent.radius = 0.1f;
                meshAgent.SetDestination(lastTarget);
                Collider[] pointColliders = Physics.OverlapSphere(transform.position, 4f, 1 << 8);
                Debug.Log(this.name + " robot colliders: " + pointColliders.Length);
                //Check if path is free
                if (meshAgent.pathStatus == NavMeshPathStatus.PathComplete && pointColliders.Length <= 1)
                {
                    currentState = previousState;
                    meshAgent.speed = 0.5f;
                    meshAgent.radius = 3.0f;
                    meshAgent.SetDestination(lastTarget);
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
                gameManager.PlayerNavObstacle.enabled = false;
                meshAgent.speed = 0.75f;
                meshAgent.stoppingDistance = 2.5f;
                meshAgent.autoBraking = false;
                currentState = States.FOLLOW;
                break;
            case "MANUALPICKUP":
                DefaultAgentSettings();
                meshAgent.avoidancePriority = 1;
                StartCoroutine(NearestPoint(customPackage));
                break;
            case "MANUALPUTDOWN":
                DefaultAgentSettings();
                meshAgent.avoidancePriority = 1;
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
        else if (currentState == States.STUCK && command != "STATUS")
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
        gameManager.PlayerNavObstacle.enabled = true;
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
        rackArea.GetComponent<NavMeshObstacle>().enabled = true;
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
            Collider[] pointColliders = Physics.OverlapSphere(pointPos, 0.1f, 1 << 6);
            if (pointColliders.Length > 0)
            {
                Debug.Log(mainItem.transform.GetChild(i).name + ": player too close");
            }
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

            if (pointColliders.Length > 0 || distance >= 15.0f)
            {
                Debug.Log("invalid: " + mainItem.transform.GetChild(i).name);
                invalidPoints.Add(i);
            }
        }
        if (invalidPoints.Count >= pointCount)
        {
            Debug.Log("invalid points on item");
            gameManager.PlayerNavObstacle.enabled = false;
            meshAgent.speed = 0.75f;
            meshAgent.stoppingDistance = 2.5f;
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
        meshAgent.speed = 0.5f;
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
