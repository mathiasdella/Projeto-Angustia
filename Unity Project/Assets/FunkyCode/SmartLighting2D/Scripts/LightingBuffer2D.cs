using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightingBuffer2D : MonoBehaviour {
	public RenderTexture renderTexture;
	public LightingSource2D lightSource;
	public int textureSize = 0;

	public Material material;

	public Camera bufferCamera;

	// Constants
	Vector2D zero = Vector2D.Zero();
	const float uv0 = 0;
	const float uv1 = 1;
	private float occlusionSize = 15;
	static Mesh tileMesh = null;

	public static List<LightingBuffer2D> list = new List<LightingBuffer2D>();

	public void OnEnable() {
		list.Add(this);
	}

	public void OnDisable() {
		list.Remove(this);
	}

	static public List<LightingBuffer2D> GetList() {
		List<LightingBuffer2D> result = new List<LightingBuffer2D>(list);
		return(result);
	}

	public static Mesh GetTileMesh() {
		if (tileMesh == null) {
			Polygon2D tilePoly = Polygon2D.CreateFromRect(new Vector2(0.5f + 0.01f, 0.5f + 0.01f));
			tileMesh = tilePoly.CreateMesh(Vector2.zero, Vector2.zero);	
		}
		return(tileMesh);
	}

	static public int GetCount() {
		return(GetList().Count);
	}

	void LateUpdate() {
		if (lightSource != null) { // Check if size > 0
			if (lightSource.lightSize > 0) {
				float cameraZ = -1000f;
				if (Camera.main != null) {
					cameraZ = Camera.main.transform.position.z - 10 - GetCount();
				}

				bufferCamera.transform.position = new Vector3(0, 0, cameraZ);
				bufferCamera.orthographicSize = lightSource.lightSize;
			}
		}
		transform.rotation = Quaternion.Euler(0, 0, 0);
	}

	public void OnRenderObject() {
		if(Camera.current != bufferCamera) {
			return;
		}

		LightingManager2D.LightingDebug.LightBufferUpdates ++;

		if (lightSource == null) {
			return;
		}

		if (lightSource.update == false) {
			bufferCamera.enabled = false;
		}
	
		LateUpdate ();

		lightSource.update = false;
		
		material.SetColor ("_TintColor", Color.white); //  lightSource.lightColor // Difference between buffer and light material?

		Max2D.Check();

		DrawShadows();

		DrawCollideMask();
	
		DrawLightTexture();
		
		//lightSource = null;
		//bufferCamera.enabled = false;
	}

	void DrawLightTexture() {
		float z = transform.position.z;
		Vector2 size = new Vector2 (bufferCamera.orthographicSize, bufferCamera.orthographicSize);

		if (lightSource.rotationEnabled) {
			Max2D.DrawImage(lightSource.GetMaterial (), Vector2.zero, size, lightSource.transform.rotation.eulerAngles.z, z);


					// Light Rotation!!!
		
			GL.PushMatrix();
			Max2D.SetColor (Color.black);
			GL.Begin(GL.TRIANGLES);
			
			Max2D.defaultMaterial.color = Color.black;
			Max2D.defaultMaterial.SetPass(0);

			float rotation = lightSource.transform.rotation.eulerAngles.z * Mathf.Deg2Rad + Mathf.PI / 4;
			float squaredSize = Mathf.Sqrt((size.x * size.x) + (size.y * size.y));
			
			Vector2 p0 = Vector2D.RotToVec((double)rotation).ToVector2() * squaredSize;
			Vector2 p1 = Vector2D.RotToVec((double)rotation + Mathf.PI / 2).ToVector2() * squaredSize;
			Vector2 p2 = Vector2D.RotToVec((double)rotation + Mathf.PI).ToVector2() * squaredSize;
			Vector2 p3 = Vector2D.RotToVec((double)rotation - Mathf.PI / 2).ToVector2() * squaredSize;

			Vector2 up0 = p1 + Vector2D.RotToVec((double)rotation + Mathf.PI / 4 + Mathf.PI / 2).ToVector2() * squaredSize;
			Vector2 up1 = p1 +  Vector2D.RotToVec((double)rotation + Mathf.PI / 4).ToVector2() * squaredSize;
			up1 += Vector2D.RotToVec((double)rotation + Mathf.PI / 4 + Mathf.PI / 2).ToVector2() * squaredSize;

			Vector2 up2 = p0 + Vector2D.RotToVec((double)rotation + Mathf.PI / 4).ToVector2() * squaredSize;
			up2 += Vector2D.RotToVec((double)rotation + Mathf.PI / 4 - Mathf.PI / 2).ToVector2() * squaredSize;

			Vector2 up3 = p0 + Vector2D.RotToVec((double)rotation + Mathf.PI / 4 - Mathf.PI / 2).ToVector2() * squaredSize;

			
			Vector2 down0 = p3 + Vector2D.RotToVec((double)rotation + Mathf.PI / 4 + Mathf.PI / 2 - Mathf.PI ).ToVector2() * squaredSize;
			Vector2 down1 = p3 + Vector2D.RotToVec((double)rotation + Mathf.PI / 4 - Mathf.PI).ToVector2() * squaredSize;
			down1 += Vector2D.RotToVec((double)rotation + Mathf.PI / 4 + Mathf.PI / 2 - Mathf.PI).ToVector2() * squaredSize;

			Vector2 down2 = p2 +  Vector2D.RotToVec((double)rotation + Mathf.PI / 4 - Mathf.PI).ToVector2() * squaredSize;
			down2 += Vector2D.RotToVec((double)rotation + Mathf.PI / 4 - Mathf.PI / 2 - Mathf.PI).ToVector2() * squaredSize;

			Vector2 down3 = p2 + Vector2D.RotToVec((double)rotation + Mathf.PI / 4 - Mathf.PI / 2 - Mathf.PI).ToVector2() * squaredSize;


			Vector2 left0 = p0 + Vector2D.RotToVec((double)rotation + Mathf.PI / 4 + Mathf.PI / 2 - Mathf.PI / 2).ToVector2() * squaredSize;
			Vector2 left1 = p0 + Vector2D.RotToVec((double)rotation + Mathf.PI / 4- Mathf.PI / 2).ToVector2() * squaredSize;
			left1 += Vector2D.RotToVec((double)rotation + Mathf.PI / 4 + Mathf.PI / 2- Mathf.PI / 2).ToVector2() * squaredSize;

			Vector2 left2 =  p3 + Vector2D.RotToVec((double)rotation + Mathf.PI / 4- Mathf.PI / 2).ToVector2() * squaredSize;
			left2 += Vector2D.RotToVec((double)rotation + Mathf.PI / 4 - Mathf.PI / 2- Mathf.PI / 2).ToVector2() * squaredSize;
			
			Vector2 left3 = p3 + Vector2D.RotToVec((double)rotation + Mathf.PI / 4 - Mathf.PI / 2- Mathf.PI / 2).ToVector2() * squaredSize;


			Vector2 right0 = p2 + Vector2D.RotToVec((double)rotation + Mathf.PI / 4 + Mathf.PI / 2 + Mathf.PI / 2).ToVector2() * squaredSize;
			Vector2 right1 = p2 + Vector2D.RotToVec((double)rotation + Mathf.PI / 4+ Mathf.PI / 2).ToVector2() * squaredSize;
			left1 += Vector2D.RotToVec((double)rotation + Mathf.PI / 4 + Mathf.PI / 2+ Mathf.PI / 2).ToVector2() * squaredSize;

			Vector2 right2 =  p1 + Vector2D.RotToVec((double)rotation + Mathf.PI / 4+ Mathf.PI / 2).ToVector2() * squaredSize;
			left2 += Vector2D.RotToVec((double)rotation + Mathf.PI / 4 - Mathf.PI / 2+ Mathf.PI / 2).ToVector2() * squaredSize;
			
			Vector2 right3 = p1 + Vector2D.RotToVec((double)rotation + Mathf.PI / 4 - Mathf.PI / 2+ Mathf.PI / 2).ToVector2() * squaredSize;

			Max2DMatrix.DrawTriangle(right0, right1, right2, Vector2.zero, z);
			Max2DMatrix.DrawTriangle(right2, right3, right0, Vector2.zero, z);

			Max2DMatrix.DrawTriangle(left0, left1, left2, Vector2.zero, z);
			Max2DMatrix.DrawTriangle(left2, left3, left0, Vector2.zero, z);

		
			Max2DMatrix.DrawTriangle(down0, down1, down2, Vector2.zero, z);
			Max2DMatrix.DrawTriangle(down2, down3, down0, Vector2.zero, z);

			
			Max2DMatrix.DrawTriangle(up0, up1, up2, Vector2.zero, z);
			Max2DMatrix.DrawTriangle(up2, up3, up0, Vector2.zero, z);

			GL.End();
			GL.PopMatrix();

			Max2D.defaultMaterial.color = Color.white;
		} else {
			Max2D.DrawImage(lightSource.GetMaterial (), Vector2.zero, size, 0, z);
		}


	}

	 public static Vector3 GetPitchYawRollRad(Quaternion rotation)
 {
      float roll = Mathf.Atan2(2*rotation.y*rotation.w - 2*rotation.x*rotation.z, 1 - 2*rotation.y*rotation.y - 2*rotation.z*rotation.z);
      float pitch = Mathf.Atan2(2*rotation.x*rotation.w - 2*rotation.y*rotation.z, 1 - 2*rotation.x*rotation.x - 2*rotation.z*rotation.z);
      float yaw = Mathf.Asin(2*rotation.x*rotation.y + 2*rotation.z*rotation.w);
                 
      return new Vector3(pitch, roll, yaw);
 }
             
	public static Vector3 GetPitchYawRollDeg(Quaternion rotation)
	{
		Vector3 radResult = GetPitchYawRollRad(rotation);
		return new Vector3(radResult.x * Mathf.Rad2Deg, radResult.y * Mathf.Rad2Deg, radResult.z * Mathf.Rad2Deg);
	}


	void DrawCollideMask() {
		float z = transform.position.z;

		Vector2D offset = new Vector2D (-lightSource.transform.position);

		Material material = LightingManager2D.Get().whiteSpriteMaterial;
		// For Collider Sprite Mask
		foreach (LightingCollider2D id in LightingCollider2D.GetList()) {
			Sprite sprite = id.lightSprite;

			if (sprite == null || id.spriteRenderer == null) {
				continue;
			}

			if (id.maskType != LightingCollider2D.MaskType.Sprite) {
				continue;
			}

			material.mainTexture = sprite.texture;

			Vector2 p = id.transform.position;
			Vector2 scale = id.transform.lossyScale;

			scale.x *= (float)sprite.texture.width / sprite.texture.height;

			if (id.spriteRenderer.flipX) {
				scale.x = -scale.x;
			}

			if (id.spriteRenderer.flipY) {
				scale.y = -scale.y;
			}

			Max2D.DrawImage(material, offset.ToVector2() + p, scale, id.transform.rotation.eulerAngles.z, z);
		}

		GL.PushMatrix();
        Max2D.defaultMaterial.SetPass(0);
        GL.Begin(GL.TRIANGLES);
        GL.Color(Color.white);

		// For Collider Mask
		foreach (LightingCollider2D id in LightingCollider2D.GetList()) {
			Mesh mesh = id.GetMesh();

			if (mesh == null) {
				continue;
			}

			if (id.maskType != LightingCollider2D.MaskType.Collider) {
				continue;
			}

			for (int i = 0; i <  mesh.triangles.GetLength (0); i = i + 3) {
				Vector2 a = id.transform.TransformPoint(mesh.vertices [mesh.triangles [i]]);
				Vector2 b = id.transform.TransformPoint(mesh.vertices [mesh.triangles [i + 1]]);
				Vector2 c = id.transform.TransformPoint(mesh.vertices [mesh.triangles [i + 2]]);
				Max2DMatrix.DrawTriangle(a, b, c, offset.ToVector2(), z);
			}
		}

		Mesh tileMesh = GetTileMesh();	
		
		foreach (LightingTilemapCollider2D id in LightingTilemapCollider2D.GetList()) {
			if (id.map == null) {
				continue;
			}

			Vector3 rot = GetPitchYawRollRad(id.transform.rotation);

			float rotationYScale = Mathf.Sin(rot.x + Mathf.PI / 2);
			float rotationXScale = Mathf.Sin(rot.y + Mathf.PI / 2);

			float scaleX = id.transform.lossyScale.x * rotationXScale;
			float scaleY = id.transform.lossyScale.y * rotationYScale;

			for(int x = 0; x < id.area.size.x; x++) {
				for(int y = 0; y < id.area.size.y; y++) {
					if (id.map[x, y] == false) {
						continue;
					}

					Vector2D polyOffset = Vector2D.Zero();
					polyOffset += new Vector2D(x + 0.5f, y + 0.5f); 
					polyOffset += new Vector2D(id.area.position.x, id.area.position.y);
					polyOffset += new Vector2D(id.transform.position.x, id.transform.position.y);
					
					if (id.mapType == LightingTilemapCollider2D.MapType.SuperTilemapEditor) {
						polyOffset += new Vector2D(-id.area.size.x / 2, -id.area.size.y / 2);
					}

					polyOffset.x *= scaleX;
					polyOffset.y *= scaleY;

					polyOffset += offset;

					for (int i = 0; i < tileMesh.triangles.GetLength (0); i = i + 3) {
						Vector2 a = tileMesh.vertices [tileMesh.triangles [i]];
						Vector2 b = tileMesh.vertices [tileMesh.triangles [i + 1]];
						Vector2 c = tileMesh.vertices [tileMesh.triangles [i + 2]];
						Max2DMatrix.DrawTriangle(a, b, c, polyOffset.ToVector2(), z, new Vector2D(scaleX, scaleY));
					}				
				}
			}	
		}

		GL.End();
		GL.PopMatrix();
	}

	void DrawShadows() {
		float z = transform.position.z;

		// Shadow Fill
		GL.PushMatrix();
		Max2D.defaultMaterial.SetPass(0);

		GL.Begin(GL.TRIANGLES);
		GL.Color(Color.black);

		foreach (LightingCollider2D id in LightingCollider2D.GetList()) {
			List<Polygon2D> polygons = id.GetPolygons();
			if (polygons.Count < 1) {
				continue;
			}

			foreach(Polygon2D polygon in polygons) {
				Polygon2D poly = polygon;
				poly = poly.ToWorldSpace (id.gameObject.transform);
				poly = poly.ToOffset (new Vector2D (-lightSource.transform.position));

				if (poly.PointInPoly (zero)) {
					continue;
				}

				foreach (Pair2D p in Pair2D.GetList(poly.pointsList)) {
					Vector2D vA = p.A.Copy();
					Vector2D vB = p.B.Copy();
					
					vA.Push (Vector2D.Atan2 (vA, zero), 25);
					vB.Push (Vector2D.Atan2 (vB, zero), 25);
					
					Max2DMatrix.DrawTriangle(p.A ,p.B, vA, zero, z);
					Max2DMatrix.DrawTriangle(vA, vB, p.B, zero, z);
				}
			}
		}

		foreach (LightingTilemapCollider2D id in LightingTilemapCollider2D.GetList()) {
			if (id.map == null) {
				continue;
			}

			Vector3 rot = GetPitchYawRollRad(id.transform.rotation);

			float rotationYScale = Mathf.Sin(rot.x + Mathf.PI / 2);
			float rotationXScale = Mathf.Sin(rot.y + Mathf.PI / 2);

			float scaleX = id.transform.lossyScale.x * rotationXScale;
			float scaleY = id.transform.lossyScale.y * rotationYScale;

			for(int x = 0; x < id.area.size.x; x++) {
				for(int y = 0; y < id.area.size.y; y++) {
					if (id.map[x, y] == false) {
						continue;
					}

					Polygon2D poly = Polygon2D.CreateFromRect(new Vector2(0.5f * scaleX, 0.5f * scaleY));

					Vector2D polyOffset = Vector2D.Zero();
					polyOffset += new Vector2D(x + 0.5f, y + 0.5f);
					polyOffset += new Vector2D(id.area.position.x, id.area.position.y);
					polyOffset += new Vector2D(id.transform.position.x, id.transform.position.y);

					if (id.mapType == LightingTilemapCollider2D.MapType.SuperTilemapEditor) {
						polyOffset += new Vector2D(-id.area.size.x / 2, -id.area.size.y / 2);
					}

					polyOffset.x *= scaleX;
					polyOffset.y *= scaleY;

					polyOffset += new Vector2D (-lightSource.transform.position);

					
					poly = poly.ToOffset(polyOffset);

					if (poly.PointInPoly (zero)) {
						continue;
					}

					foreach (Pair2D p in Pair2D.GetList(poly.pointsList)) {
						Vector2D vA = p.A.Copy();
						Vector2D vB = p.B.Copy();
						
						vA.Push (Vector2D.Atan2 (vA, zero), 25);
						vB.Push (Vector2D.Atan2 (vB, zero), 25);
						
						Max2DMatrix.DrawTriangle(p.A ,p.B, vA, zero, z);
						Max2DMatrix.DrawTriangle(vA, vB, p.B, zero, z);
					}	
				}
			}	
		}

		GL.End();

		// Penumbra

		LightingManager2D.Get().penumbraMaterial.SetPass(0);

		GL.Begin(GL.TRIANGLES);
		GL.Color(Color.white);

		foreach (LightingCollider2D id in LightingCollider2D.GetList()) {
			List<Polygon2D> polygons = id.GetPolygons();
			if (polygons.Count < 1) {
				continue;
			}

			foreach(Polygon2D polygon in polygons) {
				Polygon2D poly = polygon;
			
				poly = poly.ToWorldSpace (id.gameObject.transform);
				poly = poly.ToOffset (new Vector2D (-lightSource.transform.position));
				
				if (poly.PointInPoly (zero)) {
					continue;
				}

				foreach (Pair2D p in Pair2D.GetList(poly.pointsList)) {
					Vector2D vA = p.A.Copy();
					Vector2D pA = p.A.Copy();

					Vector2D vB = p.B.Copy();
					Vector2D pB = p.B.Copy();

					float angleA = (float)Vector2D.Atan2 (vA, zero);
					float angleB = (float)Vector2D.Atan2 (vB, zero);

					vA.Push (angleA, lightSource.lightSize);
					pA.Push (angleA - Mathf.Deg2Rad * occlusionSize, lightSource.lightSize);

					vB.Push (angleB, lightSource.lightSize);
					pB.Push (angleB + Mathf.Deg2Rad * occlusionSize, lightSource.lightSize);

					GL.TexCoord2(uv0, uv0);
					GL.Vertex3((float)p.A.x,(float)p.A.y, z);
					GL.TexCoord2(uv1, uv0);
					GL.Vertex3((float)vA.x, (float)vA.y, z);
					GL.TexCoord2((float)uv0, uv1);
					GL.Vertex3((float)pA.x,(float)pA.y, z);

					GL.TexCoord2(uv0, uv0);
					GL.Vertex3((float)p.B.x,(float)p.B.y, z);
					GL.TexCoord2(uv1, uv0);
					GL.Vertex3((float)vB.x, (float)vB.y, z);
					GL.TexCoord2(uv0, uv1);
					GL.Vertex3((float)pB.x, (float)pB.y, z);
				}
			}
		}
		
		foreach (LightingTilemapCollider2D id in LightingTilemapCollider2D.GetList()) {
			if (id.map == null) {
				continue;
			}
			//Debug.Log(Mathf.Cos(id.transform.rotation.eulerAngles.x * Mathf.Deg2Rad));
			Vector3 rot = GetPitchYawRollRad(id.transform.rotation);

			float rotationYScale = Mathf.Sin(rot.x + Mathf.PI / 2);
			float rotationXScale = Mathf.Sin(rot.y + Mathf.PI / 2);

			float scaleX = id.transform.lossyScale.x * rotationXScale;
			float scaleY = id.transform.lossyScale.y * rotationYScale;

			for(int x = 0; x < id.area.size.x; x++) {
				for(int y = 0; y < id.area.size.y; y++) {
					if (id.map[x, y] == false) {
						continue;
					}

					Polygon2D poly = Polygon2D.CreateFromRect(new Vector2(0.5f * scaleX, 0.5f * scaleY));
					
					Vector2D polyOffset = Vector2D.Zero();
					polyOffset += new Vector2D(x + 0.5f, y + 0.5f);
					polyOffset += new Vector2D(id.area.position.x, id.area.position.y);
					polyOffset += new Vector2D(id.transform.position.x, id.transform.position.y);

					if (id.mapType == LightingTilemapCollider2D.MapType.SuperTilemapEditor) {
						polyOffset += new Vector2D(-id.area.size.x / 2, -id.area.size.y / 2);
					}

					polyOffset.x *= scaleX;
					polyOffset.y *= scaleY;

					polyOffset += new Vector2D (-lightSource.transform.position);

					poly = poly.ToOffset(polyOffset);

					if (poly.PointInPoly (zero)) {
						continue;
					}

					foreach (Pair2D p in Pair2D.GetList(poly.pointsList)) {
						Vector2D vA = p.A.Copy();
						Vector2D pA = p.A.Copy();

						Vector2D vB = p.B.Copy();
						Vector2D pB = p.B.Copy();

						float angleA = (float)Vector2D.Atan2 (vA, zero);
						float angleB = (float)Vector2D.Atan2 (vB, zero);

						vA.Push (angleA, lightSource.lightSize);
						pA.Push (angleA - Mathf.Deg2Rad * occlusionSize, lightSource.lightSize);

						vB.Push (angleB, lightSource.lightSize);
						pB.Push (angleB + Mathf.Deg2Rad * occlusionSize, lightSource.lightSize);

						GL.TexCoord2(uv0, uv0);
						GL.Vertex3((float)p.A.x,(float) p.A.y, z);
						GL.TexCoord2(uv1, uv0);
						GL.Vertex3((float)vA.x, (float)vA.y, z);
						GL.TexCoord2((float)uv0, uv1);
						GL.Vertex3((float)pA.x,(float) pA.y, z);

						GL.TexCoord2(uv0, uv0);
						GL.Vertex3((float)p.B.x,(float) p.B.y, z);
						GL.TexCoord2(uv1, uv0);
						GL.Vertex3((float)vB.x, (float)vB.y, z);
						GL.TexCoord2(uv0, uv1);
						GL.Vertex3((float)pB.x, (float)pB.y, z);
					}
				}
			}	
		}

		GL.End();
		GL.PopMatrix();
	}

	static public LightingBuffer2D AddBuffer(int textureSize) {
		GameObject buffer = new GameObject ();
		buffer.name = "Buffer " + GetCount();
		buffer.transform.parent = LightingManager2D.Get().mainBuffer.transform;
		buffer.layer = LightingManager2D.lightingLayer;

		LightingBuffer2D lightingBuffer = buffer.AddComponent<LightingBuffer2D> ();
		lightingBuffer.Initiate (textureSize);

		return(lightingBuffer);
	}

	static public LightingBuffer2D GetBuffer(int textureSize, LightingSource2D lightSource) {
		foreach (LightingBuffer2D id in LightingBuffer2D.GetList()) {
			if ((id.lightSource == lightSource || id.lightSource == null) && id.textureSize == textureSize) {
				id.lightSource = lightSource;
				lightSource.update = true;
				id.bufferCamera.enabled = true;
				//id.gameObject.SetActive (true);
				return(id);
			}
		}
			
		return(AddBuffer(textureSize));		
	}

	public void Initiate (int textureSize) {
		SetUpRenderTexture (textureSize);
		SetUpRenderMaterial();
		SetUpCamera ();
	}

	void SetUpRenderTexture(int _textureSize) {
		textureSize = _textureSize;

		renderTexture = new RenderTexture(textureSize, textureSize, 16, RenderTextureFormat.ARGB32);

		name = "Buffer " + GetCount() + " (size: " + textureSize + ")";
	}

	void SetUpRenderMaterial() {
		material = new Material (Shader.Find (Max2D.shaderPath + "Particles/Additive"));
		material.mainTexture = renderTexture;
	}

	void SetUpCamera() {
		bufferCamera = gameObject.AddComponent<Camera> ();
		bufferCamera.clearFlags = CameraClearFlags.Color;
		bufferCamera.backgroundColor = Color.white;
		bufferCamera.cameraType = CameraType.Game;
		bufferCamera.orthographic = true;
		bufferCamera.targetTexture = renderTexture;
		bufferCamera.farClipPlane = 0.5f;
		bufferCamera.nearClipPlane = 0f;
		bufferCamera.allowHDR = false;
		bufferCamera.allowMSAA = false;
		bufferCamera.enabled = false;
	}
}