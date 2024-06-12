using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> gestureList;
    [SerializeField]
    private List<Animator> gestureAnimatorList;
    private int gestureIndicator = 0;
    private int tutorialStep = 0;
    [SerializeField]
    private List<Transform> goalPos = new();
    [SerializeField]
    private Transform player, robotSpot;
    private Vector2 oldPos;
    private float oldRotY;
    private bool conditionMet = true;
    [SerializeField]
    private Image background;
    [SerializeField]
    private RobotManager robot;

    // Start is called before the first frame update
    void Start()
    {
        oldRotY = player.eulerAngles.y;
        oldPos = new Vector2(player.position.x, player.position.z);
        Typewriter.Add("Hello! Welcome to the AMR VR Project!");
        Typewriter.Add("Let's walk through the gestures and commands, shall we?");
        Typewriter.Add("We will start off with the Player Gestures!");
        Typewriter.Activate();
    }

    // Update is called once per frame
    void Update()
    {
        if (conditionMet)
        {
            tutorialStep++;
            conditionMet = false;
            StepByStep();
        }
        else if (!conditionMet && !background.enabled)
        {
            StepCheck();
        }
        if (gestureAnimatorList[0].GetCurrentAnimatorStateInfo(0).length < gestureAnimatorList[0].GetCurrentAnimatorStateInfo(0).normalizedTime)
        {
            Debug.Log("playing");
        }
    }

    public void GestureDisplay(int scroll)
    {
        gestureList[gestureIndicator].SetActive(false);
        gestureIndicator += scroll;
        if (gestureIndicator < 0)
        {
            gestureIndicator = gestureList.Count - 1;
        }
        else if (gestureIndicator > gestureList.Count - 1)
        {
            gestureIndicator = 0;
        }
        gestureList[gestureIndicator].SetActive(true);
    }
    public void PlayGesture()
    {
        gestureAnimatorList[gestureIndicator].Play("Scene", -1, 0f);
    }
    private void StepByStep()
    {
        switch (tutorialStep)
        {
            case 1:
                Typewriter.Add("First up, we have the Teleport Gesture! Hold out your index finger and thumb, with your palm facing upwards. " +
                    "To confirm the teleportation, pinch your finger and thumb.");
                Typewriter.Activate();
                gestureAnimatorList[0].gameObject.SetActive(true);
                if (gestureAnimatorList[0].GetCurrentAnimatorStateInfo(0).length < gestureAnimatorList[0].GetCurrentAnimatorStateInfo(0).normalizedTime)
                {
                    gestureAnimatorList[0].Play("Scene");
                }
                break;
            case 2:
                Typewriter.Add("Next, we have the Rotate Gesture! Have your index finger and thumb make a reversed 'C' shape. " +
                    "Two arrows will appear, move your hand in the direction you desire. " +
                    "To confirm the rotation, pinch your finger and thumb.");
                Typewriter.Activate();
                break;
            case 3:
                Typewriter.Add("Finally, let's move onto our Robot Gestures! Hold out the following gestures for a few seconds for the robot to register the command. " +
                    "Move to the front of the robot.");
                Typewriter.Activate();
                break;
            case 4:
                Typewriter.Add("Let's activate our robot! First, we have the Resume Gesture. " + 
                    "Close your hand and point out only your index finger towards the front. ");
                Typewriter.Activate();
                break;
            case 5:
                Typewriter.Add("Next, we have the Stop Gesture. " +
                    "Open up your hand, with your palm facing the robot.");
                Typewriter.Activate();
                break;
            case 6:
                Typewriter.Add("Next, we have the Follow Gesture. " +
                    "Close all fingers, stick out your thumb and point it towards yourself.");
                Typewriter.Activate();
                break;
            case 7:
                Typewriter.Add("Next, we have the Pick Up Gesture. " +
                    "Open up your hand again, but with your palm facing inwards. " +
                    "Make sure your hand is near a package");
                Typewriter.Activate();
                break;
            case 8:
                Typewriter.Add("Next, we have the Put Down Gesture. " +
                    "Close your hand, with your palm still facing inwards. " +
                    "Make sure your hand is near a delivery area.");
                Typewriter.Activate();
                break;
            case 9:
                Typewriter.Add("Next, we have the Status Gesture. " +
                    "A simple thumbs up facing the robot.");
                Typewriter.Activate();
                break;
            case 10:
                Typewriter.Add("And that's the tutorial! If you ever want to revisit certain gestures, go over to the projector to cycle through them.");
                Typewriter.Activate();
                break;
            default:
                break;
        }
    }
    private void StepCheck()
    {
        switch (tutorialStep)
        {
            case 1:
                if (oldPos != new Vector2(player.position.x, player.position.z))
                {
                    conditionMet = true;
                }
                oldRotY = player.eulerAngles.y;
                break;
            case 2:
                if (oldRotY != player.eulerAngles.y)
                {
                    conditionMet = true;
                }
                break;
            case 3:
                if (Vector2.Distance(new Vector2(player.position.x, player.position.z), new Vector2(robotSpot.position.x, robotSpot.position.z)) <= 1.5f)
                {
                    conditionMet = true;
                }
                break;
            case 4:
                if (robot.StoreCommand == "GO")
                {
                    conditionMet = true;
                }
                break;
            case 5:
                if (robot.StoreCommand == "STOP")
                {
                    conditionMet = true;
                }
                break;
            case 6:
                if (robot.StoreCommand == "FOLLOW")
                {
                    conditionMet = true;
                }
                break;
            case 7:
                if (robot.StoreCommand.Contains("PICKUP"))
                {
                    conditionMet = true;
                }
                break;
            case 8:
                if (robot.StoreCommand.Contains("PUTDOWN"))
                {
                    conditionMet = true;
                }
                break;
            case 9:
                if (robot.StoreCommand == "STATUS")
                {
                    conditionMet = true;
                }
                break;
            default:
                break;
        }
    }

    //postpone
    public void PoseRestrictor(string command)
    {
        if (background.enabled)
        {
            return;
        }

        GameManager.Instance.PoseCommand(0, command);
        switch (command)
        {
            case "GO":
                if (tutorialStep >= 4)
                {

                }
                break;
            case "STOP":
                if (tutorialStep >= 5)
                {

                }
                break;
            case "FOLLOW":
                
                break;
            case "RIGHTPICKUP":
                
                break;
            case "LEFTPICKUP":
                
                break;
            case "RIGHTPUTDOWN":
                
                break;
            case "LEFTPUTDOWN":
                
                break;
            case "STATUS":
                
                break;
            default:
                
                break;
        }
    }
}
