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
    private List<TypewriterMessage> messages = new();
    private TypewriterMessage currentMsg = null;
    private int msgIndex = 0;
    [SerializeField]
    private GameObject rayInteraction;
    private Image background;
    
    public static void Add(string msg, Action callback = null)
    {
        TypewriterMessage typeMsg = new(msg, callback);
        instance.messages.Add(typeMsg);
    }
    public static void Activate()
    {
        instance.currentMsg = instance.messages[0];
        instance.tmp.gameObject.SetActive(true);
    }

    private void Awake()
    {
        instance = this;
        background = GetComponent<Image>();
    }
    // Update is called once per frame
    void Update()
    {
        if (messages.Count > 0 && currentMsg != null)
        {
            background.enabled = true;
            rayInteraction.SetActive(true);
            currentMsg.Update();
            tmp.text = currentMsg.GetMsg();
        }
    }

    public void WriteNextMessageInQueue()
    {
        //If active, show entire string
        if (currentMsg != null && currentMsg.IsActive())
        {
            tmp.text = currentMsg.GetFullMsgAndCallback();
            currentMsg = null;
            return;
        }

        msgIndex++;
        if (msgIndex >= messages.Count)
        {
            currentMsg = null;
            instance.tmp.gameObject.SetActive(false);
            messages.Clear();
            background.enabled = false;
            rayInteraction.SetActive(false);
            return;
        }
        currentMsg = messages[msgIndex];
    }
}
