using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerMenu, robotMenu, robotFOV;
    private bool menuToggle = true;
    [SerializeField]
    private List<GameObject> circleGuides, gestureList, gestureAnimatorList, gestureInfo;
    private int gestureAnimatorStep = 0;
    public int tutorialStep = 0;
    [SerializeField]
    private Transform player, robotSpot;
    private Vector2 oldPos;
    private float oldRotY;
    private bool conditionMet = true;
    private bool gestureOn = false;
    [SerializeField]
    private GameObject textGuide, gestureBackground;
    [SerializeField]
    private RobotManager robot;

    // Start is called before the first frame update
    void Start()
    {
        robotFOV.SetActive(false);
        oldRotY = player.eulerAngles.y;
        oldPos = new Vector2(player.position.x, player.position.z);

        if (PlayerPrefs.GetString("player") == "first")
        {
            Typewriter.Add("Hello! Welcome to the AMR VR Project!");
            Typewriter.Add("Let's walk through the gestures and commands, shall we? Make sure to tap these boxes away after you are done reading them.");
            Typewriter.Add("We will start off with the Player Gestures!");
            Typewriter.Activate();
        }
        else
        {
            tutorialStep = 99;
            textGuide.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (conditionMet && !textGuide.activeSelf)
        {
            tutorialStep++;
            conditionMet = false;
            StepByStep();
        }
        else if (!conditionMet && !textGuide.activeSelf)
        {
            StepCheck();
        }

        foreach (var gesture in gestureAnimatorList)
        {
            if (gesture.activeSelf)
            {
                gestureBackground.SetActive(true);
                break;
            }
            else if (gestureBackground.activeSelf)
            {
                gestureBackground.SetActive(false);
            }
        }

        if (gestureOn && !textGuide.activeSelf)
        {
            gestureAnimatorList[gestureAnimatorStep].SetActive(true);
        }
    }
    
    //Gesture Hologram
    public void ToggleGesture()
    {
        foreach (var gesture in gestureList)
        {
            if (gesture.activeSelf)
            {
                gesture.SetActive(false);
                break;
            }
        }
        foreach (var info in gestureInfo)
        {
            if (info.activeSelf)
            {
                info.SetActive(false);
                break;
            }
        }
    }
    public void SwitchMenu()
    {
        menuToggle = !menuToggle;
        playerMenu.SetActive(menuToggle);
        robotMenu.SetActive(!menuToggle);
    }
    public void PlayGesture()
    {
        foreach (var gesture in gestureList)
        {
            if (gesture.TryGetComponent<Animator>(out Animator animator) && gesture.activeSelf)
            {
                animator.Play("Scene", -1, 0f);
                break;
            }
        }
    }

    //Tutorial Steps and Guide
    private void StepByStep()
    {
        switch (tutorialStep)
        {
            case 1:
                Typewriter.Add("First up, we have the Teleport Gesture! Hold out your index finger and thumb, with your palm facing upwards. " +
                    "To confirm the teleportation, pinch your finger and thumb. ");
                Typewriter.Activate();
                gestureOn = true;
                break;
            case 2:
                gestureAnimatorList[gestureAnimatorStep].SetActive(false);
                Typewriter.Add("Next, we have the Rotate Gesture! Have your index finger and thumb make a 'C'/reversed 'C' shape. (Depending on the hand) " +
                    "Arrows will appear, move your hand in the direction you desire. " +
                    "To confirm, pinch your finger and thumb.");
                Typewriter.Activate();
                gestureAnimatorStep++;
                break;
            case 3:
                gestureAnimatorList[gestureAnimatorStep].SetActive(false);
                Typewriter.Add("Finally, let's move onto our Robot Gestures! Hold out the upcoming gestures for a few seconds for the robot to register the command. ");
                Typewriter.Add("The green arc you see is the robot's vision.");
                Typewriter.Add("Now, move to the front of the robot!");
                Typewriter.Activate();
                gestureOn = false;
                circleGuides[0].SetActive(true);
                robotFOV.SetActive(true);
                break;
            case 4:
                circleGuides[0].SetActive(false);
                gestureAnimatorList[gestureAnimatorStep].SetActive(false);
                Typewriter.Add("Let's activate our robot! First, we have the Follow Gesture. " +
                    "Close all fingers, stick out your thumb and point it towards yourself.");
                Typewriter.Activate();
                gestureOn = true;
                gestureAnimatorStep++;
                break;
            case 5:
                gestureAnimatorList[gestureAnimatorStep].SetActive(false);
                Typewriter.Add("Next, we have the Pick Up/Grab Gesture. Go to the package. " +
                    "Close your hand, with your palm still facing inwards. " +
                    "Make sure your hand is near the package.");
                Typewriter.Activate();
                circleGuides[1].SetActive(true);
                gestureAnimatorStep++;
                break;
            case 6:
                gestureAnimatorList[gestureAnimatorStep].SetActive(false);
                circleGuides[1].SetActive(false);
                Typewriter.Add("Next, we have the Put Down/Release Gesture. The robot should still Follow, so proceed to the delivery area. " +
                    "Open up your hand again, but with your palm facing inwards. " +
                    "Make sure your hand is near the delivery area");
                Typewriter.Activate();
                circleGuides[2].SetActive(true);
                gestureAnimatorStep++;
                break;
            case 7:
                gestureAnimatorList[gestureAnimatorStep].SetActive(false);
                circleGuides[2].SetActive(false);
                Typewriter.Add("Next, we have the Stop Gesture. " +
                    "Open up your hand, with your palm facing the robot.");
                Typewriter.Activate();
                gestureAnimatorStep++;
                break;
            case 8:
                gestureAnimatorList[gestureAnimatorStep].SetActive(false);
                Typewriter.Add("Next, we have the Status Gesture. " +
                    "A simple thumbs up facing the robot.");
                Typewriter.Activate();
                gestureAnimatorStep++;
                break;
            case 9:
                gestureAnimatorList[gestureAnimatorStep].SetActive(false);
                Typewriter.Add("Last but not least, we have the Resume Gesture. " +
                    "Close your hand and point out only your index finger towards the front. ");
                Typewriter.Activate();
                gestureAnimatorStep++;
                break;
            case 10:
                gestureAnimatorList[gestureAnimatorStep].SetActive(false);
                Typewriter.Add("If you ever need it, there is a Menu Gesture. It contains robot and miscellaneous functions, along with the transcription of the voice detection. " + 
                    "Make the 'OK' symbol on your left hand, with the 'O' facing up, to toggle On/Off.");
                Typewriter.Activate();
                gestureAnimatorStep++;
                break;
            case 11:
                gestureAnimatorList[gestureAnimatorStep].SetActive(false);
                Typewriter.Add("And that's the tutorial! If you ever want to revisit certain gestures, go over to the projector to cycle through them. ");
                Typewriter.Add("Feel free to exit the tutorial room by going to the door and holding your hand on the handle.");
                Typewriter.Activate();
                gestureOn = false;
                circleGuides[3].SetActive(true);
                circleGuides[4].SetActive(true);
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
                gestureAnimatorList[gestureAnimatorStep].SetActive(false);
                if (Vector2.Distance(new Vector2(player.position.x, player.position.z), new Vector2(robotSpot.position.x, robotSpot.position.z)) <= 0.75f)
                {
                    conditionMet = true;
                }
                break;
            case 4:
                if (robot.StoreCommand == "FOLLOW")
                {
                    conditionMet = true;
                }
                break;
            case 5:
                if (robot.StoreCommand.Contains("PICKUP"))
                {
                    conditionMet = true;
                }
                break;
            case 6:
                if (robot.StoreCommand.Contains("PUTDOWN"))
                {
                    conditionMet = true;
                }
                break;
            case 7:
                if (robot.StoreCommand == "STOP")
                {
                    conditionMet = true;
                }
                break;
            case 8:
                if (robot.StoreCommand == "STATUS")
                {
                    conditionMet = true;
                }
                break;
            case 9:
                if (robot.StoreCommand == "GO")
                {
                    conditionMet = true;
                }
                break;
            case 10:
                if (GameManager.Instance.menuActive)
                {
                    conditionMet = true;
                }
                break;
            default:
                break;
        }
    }
    public void PoseRestrictor(string command)
    {
        if (textGuide.activeSelf && PlayerPrefs.GetString("player") == "first")
        {
            return;
        }
        switch (command)
        {
            case "FOLLOW":
                if (tutorialStep >= 4)
                {
                    GameManager.Instance.PlayerPoseDetection(command);
                }
                break;
            case "RIGHTPICKUP":
                if (tutorialStep >= 5)
                {
                    GameManager.Instance.PlayerPoseDetection(command);
                }
                break;
            case "LEFTPICKUP":
                if (tutorialStep >= 5)
                {
                    GameManager.Instance.PlayerPoseDetection(command);
                }
                break;
            case "RIGHTPUTDOWN":
                if (tutorialStep >= 6)
                {
                    GameManager.Instance.PlayerPoseDetection(command);
                }
                break;
            case "LEFTPUTDOWN":
                if (tutorialStep >= 6)
                {
                    GameManager.Instance.PlayerPoseDetection(command);
                }
                break;
            case "STOP":
                if (tutorialStep >= 7)
                {
                    GameManager.Instance.PlayerPoseDetection(command);
                }
                break;
            case "STATUS":
                if (tutorialStep >= 8)
                {
                    GameManager.Instance.PlayerPoseDetection(command);
                }
                break;
            case "GO":
                if (tutorialStep >= 9)
                {
                    GameManager.Instance.PlayerPoseDetection(command);
                }
                break;
            default:
                break;
        }
    }
}
