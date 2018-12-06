using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachCamera : MonoBehaviour {
	public Transform gameObjectTransform;
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = gameObjectTransform.position;
		pos.z = transform.position.z;
		transform.position = pos;
	}
}
