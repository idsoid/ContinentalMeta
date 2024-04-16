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
    public bool playerSpotted, startTimer, poseStopped;
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
            if (scanTimer >= 1.75f)
            {
                SendCommand();
                ResetScan();
            }
        }
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
    private void SendCommand()
    {
        switch (currentPose)
        {
            case "GO":
                meshRenderer.material.color = Color.green;
                robotMovement.SetState("");
                break;
            case "STOP":
                meshRenderer.material.color = Color.red;
                robotMovement.SetState("");
                break;
            case "FOLLOW ME":
                meshRenderer.material.color = Color.yellow;
                robotMovement.SetState("");
                break;
            default:
                break;
        }
    }
}
