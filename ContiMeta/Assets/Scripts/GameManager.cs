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
    private List<MeshRenderer> meshRenderers;
    [SerializeField]
    private List<RobotManager> robotList;
    public NavMeshSurface navMeshSurface;
    public List<float> scanTimer;
    public List<bool> startTimer;
    public string currentPose;
    public bool menuActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        foreach (var robot in robotList)
        {
            if (robot.StartTimer)
            {
                robot.ScanTimer += Time.deltaTime;
                if (robot.ScanTimer >= 1f)
                {
                    PoseCommand(currentPose);
                    ResetScan();
                }
            }
        }
    }

    public void PlayerPoseDetection(string playerPose)
    {
        foreach (var robot in robotList)
        {
            if (robot.PlayerSpotted())
            {
                robot.StartTimer = true;
            }
        }
        currentPose = playerPose;
    }
    public void ResetScan()
    {
        foreach (var robot in robotList)
        {
            robot.Restart();
        }
    }
    public void ToggleMenu()
    {
        menuActive = !menuActive;
    }
    public bool GetMenuActive()
    {
        return menuActive;
    }
    public void PoseCommand(string command)
    {
        for (int i = 0; i < robotList.Count; i++)
        {
            if (robotList[i].PlayerSpotted())
            {
                meshRenderers[i].material.color = command switch
                {
                    "GO" => Color.green,
                    "STOP" => Color.red,
                    "FOLLOW" => Color.yellow,
                    "STATUS" => Color.blue,
                    _ => Color.black,
                };
                robotList[i].SendCommand(command);
            }
        }
    }
    public void VoiceCommand(string[] command)
    {
        int robotID = command[0].ToUpper() switch
        {
            string id when id.ToUpper().Contains("ONE") => 1,
            string id when id.ToUpper().Contains("TWO") => 2,
            string id when id.ToUpper().Contains("THREE") => 3,
            string id when id.ToUpper().Contains("FOUR") => 4,
            _ => 0
        };
        //if (!robotList[robotID].PlayerInRange())
        //{
        //    return;
        //}

        meshRenderers[robotID].material.color = command[1].ToUpper() switch
        {
            "GO" => Color.green,
            "STOP" => Color.red,
            "FOLLOW" => Color.yellow,
            "STATUS" => Color.blue,
            _ => Color.black,
        };
        robotList[robotID].SendCommand(command[1]);
    }
}
