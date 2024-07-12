using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Oculus.Interaction.Samples;

public class ExitScript : MonoBehaviour
{
    [SerializeField]
    private GameObject doorCircle;
    [SerializeField]
    private string sceneName;
    [SerializeField]
    private SceneLoader sceneLoader;
    [SerializeField]
    private Transform playerLeft, playerRight;
    private bool handIn = false;
    private float handInTimer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, playerLeft.position) <= 0.1f || Vector3.Distance(transform.position, playerRight.position) <= 0.15f)
        {
            Debug.Log("touching exit");
            handIn = true;
        }
        else if (Vector3.Distance(transform.position, playerLeft.position) > 0.1f || Vector3.Distance(transform.position, playerRight.position) > 0.15f)
        {
            Debug.Log("not touching exit");
            handIn = false;
        }

        if (handIn)
        {
            doorCircle.SetActive(true);
            handInTimer += Time.deltaTime;
            if (handInTimer >= 1.5f)
            {
                handIn = false;
                sceneLoader.Load(sceneName);
            }
        }
        else
        {
            doorCircle.SetActive(false);
            handInTimer = 0f;
        }
    }
}
