using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer vidPlayer;
    public bool vidDone = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadInBackground());
        StartCoroutine(LoadAfterVid());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LoadAfterVid()
    {
        yield return new WaitUntil(() => vidPlayer.frame == (long)vidPlayer.frameCount - 1);
        vidDone = true;    
    }
    private IEnumerator LoadInBackground()
    {
        yield return null;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("TutorialScene");
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f && vidDone)
            {
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
