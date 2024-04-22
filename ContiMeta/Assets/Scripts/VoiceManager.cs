using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Voice;
using TMPro;

public class VoiceManager : MonoBehaviour
{
    private GameManager gameManager;
    public AppVoiceExperience voiceExp;
    public TMP_Text tmp;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        StartCoroutine(VoiceCheck());
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("voice active: " + voiceExp.Active);
    }

    private IEnumerator VoiceCheck()
    {
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(0.5f);

        while (true)
        {
            yield return wait;
            if (!voiceExp.Active)
            {
                voiceExp.Activate();
            }
        }
    }
    public void PlayerVoiceDetection(string[] info)
    {
        Debug.Log("voice received: " + info[0]);
        tmp.text = "value: " + info[0];
        gameManager.SendCommand(info[0].ToUpper());
    }
}
