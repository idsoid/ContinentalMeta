using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    private float floatTime = 0.0f;
    private int dir = 1;
    
    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += new Vector3(0, 1, 0);
        transform.position += new Vector3(0, 0.001f * dir, 0);

        floatTime += Time.deltaTime * 0.25f;
        if (floatTime >= 0.2f)
        {
            dir *= -1;
            floatTime = 0f;
        }
    }
}
