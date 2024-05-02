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
        WaitForSecondsRealtime wait = new(0.5f);

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
        string allinfo = "";
        foreach (var item in info)
        {
            Debug.Log("voice received: " + item);
            tmp.text += item;
            allinfo += item + " ";
        }
        Debug.Log("string info: " + allinfo + info.Length);
        //gameManager.VoiceCommand(info);
    }
}
