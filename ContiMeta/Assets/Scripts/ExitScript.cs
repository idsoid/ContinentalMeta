using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Oculus.Interaction.Samples;

public class ExitScript : MonoBehaviour
{
    [SerializeField]
    private string sceneName;
    [SerializeField]
    private SceneLoader sceneLoader;
    [SerializeField]
    private Transform playerLeft, playerRight;
    private bool handIn = false;
    private float handInTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, playerLeft.position) <= 0.1f || Vector3.Distance(transform.position, playerRight.position) <= 0.1f)
        {
            Debug.Log("touching exit");
            handIn = true;
        }
        else if (Vector3.Distance(transform.position, playerLeft.position) > 0.1f || Vector3.Distance(transform.position, playerRight.position) > 0.1f)
        {
            Debug.Log("not touching exit");
            handIn = false;
        }

        if (handIn)
        {
            handInTimer += Time.deltaTime;
            if (handInTimer >= 1.5f)
            {
                handIn = false;
                sceneLoader.Load(sceneName);
            }
        }
    }
}
