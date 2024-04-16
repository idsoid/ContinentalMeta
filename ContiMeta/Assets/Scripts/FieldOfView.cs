using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfView : MonoBehaviour
{
    private GameManager gameManager;
    public float radius, angle;
    public LayerMask targetMask, obstructionMask;
    private IEnumerator coroutine;
    
    void Start()
    {
        gameManager = GameManager.Instance;
        coroutine = FOVRoutine();
        StartCoroutine(coroutine);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Field of View
    private IEnumerator FOVRoutine()
    {
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }
    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        if (rangeChecks.Length > 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    gameManager.SetPlayerSpotted(true);
                    return;
                }
            }
        }

        gameManager.SetPlayerSpotted(false);
    }
}
