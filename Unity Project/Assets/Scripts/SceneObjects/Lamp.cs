using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : SceneObject
{

    private LightingSource2D lightSource;

    // Use this for initialization
    void Start()
    {
        this.isInteractible = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override bool OnInteract(PlayerController player)
    {
        if (lightSource == null)
        {
            lightSource = GetComponent<LightingSource2D>();
        }

        if (lightSource == null)
        {
            Debug.LogError("Lamp Lightsource2D not found!");
            return false;
        }

        lightSource.enabled = !lightSource.enabled;
        return true;
    }
}
