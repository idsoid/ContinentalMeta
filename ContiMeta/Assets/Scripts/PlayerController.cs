using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform playerCenter;
    [SerializeField]
    private GameObject canvasCommandPopupController, canvasCommandPopupHand;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        playerCenter.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {     
        if (OVRPlugin.GetConnectedControllers() == OVRPlugin.Controller.Touch)
        {
            Debug.Log("controllers enabled");
            if (OVRInput.GetDown(OVRInput.Button.Three))
            {
                gameManager.ToggleMenu();
            }
            canvasCommandPopupController.SetActive(gameManager.GetMenuActive());
            canvasCommandPopupHand.SetActive(false);
        }
        else if (OVRPlugin.GetConnectedControllers() == OVRPlugin.Controller.Hands)
        {
            Debug.Log("hands enabled");
            canvasCommandPopupHand.SetActive(gameManager.GetMenuActive());
            canvasCommandPopupController.SetActive(false);
        }
        else
        {
            Debug.Log("nothing enabled");
            canvasCommandPopupController.SetActive(false);
            canvasCommandPopupHand.SetActive(false);
        }

        //controller info - position: 0, 0.125f, 0.075f; rotation: 15, 0, 0;
        //hand info - position: 0.2f, 0, 0.1f; rotation: -2.5, 150, 110;

        if (Vector2.Distance(new Vector2(playerCenter.position.x, playerCenter.position.z), new Vector2(transform.position.x, transform.position.z)) >= 0.5f)
        {
            Debug.Log("teleported");
            playerCenter.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        }
        if (Mathf.Abs(playerCenter.rotation.eulerAngles.y - transform.rotation.eulerAngles.y) > 1f)
        {
            Debug.Log("rotated");
            playerCenter.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }
}
