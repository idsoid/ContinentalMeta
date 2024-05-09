using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using Oculus.Interaction.Locomotion;

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
        for (int i = 0; i < robotList.Count; i++)
        {
            if (robotList[i].StartTimer)
            {
                robotList[i].ScanTimer += Time.deltaTime;
                if (robotList[i].ScanTimer >= 1f)
                {
                    PoseCommand(i, currentPose);
                    ResetScan(i);
                }
            }
        }
    }

    //Player Stuff
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
    public void ResetScan(int robotID)
    {
        robotList[robotID].Restart();
    }
    public void ToggleMenu()
    {
        menuActive = !menuActive;
    }
    public bool GetMenuActive()
    {
        return menuActive;
    }
    public void PoseCommand(int robotID, string command)
    {
        meshRenderers[robotID].material.color = command switch
        {
            "GO" => Color.green,
            "STOP" => Color.red,
            "FOLLOW" => Color.yellow,
            "STATUS" => Color.blue,
            _ => Color.black,
        };
        robotList[robotID].SendCommand(command);
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
    public void DebugTestMessage(string text)
    {
        Debug.Log(this.name + ": " + text);
    }

    //Object Stuff
    public void SpawnRack()
    {

    }
}
