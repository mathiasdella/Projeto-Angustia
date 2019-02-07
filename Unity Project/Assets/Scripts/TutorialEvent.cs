using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvent : MonoBehaviour
{
    public LightingSource2D[] lightSources;
    public float[] lightTimers;
    public float timer;
    public int index = -1;

    public bool EventComplete { get { return index >= lightTimers.Length; } }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (index < 0 || index >= lightTimers.Length)
        {
            return;
        }

        timer += Time.deltaTime;

        if (index < lightTimers.Length)
        {
            if (timer >= lightTimers[index])
            {
                index++;
                for (int j = 0; j < lightSources.Length; j++)
                {
                    lightSources[j].enabled = !lightSources[j].enabled;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (index < 0 && col.name == "Player")
        {
            index = 0;
            timer = 0;
        }
    }
}
