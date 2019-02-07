using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBox : SceneObject
{

    public TutorialEvent targetEvent;
    public LightingSource2D lightSource;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override bool OnInteract(PlayerController player)
    {        
        if (targetEvent == null || lightSource == null)
        {
            return false;
        }

        if (targetEvent.EventComplete)
        {
            lightSource.enabled = true;
        }


        player.BeginInteracting(0.50f);
        return true;
    }
}
