using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TypewriterMessage
{
    private float timer = 0;
    private int charIndex = 0;
    private float timePerChar = 0.05f;

    [SerializeField]
    public string currentMsg = null;
    private string displayMsg = null;

    private Action onActionCallback = null;

    public TypewriterMessage(string msg, Action callback = null)
    {
        onActionCallback = callback;
        currentMsg = msg;
    }
    public void Callback()
    {
        if (onActionCallback != null)
        {
            onActionCallback();
        }
    }
    public string GetFullMsgAndCallback()
    {
        if (onActionCallback != null)
        {
            onActionCallback();
        }
        return currentMsg;
    }
    public string GetFullMsg()
    {
        return currentMsg;
    }
    public string GetMsg()
    {
        return displayMsg;
    }

    public void Update()
    {
        if (string.IsNullOrEmpty(currentMsg))
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            //Keep showing msg
            timer += timePerChar;
            charIndex++;

            //Assign message
            displayMsg = currentMsg.Substring(0, charIndex);
            displayMsg += "<color=#00000000>" + currentMsg.Substring(charIndex) + "</color>";

            //End of sentence
            if (charIndex >= currentMsg.Length)
            {
                Callback();
                currentMsg = null;
            }
        }
    }

    public bool IsActive()
    {
        if (string.IsNullOrEmpty(currentMsg))
        {
            return false;
        }
        return charIndex < currentMsg.Length;
    }
}
public class Typewriter : MonoBehaviour
{
    public TMP_Text tmp;
    private static Typewriter instance;
    private List<TypewriterMessage> messages = new List<TypewriterMessage>();
    private TypewriterMessage currentMsg = null;
    private int msgIndex = 0;

    public static void Add(string msg, Action callback = null)
    {
        TypewriterMessage typeMsg = new TypewriterMessage(msg, callback);
        instance.messages.Add(typeMsg);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
