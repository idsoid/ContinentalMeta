using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotManager : MonoBehaviour
{
    [SerializeField]
    private RobotController robotController;
    [SerializeField]
    private FieldOfView robotView;
    private float scanTimer = 0.0f;
    private bool startTimer = false;
    private string storeCommand = "";
    public float ScanTimer { get => scanTimer; set => scanTimer = value; }
    public bool StartTimer { get => startTimer; set => startTimer = value; }
    public string StoreCommand { get => storeCommand; set => storeCommand = value; }

    public void SendCommand(string command)
    {
        robotController.ReceiveCommand(command);
        storeCommand = command;
    }
    public void SendPackage(Transform package)
    {
        robotController.ReceivePackage(package);
    }
    public void SendDeliveryArea(Transform deliveryArea)
    {
        robotController.ReceiveDeliveryArea(deliveryArea);
    }
    public bool PlayerSpotted()
    {
        return robotView.PlayerSpotted;
    }
    public bool PlayerInRange()
    {
        return robotView.PlayerInRange;
    }
    public void Restart()
    {
        scanTimer = 0.0f;
        startTimer = false;
    }
}
