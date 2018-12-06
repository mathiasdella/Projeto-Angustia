using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeController : MonoBehaviour {
	public GameObject rearWheel;
	public GameObject frontWheel;
	public GameObject body;

	public float SwitchAnimation;
	public bool switchDirection = true;
	public float SwitchRot;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetKey(KeyCode.UpArrow)) {
			if (switchDirection) {
				Rigidbody2D rigidbody2D = rearWheel.GetComponent<Rigidbody2D>();
				rigidbody2D.AddTorque(-1000);
			} else {
				Rigidbody2D rigidbody2D = frontWheel.GetComponent<Rigidbody2D>();
				rigidbody2D.AddTorque(1000);
			}
			
		}

		if (Input.GetKey(KeyCode.DownArrow)) {
			Rigidbody2D rigidbody2D = rearWheel.GetComponent<Rigidbody2D>();
			rigidbody2D.angularVelocity = 0;
			rigidbody2D = frontWheel.GetComponent<Rigidbody2D>();
			rigidbody2D.angularVelocity = 0;
		}

		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			Rigidbody2D rigidbody2D = rearWheel.GetComponent<Rigidbody2D>();
			rigidbody2D.AddForce(new Vector2(0, 3000));
			rigidbody2D = frontWheel.GetComponent<Rigidbody2D>();
			rigidbody2D.AddForce(new Vector2(0, -3000));
		}

		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			Rigidbody2D rigidbody2D = frontWheel.GetComponent<Rigidbody2D>();
			rigidbody2D.AddForce(new Vector2(0, 3000));
			rigidbody2D = rearWheel.GetComponent<Rigidbody2D>();
			rigidbody2D.AddForce(new Vector2(0, -3000));
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			switchDirection = !switchDirection;
			
		}

		Vector3 scale = body.transform.localScale;
		if (switchDirection) {
			scale.x = 1.0f * 0.05f + scale.x * 0.95f;
			body.transform.localScale = scale;
		} else {
			scale.x = -1.0f * 0.05f + scale.x * 0.95f;
			body.transform.localScale = scale;

		}
	}
}
