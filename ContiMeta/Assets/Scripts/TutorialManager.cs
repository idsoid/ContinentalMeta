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

    // Start is called before the first frame update
    void Start()
    {
        
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
}
