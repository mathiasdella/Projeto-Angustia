using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardrobeCatEvent : SceneObject
{
    bool closed = true;
    Animator anim;

    // Use this for initialization
    void Start()
    {
        this.isInteractible = true;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override bool OnInteract(PlayerController player)
    {
        if (closed)
        {
            anim.SetTrigger("Open");            
        }
        else
        {
            anim.SetTrigger("Close");
        }

        closed = !closed;

        player.BeginInteracting(0.50f);
        return true;
    }
}
