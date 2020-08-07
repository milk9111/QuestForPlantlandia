using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneControl : MonoBehaviour
{
    public GameObject loadingScreen;
    public Text blinkingDots;

    private AsyncOperation async;

    void Start()
    {
        StartCoroutine(LoadingScreen());
        StartCoroutine(BlinkingDots());
    }

    IEnumerator BlinkingDots()
    {
        blinkingDots.text = "";
        while (!async.isDone)
        {
            if (blinkingDots.text.Length > 6)
            {
                blinkingDots.text = "";
            }

            blinkingDots.text += ".";
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator LoadingScreen()
    {
        loadingScreen.SetActive(true);
        async = SceneManager.LoadSceneAsync(PlayerPrefs.GetString("NextScene"));
        async.allowSceneActivation = false;

        yield return new WaitForSeconds(2.5f);

        while (async.isDone == false)
        {
            if (async.progress == 0.9f)
            {
                async.allowSceneActivation = true;
            }
            yield return null;
        }
    }

}
