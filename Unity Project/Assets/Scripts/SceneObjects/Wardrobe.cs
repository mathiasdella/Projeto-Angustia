using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wardrobe : SceneObject {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override bool OnInteract(PlayerController player)
    {
        player.BeginHiding(0.50f);
        return true;
    }
}
