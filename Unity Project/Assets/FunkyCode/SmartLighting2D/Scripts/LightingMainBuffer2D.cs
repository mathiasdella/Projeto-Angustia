using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode] 
public class LightingMainBuffer2D : MonoBehaviour {
	static private LightingMainBuffer2D instance;

	private RenderTexture renderTexture;

	public Material material; // Static
	public Camera bufferCamera; // Static
	public Color darknessColor; // Static

	public int screenWidth = 640;
	public int screenHeight = 480;

	const float uv0 = 0;
	const float uv1 = 1;
	const float pi2 = Mathf.PI / 2;

	const float cameraOffset = 40f;

	static public LightingMainBuffer2D Get() {
		if (instance != null) {
			return(instance);
		}

		foreach(LightingMainBuffer2D mainBuffer in Object.FindObjectsOfType(typeof(LightingMainBuffer2D))) {
			instance = mainBuffer;
			return(instance);
		}

		GameObject setMainBuffer = new GameObject ();
		setMainBuffer.transform.parent = LightingManager2D.Get().transform;
		setMainBuffer.name = "Main Buffer";
		setMainBuffer.layer = LightingManager2D.lightingLayer;

		instance = setMainBuffer.AddComponent<LightingMainBuffer2D> ();
		instance.Initialize();
		
		return(instance);
	}

	 public static void ForceUpdate() {
		Get().gameObject.SetActive(false);
		Get().gameObject.SetActive(true);
	}

	public void Initialize() {
		SetUpRenderTexture ();
		SetUpRenderMaterial ();
		SetUpCamera ();
	}

	void SetUpRenderTexture() {
		screenWidth = Screen.width;
		screenHeight = Screen.height;

		renderTexture = new RenderTexture (screenWidth, screenHeight, 16, RenderTextureFormat.ARGB32);
		renderTexture.Create ();
	}

	void SetUpRenderMaterial() {
		material = new Material (Shader.Find (LightingManager2D.shaderPath + "Particles/Multiply"));
		material.mainTexture = renderTexture;
	}

	void SetUpCamera() {
		bufferCamera = gameObject.AddComponent<Camera> ();
		bufferCamera.clearFlags = CameraClearFlags.Color;
		bufferCamera.backgroundColor = darknessColor;
		bufferCamera.cameraType = CameraType.Game;
		bufferCamera.orthographic = true;
		bufferCamera.targetTexture = renderTexture;
		bufferCamera.farClipPlane = 1f;
		bufferCamera.nearClipPlane = 0f;
		bufferCamera.allowMSAA = false;
		bufferCamera.allowHDR = false;
	}

	void Update () {
		if (Screen.width != screenWidth || Screen.height != screenHeight) {
			SetUpRenderTexture();
			SetUpRenderMaterial();
			bufferCamera.targetTexture = renderTexture;
		}

		if (Camera.main == null) {
			Debug.LogWarning("SmartLighting2D: Main Camera is Missing");
			return;
		}

		bufferCamera.orthographicSize = Camera.main.orthographicSize;

		bufferCamera.backgroundColor = darknessColor;

		transform.position = new Vector3(0, 0, Camera.main.transform.position.z - cameraOffset);
		transform.rotation = Camera.main.transform.rotation;

		gameObject.SetActive (false);
		gameObject.SetActive (true);
	}

	public void OnRenderObject() {
		if (Camera.current != bufferCamera) {
			return;
		}

		if (Camera.main == null) {
			return;
		}

		LightingManager2D.LightingDebug.LightMainBufferUpdates +=1 ;

		float z = transform.position.z;

		Vector2D offset = new Vector2D(-Camera.main.transform.position);

		Max2D.Check();

		LightingSoftDayShadows.Draw(offset, z);

		foreach (LightingCollider2D id in LightingCollider2D.GetList()) {
			if (id.dayHeight == false) {
				continue;
			}
            Max2D.SetColor (Color.white);
            Max2D.iDrawMesh (id.GetMesh(), id.transform, offset, z);
        }

		float darkness = 1f - LightingManager2D.Get().shadowDarkness;

		LightingManager2D.Get().additiveMaterial.SetColor ("_TintColor", new Color(0.5f, 0.5f, 0.5f, darkness));

		float ratio = (float)Screen.width / Screen.height;

		Vector2 size = new Vector2(Camera.main.orthographicSize * ratio, Camera.main.orthographicSize);
		Max2D.DrawImage(LightingManager2D.Get().additiveMaterial, Vector2.zero, size, 0, z);
	
		DrawRooms(offset, z);

		DrawLightingBuffers(z);
		
		LightingOcclussion.Draw(offset, z);
	}
	
	// Room Mask
	void DrawRooms(Vector2D offset, float z) {
		foreach (LightingRoom2D id in LightingRoom2D.GetList()) {
			Max2D.SetColor (id.color);
			Max2D.iDrawMesh (id.GetMesh(), id.transform, offset, z);
		}
	}

	// Lighting Buffers
	void DrawLightingBuffers(float z) {
		foreach (LightingBuffer2D id in LightingBuffer2D.GetList()) {
			if (id.lightSource != null) {
				if (id.lightSource.isActiveAndEnabled == false) {
					continue;
				}

				LightingManager2D.LightingDebug.LightSourceDrawn +=1;

				Vector3 pos = id.lightSource.transform.position - Camera.main.transform.position;
				float size = id.bufferCamera.orthographicSize;

				Color lightColor = id.lightSource.lightColor;
				lightColor.a = id.lightSource.lightAlpha / 2;

				id.material.SetColor ("_TintColor", lightColor);
			
				Max2D.iDrawImage (id.material, new Vector2D (pos), new Vector2D (size, size), z);
			}
		}
	}
}

			//Max2D.SetColor (Color.white);
			//foreach(Polygon2D poly in id.collisions) {
			//	Mesh mesh = mesh = PolygonTriangulator2D.Triangulate (poly, Vector2.zero, Vector2.zero, PolygonTriangulator2D.Triangulation.Advanced);
            //	
			//	Max2D.iDrawMesh (mesh, id.transform, offset, z);
			//}



	/* 
	void DrawTile1(Vector2D off, float z) {
		Polygon2D poly = Polygon2DList.CreateFromRect(new Vector2(1, 1) / 2);
		poly = poly.ToOffset(new Vector2D(0.5f, 0.5f));

		List<Pair2D> iterate1 = Pair2D.GetList(poly.pointsList);
		List<Pair2D> iterate2 = Pair2D.GetList(PreparePolygon(poly, 1).pointsList);

		int i = 0;
		foreach (Pair2D pA in iterate1) {
			Pair2D pB = iterate2[i];
			GL.TexCoord2 (uv0, uv0);
			Max2D.Vertex3 (pA.A + off, z);
			GL.TexCoord2 (uv1, uv0);
			Max2D.Vertex3 (pA.B + off, z);
			GL.TexCoord2 (uv1, uv1);
			Max2D.Vertex3 (pB.B + off, z);
			GL.TexCoord2 (uv0, uv1);
			Max2D.Vertex3 (pB.A + off, z);
			i ++;
		}
	}*/

			/* 
		foreach (LightingTileMap id in LightingTileMap.GetList()) {
			if (id.map == null) {
				continue;
			}
			if (id.dayHeight == false) {
				continue;
			}
			for(int x = 0; x < id.area.size.x; x++) {
				for(int y = 0; y < id.area.size.y; y++) {
					if (id.map[x, y] == true) {
						Polygon2D poly = Polygon2D.CreateFromRect(new Vector2(0.5f, 0.5f));
						poly = poly.ToOffset(new Vector2D(x + 0.5f, y + 0.5f));
						//poly = poly.ToOffset(new Vector2D(id.area.position.x, id.area.position.y));
						Max2D.SetColor (Color.white);
            		//	Max2D.iDrawMesh (poly.CreateMesh(Vector2.zero, Vector2.zero), id.transform, offset, z);
					}	
				}
			}	
		}*/

		// Draw Brightness/Shadow Darkness Setting
		//LightingManager2D.Get().additiveMaterial.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
