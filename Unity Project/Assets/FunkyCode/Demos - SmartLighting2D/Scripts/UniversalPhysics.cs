using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalPhysics : MonoBehaviour {
	Rigidbody2D rigidBody;
	// Use this for initialization
	void Start () {
		Physics2D.gravity = new Vector2 (0, 0);

		rigidBody = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		float angle = (float)Vector2D.Atan2 (new Vector2D(0, 0), new Vector2D(transform.position));
	
		rigidBody.AddForce (new Vector2(Mathf.Cos(angle) * 3, Mathf.Sin(angle) * 3));
	}
}
