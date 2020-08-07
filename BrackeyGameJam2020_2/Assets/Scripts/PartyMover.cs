using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMover : MonoBehaviour
{
    public Transform target;
    public Transform start;

    // Movement speed in units per second.
    public float speed = 1.0F;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength;

    void Start()
    {
        start = transform;

        // Keep a note of the time the movement started.
        startTime = Time.time;

        var t = new Vector3(0, 0, 0);
        if (target != null)
        {
            t = new Vector3(target.position.x, target.position.y, 0);
        }

        // Calculate the journey length.
        journeyLength = Vector3.Distance(start.position, t);
    }

    void Update()
    {
        if (journeyLength == 0)
        {
            return;
        }

        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTime) * speed;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;

        var t = new Vector3(0, 0, -10);
        if (target != null)
        {
            t = new Vector3(target.position.x, target.position.y, 0);
        }

        transform.position = Vector3.Lerp(start.position, t, fractionOfJourney);
    }

    public void SetTarget(Transform t)
    {
        start = transform;
        target = t;

        // Keep a note of the time the movement started.
        startTime = Time.time;

        // Calculate the journey length.
        journeyLength = Vector3.Distance(start.position, target.position);
    }
}
