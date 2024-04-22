using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    private void Awake()
    {
        navMeshSurface.BuildNavMesh();
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private RobotMovement robotMovement;
    public NavMeshSurface navMeshSurface;
    public float scanTimer = 0.0f;
    public bool playerInRange, playerSpotted, startTimer;
    public string currentPose;

    // Start is called before the first frame update
    void Start()
    {
        navMeshSurface.BuildNavMesh();
    }
    // Update is called once per frame
    void Update()
    {
        if (startTimer)
        {
            scanTimer += Time.deltaTime;
            if (scanTimer >= 1f)
            {
                SendCommand(currentPose);
                ResetScan();
            }
        }
    }
    void FixedUpdate()
    {
        navMeshSurface.BuildNavMesh();   
    }

    public void PlayerPoseDetection(string playerPose)
    {
        if (!playerSpotted)
        {
            return;
        }

        startTimer = true;
        currentPose = playerPose;
    }
    public void ResetScan()
    {
        startTimer = false;
        scanTimer = 0.0f;
    }
    public void SetPlayerSpotted(bool check)
    {
        playerSpotted = check;
    }
    public void SetPlayerInRange(bool check)
    {
        playerInRange = check;
    }
    public void SendCommand(string command)
    {
        meshRenderer.material.color = command switch
        {
            "GO" => Color.green,
            "STOP" => Color.red,
            "FOLLOW" => Color.yellow,
            _ => Color.white,
        };
        robotMovement.SetState(command);
    }
}
