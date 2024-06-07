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
    private GameObject startZone;

    // Start is called before the first frame update
    void Start()
    {
        Typewriter.Add("Hello! Welcome to the AMR VR Project!");
        Typewriter.Add("Let's walk through the gestures and commands, shall we?");
        Typewriter.Activate();
    }

    // Update is called once per frame
    void Update()
    {
        
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
    public void StepByStep()
    {
        switch (tutorialStep)
        {
            case 1:
                Typewriter.Add("We will start off with the Player Gestures.");
                Typewriter.Add("First up, we have the Teleport Gesture! Hold out your index finger and thumb, with your palm facing upwards. " +
                    "An arc should appear indicating where you want to be teleported." +
                    "To confirm the teleportation, pinch your finger and thumb.");
                Typewriter.Activate();
                break;
            case 2:
                Typewriter.Add("Next, we have the Rotate Gesture! Have your index finger and thumb make a reversed 'C' shape, with it facing towards you. " +
                    "Two arrows will appear, indicating where your orientation will face. Move your hand in the direction you desire." +
                    "To confirm the rotation, pinch your finger and thumb.");
                Typewriter.Activate();
                break;
            case 3:
                Typewriter.Add("Let's activate our robot! Do the 'Resume' gesture or say, 'Bot One, Go'!");
                Typewriter.Activate();
                break;
            default:
                break;
        }
    }
}
