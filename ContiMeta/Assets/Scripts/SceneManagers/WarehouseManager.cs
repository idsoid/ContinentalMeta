using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class WarehouseManager : MonoBehaviour
{
    //tasks: 
    //activate robot 1 with a command
    //place all misplaced packages at a delivery area 0/6
    //check robots status 0/4
    //command each robot to follow 0/4
    //stop 2 robots 0/2
    //free robot 3 from being stuck

    [SerializeField]
    private List<RobotManager> robotCheck;
    [SerializeField]
    private List<TMP_Text> tasklistTexts;

    public bool robot1Active = false;
    [SerializeField]
    private List<GameObject> packages;
    private List<bool> robotStatus = new() { false, false, false, false };
    private List<bool> robotFollow = new() { false, false, false, false };
    private List<bool> robotStop = new() { false, false, false, false };
    private int statusCount, followCount, stopCount = 0;
    private bool robotStuck = false;

    // Start is called before the first frame update
    void Start()
    {
        Typewriter.Add("Check your menu command for your Tasklist! Press the left/right arrows to cycle through the panels. Good luck! :)");
        Typewriter.Activate();
    }

    // Update is called once per frame
    void Update()
    {
        if (robotCheck[2].GetComponent<RobotController>().currentState == RobotController.States.STUCK && !robotStuck)
        {
            robotStuck = true;
        }

        //Task 1
        if (robotCheck[0].StoreCommand != "" && !robot1Active)
        {
            robot1Active = true;
            tasklistTexts[0].color = Color.green;
            tasklistTexts[0].text = "Activate Robot 1: 1/1";
        }
        //Task 2
        if (packages.Count > 0)
        {
            foreach (var package in packages)
            {
                if (package == null)
                {
                    packages.Remove(package);
                    if (packages.Count <= 0)
                    {
                        tasklistTexts[1].color = Color.green;
                    }
                }
                tasklistTexts[1].text = "Place all misplaced packages at a delivery area: " + (6 - packages.Count) + "/6";
            }
        }
        Debug.Log("packages left: " + packages.Count);
        //Task 3
        if (statusCount < 4)
        {
            statusCount = 0;
            for (int i = 0; i < robotCheck.Count; i++)
            {
                if (robotCheck[i].StoreCommand == "STATUS" && !robotStatus[i])
                {
                    robotStatus[i] = true;
                }
            }
            foreach (var item in robotStatus)
            {
                if (item)
                {
                    statusCount += 1;
                    if (statusCount >= 4)
                    {
                        tasklistTexts[2].color = Color.green;
                    }
                }
            }
            tasklistTexts[2].text = "Check robots status: " + statusCount + "/4";
        }
        //Task 4
        if (followCount < 2)
        {
            followCount = 0;
            for (int i = 0; i < robotCheck.Count; i++)
            {
                if (robotCheck[i].StoreCommand == "FOLLOW" && !robotFollow[i])
                {
                    robotFollow[i] = true;
                }
            }
            foreach (var item in robotFollow)
            {
                if (item)
                {
                    followCount += 1;
                    if (followCount >= 2)
                    {
                        tasklistTexts[3].color = Color.green;
                    }
                }
            }
            tasklistTexts[3].text = "Command robots to follow: " + followCount + "/2";
        }
        //Task 5
        if (stopCount < 4)
        {
            stopCount = 0;
            for (int i = 0; i < robotCheck.Count; i++)
            {
                if (robotCheck[i].StoreCommand == "STOP" && !robotStop[i])
                {
                    robotStop[i] = true;
                }
            }
            foreach (var item in robotFollow)
            {
                if (item)
                {
                    stopCount += 1;
                    if (stopCount >= 4)
                    {
                        tasklistTexts[4].color = Color.green;
                    }
                }
            }
            tasklistTexts[4].text = "Stop each of the robots: " + stopCount + "/4";
        }
        //Task 6
        if (robotCheck[2].GetComponent<RobotController>().currentState != RobotController.States.STUCK && robotStuck)
        {
            robotStuck = false;
            tasklistTexts[5].color = Color.green;
            tasklistTexts[5].text = "Free Robot 3 from being stuck: 1/1";
        }
    }
}
