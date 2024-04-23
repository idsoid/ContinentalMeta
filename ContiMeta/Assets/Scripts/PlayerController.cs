using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject canvasCommandPopup;
    public bool menuActive;

    // Start is called before the first frame update
    void Start()
    {
        menuActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            ToggleMenu();
        }

        canvasCommandPopup.SetActive(menuActive);
    }

    public void ToggleMenu()
    {
        menuActive = !menuActive;
    }
}
