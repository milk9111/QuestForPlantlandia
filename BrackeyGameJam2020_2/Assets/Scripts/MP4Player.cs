using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MP4Player : MonoBehaviour
{
    public RawImage rawImage;
    public VideoPlayer activeVideoPlayer;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(PlayVideo());
    }

    // Use this for the main functionality
    IEnumerator PlayVideo()
    {
        activeVideoPlayer.Prepare(); //initiates playback engine preparation
        yield return new WaitForSeconds(1); 

        rawImage.texture = activeVideoPlayer.texture;
        activeVideoPlayer.Play();
    }
}
