using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Voice;

public class VoiceManager : MonoBehaviour
{
    public AppVoiceExperience voiceExp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetUp(OVRInput.Button.One))
        {
            voiceExp.Activate();
        }
    }
}
