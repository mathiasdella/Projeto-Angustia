using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode] // Only 1 Lighting Manager Allowed
public class LightingManager2D : MonoBehaviour {
	public enum Preset {TopDown, SideScroller, Isometric};
	private static LightingManager2D instance;

	public Preset preset = Preset.TopDown;

	public Color darknessColor = Color.black;
	public float sunDirection = - Mathf.PI / 2;
	
	public float shadowDarkness = 1;


	public LightingMainBuffer2D mainBuffer;

	public Material penumbraMaterial;
	public Material occlusionEdgeMaterial;
	public Material occlusionBlurMaterial;
	public Material shadowBlurMaterial;
	public Material additiveMaterial;
	public Material whiteSpriteMaterial;

	public bool enable = true;
	public bool debug = false;

	static public int lightingLayer = 8;

	public const string shaderPath = "";

	static public LightingManager2D Get() {
		if (instance != null) {
			return(instance);
		}

		foreach(LightingManager2D manager in Object.FindObjectsOfType(typeof(LightingManager2D))) {
			instance = manager;
			return(instance);
		}

		// Create New Light Manager
		GameObject gameObject = new GameObject();
		gameObject.name = "Lighting Manager 2D";
		instance = gameObject.AddComponent<LightingManager2D>();
		instance.Start();

		return(instance);
	}

	void Start () {
		instance = this;

		// Lighting Materials
		additiveMaterial = new Material (Shader.Find (LightingManager2D.shaderPath + "Particles/Additive"));
		//additiveMaterial.mainTexture = Resources.Load ("textures/additive") as Texture;

		penumbraMaterial = new Material (Shader.Find (LightingManager2D.shaderPath + "Particles/Multiply"));
		penumbraMaterial.mainTexture = Resources.Load ("textures/penumbra") as Texture;

		occlusionEdgeMaterial = new Material (Shader.Find (LightingManager2D.shaderPath + "Particles/Multiply"));
		occlusionEdgeMaterial.mainTexture = Resources.Load ("textures/occlusionedge") as Texture;

		shadowBlurMaterial = new Material (Shader.Find (LightingManager2D.shaderPath + "Particles/Multiply"));
		shadowBlurMaterial.mainTexture = Resources.Load ("textures/shadowblur") as Texture;

		occlusionBlurMaterial = new Material (Shader.Find (LightingManager2D.shaderPath + "Particles/Multiply"));
		occlusionBlurMaterial.mainTexture = Resources.Load ("textures/occlussionblur") as Texture;

		whiteSpriteMaterial = new Material (Shader.Find ("SmartLighting2D/SpriteWhite"));

		transform.position = Vector3.zero;

		mainBuffer = LightingMainBuffer2D.Get(); 
	}
	
	void Update() {
		if (mainBuffer == null) { //???
			Start();
		}

		mainBuffer.darknessColor = darknessColor;

		if (enable) {
			mainBuffer.enabled = true;
			mainBuffer.gameObject.GetComponent<Camera>().enabled = true;
		} else {
			mainBuffer.enabled = false;
			mainBuffer.gameObject.GetComponent<Camera>().enabled = false;
		}
	}
		
	public void OnRenderObject() {
		if (enable == false) {
			return;
		}
		
		if (Camera.current != Camera.main) {
			return;
		}

		if (mainBuffer == null) {
			mainBuffer = LightingMainBuffer2D.Get();
			return;
		}	

		if (mainBuffer.bufferCamera == null) {
			return;
		}

		LightingDebug.LightMainCameraUpdates +=1 ;

		float sizeX = mainBuffer.bufferCamera.orthographicSize * ((float)Screen.width / Screen.height);
		Vector2 size = new Vector2(sizeX, mainBuffer.bufferCamera.orthographicSize);
		
		size.x *= ((float)Screen.width / (float)Screen.height) / (size.x / size.y);
		Vector3 pos = Camera.main.transform.position;
		pos.z += 1;

		Max2D.iDrawImage2(mainBuffer.material, pos, size, Camera.main.transform.eulerAngles.z, pos.z);
	}

	static public float GetSunDirection() {
		return(Get().sunDirection);
	}

	void OnGUI() {
		if (debug) {
			LightingDebug.OnGUI();
		}
	}

	public class LightingDebug {
		static public int LightBufferUpdates = 0;
		static public int ShowLightBufferUpdates = 0;

		static public int LightMainBufferUpdates = 0;
		static public int ShowLightMainBufferUpdates = 0;

		static public int LightMainCameraUpdates = 0;
		static public int ShowLightMainCameraUpdates = 0;

		static public int LightSourceDrawn = 0;
		static public int ShowLightSourceDrawn = 0;
		
		static public TimerHelper timer;

		static public void OnGUI() {
			if (timer == null) {
				LightingDebug.timer = TimerHelper.Create();
			}
			if (timer.GetMillisecs() > 1000) {
				ShowLightBufferUpdates = LightBufferUpdates;

				LightBufferUpdates = 0;

				ShowLightMainBufferUpdates = LightMainBufferUpdates;

				LightMainBufferUpdates = 0;

				ShowLightMainCameraUpdates = LightMainCameraUpdates;

				LightMainCameraUpdates = 0;

				timer = TimerHelper.Create();
			}

			ShowLightSourceDrawn = LightSourceDrawn;

			LightSourceDrawn = 0;

			GUI.Label(new Rect(10, 10, 200, 20), "Light Buffer Updates: " + ShowLightBufferUpdates);
			GUI.Label(new Rect(10, 30, 200, 20), "Light Main Buffer Updates: " + ShowLightMainBufferUpdates);
			GUI.Label(new Rect(10, 50, 200, 20), "Light Main Camera Updates: " + ShowLightMainCameraUpdates);
			GUI.Label(new Rect(10, 70, 200, 20), "Light Buffer Count: " + LightingBuffer2D.GetList().Count);
			GUI.Label(new Rect(10, 90, 200, 20), "Light Collider Count: " + LightingCollider2D.GetList().Count);

			Texture texture = LightingMainBuffer2D.Get().bufferCamera.activeTexture;
			GUI.Label(new Rect(10, 110, 200, 20), "Main Buffer Resolution: " + texture.width + "x" + texture.height);
			
			GUI.Label(new Rect(10, 130, 200, 20), "Light Sources Drawn: " + ShowLightSourceDrawn);
		}
	}
}