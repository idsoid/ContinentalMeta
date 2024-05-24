using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotManager : MonoBehaviour
{
    [SerializeField]
    private RobotController robotController;
    [SerializeField]
    private FieldOfView robotView;
    public float scanTimer = 0.0f;
    public bool startTimer = false;
    public float ScanTimer { get => scanTimer; set => scanTimer = value; }
    public bool StartTimer { get => startTimer; set => startTimer = value; }

    public void SendCommand(string command)
    {
        robotController.ReceiveCommand(command);
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
        return robotView.GetPlayerSpotted();
    }
    public bool PlayerInRange()
    {
        return robotView.GetPlayerInRange();
    }
    public void Restart()
    {
        scanTimer = 0.0f;
        startTimer = false;
    }
}
