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
    private NavMeshObstacle playerNavObstacle;
    [SerializeField]
    private Transform playerRightHand, playerLeftHand, player;
    private int closestPackage;
    private int closestDeliveryArea;
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
    public void PoseResetScan()
    {
        foreach (var robot in robotList)
        {
            if (robot.PlayerSpotted())
            {
                robot.Restart();
            }
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
        switch (command)
        {
            case "RIGHTPICKUP":
                PackageCheck(playerRightHand, robotID);
                robotList[robotID].SendCommand("MANUALPICKUP");
                break;
            case "LEFTPICKUP":
                PackageCheck(playerLeftHand, robotID);
                robotList[robotID].SendCommand("MANUALPICKUP");
                break;
            case "RIGHTPUTDOWN":
                AreaCheck(playerRightHand, robotID);
                robotList[robotID].SendCommand("MANUALPUTDOWN");
                break;
            case "LEFTPUTDOWN":
                AreaCheck(playerLeftHand, robotID);
                robotList[robotID].SendCommand("MANUALPUTDOWN"); 
                break;
            default:
                robotList[robotID].SendCommand(command);
                break;
        }
    }
    public void VoiceCommand(string[] command)
    {
        int robotID = command[0].ToUpper() switch
        {
            string id when id.ToUpper().Contains("ONE") => 0,
            string id when id.ToUpper().Contains("TWO") => 1,
            string id when id.ToUpper().Contains("THREE") => 2,
            string id when id.ToUpper().Contains("FOUR") => 3,
            _ => 9
        };
        if (robotID == 9)
        {
            return;
        }
        else if (!robotList[robotID].PlayerInRange())
        {
            return;
        }

        meshRenderers[robotID].material.color = command[1].ToUpper() switch
        {
            "GO" => Color.green,
            "STOP" => Color.red,
            "FOLLOW" => Color.yellow,
            "STATUS" => Color.blue,
            _ => Color.black,
        };
        //switch (command[1])
        //{
        //    case "RIGHTPICKUP":
        //        PackageCheck(playerRightHand, robotID);
        //        robotList[robotID].SendCommand("MANUALPICKUP");
        //        break;
        //    case "LEFTPICKUP":
        //        PackageCheck(playerLeftHand, robotID);
        //        robotList[robotID].SendCommand("MANUALPICKUP");
        //        break;
        //    case "RIGHTPUTDOWN":
        //        AreaCheck(playerRightHand, robotID);
        //        robotList[robotID].SendCommand("MANUALPUTDOWN");
        //        break;
        //    case "LEFTPUTDOWN":
        //        AreaCheck(playerLeftHand, robotID);
        //        robotList[robotID].SendCommand("MANUALPUTDOWN");
        //        break;
        //    default:
        //        robotList[robotID].SendCommand(command);
        //        break;
        //}
        robotList[robotID].SendCommand(command[1].ToUpper());
    }
    public void MenuCommand (string command)
    {
        foreach (var robot in robotList)
        {
            if (robot.PlayerSpotted())
            {
                robot.SendCommand(command);
            }
        }
    }
    public NavMeshObstacle PlayerNavObstacle()
    {
        return playerNavObstacle;
    }
    public void PackageCheck(Transform hand, int robotID)
    {
        Collider[] packages = Physics.OverlapSphere(hand.position, 3.0f, 1 << 9);
        for (int i = 0; i < packages.Length; i++)
        {
            if (i == 0)
            {
                closestPackage = 0;
            }
            else
            {
                if (Vector3.Distance(packages[i].transform.position, hand.position) < Vector3.Distance(packages[closestPackage].transform.position, hand.position))
                {
                    closestPackage = i;
                }
            }
        }
        robotList[robotID].SendPackage(packages[closestPackage].transform);
    }
    public void AreaCheck(Transform hand, int robotID)
    {
        Collider[] packages = Physics.OverlapSphere(hand.position, 3.0f, 1 << 10);
        for (int i = 0; i < packages.Length; i++)
        {
            if (i == 0)
            {
                closestDeliveryArea = 0;
            }
            else
            {
                if (Vector3.Distance(packages[i].transform.position, hand.position) < Vector3.Distance(packages[closestDeliveryArea].transform.position, hand.position))
                {
                    closestDeliveryArea = i;
                }
            }
        }
        robotList[robotID].SendDeliveryArea(packages[closestDeliveryArea].transform);
    }
    public void DebugTestMessage(string text)
    {
        Debug.Log(this.name + ": " + text);
    }
}
