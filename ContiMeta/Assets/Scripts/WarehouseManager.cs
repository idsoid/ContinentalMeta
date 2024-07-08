using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    //

    [SerializeField]
    private List<GameObject> robotCheck;
    [SerializeField]
    private List<TMP_Text> tasklistTexts;

    // Start is called before the first frame update
    void Start()
    {
        Typewriter.Add("Check your menu command for your Tasklist! Press the left/right arrows to cycle through the panels. Good luck! :)");
        Typewriter.Activate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
