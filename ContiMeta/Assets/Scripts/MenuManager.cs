using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private ToggleGroup toggleGroup;
    [SerializeField]
    private VideoPlayer firstVid, recurringVid;
    private bool startVid = false;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(LoadInBackground());
        //StartCoroutine(LoadAfterVid());
    }

    // Update is called once per frame
    void Update()
    {
        //-2.5 150 110
    }

    private IEnumerator LoadInBackground(VideoPlayer vidPlayer)
    {
        yield return null;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("TutorialScene");
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f && (vidPlayer.frame == (long)vidPlayer.frameCount - 1))
            {
                yield return new WaitForSecondsRealtime(1.0f);
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
    
    public void PlayerType()
    {
        foreach (var toggle in toggleGroup.ActiveToggles())
        {
            if (toggle.isOn)
            {
                if (toggle.name == "First-Time")
                {
                    PlayerPrefs.SetString("player", "first");
                    PlayerPrefs.Save();
                    firstVid.Play();
                    StartCoroutine(LoadInBackground(firstVid));
                    break;
                }
                else
                {
                    PlayerPrefs.SetString("player", "recurring");
                    PlayerPrefs.Save();
                    recurringVid.Play();
                    StartCoroutine(LoadInBackground(recurringVid));
                    break;
                }
            }
        }
    }
}
