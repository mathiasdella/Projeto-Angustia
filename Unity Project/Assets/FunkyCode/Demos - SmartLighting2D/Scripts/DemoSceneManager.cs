using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoSceneManager : MonoBehaviour {
	public string[] scenes;

	public void RunScene(int id) {
		SceneManager.LoadScene(scenes[id], LoadSceneMode.Single);
	}
}
