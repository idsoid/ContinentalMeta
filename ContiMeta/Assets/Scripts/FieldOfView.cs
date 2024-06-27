using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfView : MonoBehaviour
{
    public float radius, angle;
    public LayerMask targetMask, obstructionMask, forkliftMask;
    private IEnumerator coroutine;
    public bool playerInRange, playerSpotted, forkliftSpotted;

    [SerializeField]
    private MeshFilter meshFilter;
    private Mesh visionConeMesh;
    public int visionConeResolution = 120;//the vision cone will be made up of triangles, the higher this value is the prettier the vision cone will be

    void Start()
    {
        coroutine = FOVRoutine();
        StartCoroutine(coroutine);
        visionConeMesh = new Mesh();
    }

    // Update is called once per frame
    void Update()
    {
        DrawVisionCone();//calling the vision cone function everyframe just so the cone is updated every frame
    }
    void DrawVisionCone()//this method creates the vision cone mesh
    {
        int[] triangles = new int[(visionConeResolution - 1) * 3];
        Vector3[] Vertices = new Vector3[visionConeResolution + 1];
        Vertices[0] = Vector3.zero;
        float Currentangle = -(angle * Mathf.Deg2Rad) / 2;
        float angleIcrement = (angle * Mathf.Deg2Rad) / (visionConeResolution - 1);
        float Sine;
        float Cosine;

        for (int i = 0; i < visionConeResolution; i++)
        {
            Sine = Mathf.Sin(Currentangle);
            Cosine = Mathf.Cos(Currentangle);
            Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);
            Vertices[i + 1] = VertForward * radius * 3.33f;


            Currentangle += angleIcrement;
        }
        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }
        visionConeMesh.Clear();
        visionConeMesh.vertices = Vertices;
        visionConeMesh.triangles = triangles;
        meshFilter.mesh = visionConeMesh;
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
            playerInRange = true;
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    playerSpotted = true;
                    return;
                }
            }
            return;
        }

        playerInRange = false;
        playerSpotted = false;
    }
    public bool GetPlayerSpotted()
    {
        return playerSpotted;
    }
    public bool GetPlayerInRange()
    {
        return playerInRange;
    }

    //https://www.youtube.com/watch?v=luLrhoTZYD8
}
