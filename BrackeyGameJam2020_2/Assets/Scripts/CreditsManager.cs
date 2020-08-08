using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    private SceneFader _fader;

    public Transform target;

    public float scrollSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _fader = GetComponent<SceneFader>();
        StartCoroutine(_fader.Fade(SceneFader.FadeDirection.Out));
    }


    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, scrollSpeed * Time.deltaTime);
    }

    public void MainMenu()
    {
        PlayerPrefs.SetString("NextScene", "start_menu");

        SceneManager.LoadScene("loading_screen");
    }
}
