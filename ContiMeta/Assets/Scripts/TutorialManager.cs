using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> gestureList;
    [SerializeField]
    private List<Animator> gestureAnimatorList;
    private int gestureIndicator = 0;
    private int tutorialStep = 0;
    [SerializeField]
    private List<Transform> arrowPos = new();
    [SerializeField]
    private Transform player, robotSpot;
    private Vector2 oldPos;
    private float oldRotY;
    public bool conditionMet = true;

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
        //if (tutorialStep == 2 && oldRot != playerCenter.eulerAngles)
        //{

        //}
        if (!conditionMet)
        {
            StepCheck();
        }
        Debug.Log("distance: " + Vector2.Distance(new Vector2(player.position.x, player.position.z), new Vector2(robotSpot.position.x, robotSpot.position.z)));
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
                break;
            case 2:
                Typewriter.Add("Next, we have the Rotate Gesture! Have your index finger and thumb make a reversed 'C' shape. " +
                    "Two arrows will appear, move your hand in the direction you desire. " +
                    "To confirm the rotation, pinch your finger and thumb.");
                Typewriter.Activate();
                break;
            case 3:
                Typewriter.Add("Finally, let's move onto our Robot Gestures! Move to the front of the robot.");
                Typewriter.Activate();
                break;
            case 4:
                Typewriter.Add("Let's activate our robot! Do the 'Resume' gesture or say, 'Bot One, Go'!");
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

                break;
            default:
                break;
        }
    }
}
