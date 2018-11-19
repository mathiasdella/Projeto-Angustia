using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBox : SceneObject {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override bool OnInteract(PlayerController player)
    {
        Debug.Log("Ïnteracting with fuse box.");
        player.BeginInteracting(0.50f);
        return true;
    }
}
