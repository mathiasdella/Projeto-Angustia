using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingSoftDayShadows {
	const float uv0 = 0;
	const float uv1 = 1;
	const float pi2 = Mathf.PI / 2;
    
    public static void Draw(Vector2D offset, float z) {
		float sunDirection = LightingManager2D.GetSunDirection ();
		
		// Day Soft Shadows
		GL.PushMatrix();
		Max2D.defaultMaterial.SetPass(0);
		GL.Begin(GL.TRIANGLES);
		GL.Color(Color.black);

		foreach (LightingCollider2D id in LightingCollider2D.GetList()) {
			if (id.dayHeight == false || id.height <= 0) {
				continue;
			}
			List<Polygon2D> polygons = id.GetPolygons();

			foreach(Polygon2D polygon in polygons) {
				Polygon2D poly = polygon;
				poly = poly.ToWorldSpace (id.gameObject.transform);
				
				foreach (Pair2D p in Pair2D.GetList(poly.pointsList)) {
					Vector2D vA = p.A.Copy();
					Vector2D vB = p.B.Copy();

					vA.Push (sunDirection, id.height);
					vB.Push (sunDirection, id.height);

					Max2DMatrix.DrawTriangle(p.A, p.B, vA, offset, z);
					Max2DMatrix.DrawTriangle(vA, vB, p.B, offset, z);
				}
			}
		}

		GL.End();
		GL.PopMatrix();
		
		GL.PushMatrix ();
		LightingManager2D.Get().shadowBlurMaterial.SetPass (0);
		GL.Begin (GL.TRIANGLES);
		Max2D.SetColor (Color.white);

		foreach (LightingCollider2D id in LightingCollider2D.GetList()) {
			if (id.dayHeight == false || id.height <= 0) {
				continue;
			}
			List<Polygon2D> polygons = id.GetPolygons();

			foreach(Polygon2D polygon in polygons) {
				Polygon2D poly = polygon;
				poly = poly.ToWorldSpace (id.gameObject.transform);
				
				Polygon2D convexHull = Polygon2D.GenerateShadow(new Polygon2D(poly.pointsList), sunDirection, id.height);
				
				foreach (DoublePair2D p in DoublePair2D.GetList(convexHull.pointsList)) {
					Vector2D zA = new Vector2D (p.A + offset);
					Vector2D zB = new Vector2D (p.B + offset);
					Vector2D zC = zB.Copy();

					Vector2D pA = zA.Copy();
					Vector2D pB = zB.Copy();

					zA.Push (Vector2D.Atan2 (p.A, p.B) + pi2, .5f);
					zB.Push (Vector2D.Atan2 (p.A, p.B) + pi2, .5f);
					zC.Push (Vector2D.Atan2 (p.B, p.C) + pi2, .5f);
					
					GL.TexCoord2 (uv0, uv0);
					Max2D.Vertex3 (pB, z);
					GL.TexCoord2 (0.5f - uv0, uv0);
					Max2D.Vertex3 (pA, z);
					GL.TexCoord2 (0.5f - uv0, uv1);
					Max2D.Vertex3 (zA, z);
				
					GL.TexCoord2 (uv0, uv1);
					Max2D.Vertex3 (zA, z);
					GL.TexCoord2 (0.5f - uv0, uv1);
					Max2D.Vertex3 (zB, z);
					GL.TexCoord2 (0.5f - uv0, uv0);
					Max2D.Vertex3 (pB, z);
					
					GL.TexCoord2 (uv0, uv1);
					Max2D.Vertex3 (zB, z);
					GL.TexCoord2 (0.5f - uv0, uv0);
					Max2D.Vertex3 (pB, z);
					GL.TexCoord2 (0.5f - uv0, uv1);
					Max2D.Vertex3 (zC, z);
				}
			}
		}

		GL.End();
		GL.PopMatrix();

		GL.PushMatrix();
		Max2D.defaultMaterial.SetPass(0);
		GL.Begin(GL.TRIANGLES);
		GL.Color(Color.black);

		// Day Soft Shadows
		foreach (LightingTilemapCollider2D id in LightingTilemapCollider2D.GetList()) {
			if (id.map == null) {
				continue;
			}
			if (id.dayHeight == false) {
				continue;
			}
			for(int x = 0; x < id.area.size.x; x++) {
				for(int y = 0; y < id.area.size.y; y++) {
					if (id.map[x, y] == true) {
						DrawSoftShadowTile(offset + new Vector2D(x, y), z, id.height);
					}	
				}
			}	
		}

		GL.End();
		GL.PopMatrix();

		GL.PushMatrix ();
		LightingManager2D.Get().shadowBlurMaterial.SetPass (0);
		GL.Begin (GL.TRIANGLES);
		Max2D.SetColor (Color.white);
		
		// Day Soft Shadows
		foreach (LightingTilemapCollider2D id in LightingTilemapCollider2D.GetList()) {
			if (id.map == null) {
				continue;
			}
			if (id.dayHeight == false) {
				continue;
			}
			for(int x = 0; x < id.area.size.x; x++) {
				for(int y = 0; y < id.area.size.y; y++) {
					if (id.map[x, y] == true) {
						DrawSoftShadowTileBlur(offset + new Vector2D(x, y), z, id.height);
					}	
				}
			}	
		}
	
		GL.End();
		GL.PopMatrix();

		Material material = LightingManager2D.Get().whiteSpriteMaterial;
		foreach (LightingSprite2D id in LightingSprite2D.GetList()) {
			if (id.GetSpriteRenderer() == null) {
				continue;
			}
			material.mainTexture = id.GetSpriteRenderer().sprite.texture; //Debug.Log(sprite.pivot);

			Vector2 p = id.transform.position;
			Vector2 scale = id.transform.lossyScale;

			if (id.GetSpriteRenderer().flipX) {
				scale.x = -scale.x;
			}

			if (id.GetSpriteRenderer().flipY) {
				scale.y = -scale.y;
			}

			Max2D.DrawImage(material, offset.ToVector2() + p, scale, id.transform.rotation.eulerAngles.z, z);
		}

		material = LightingManager2D.Get().additiveMaterial;
		foreach (LightingSpriteRenderer2D id in LightingSpriteRenderer2D.GetList()) {
			if (id.sprite == null) {
				continue;
			}
			material.mainTexture = id.sprite.texture; //Debug.Log(sprite.pivot);

			Vector2 position = id.transform.position;
			Vector2 scale = id.transform.lossyScale;

			float scaleX = (float) id.sprite.texture.width / (id.sprite.pixelsPerUnit * 2);
			float scaleY = (float) id.sprite.texture.width / (id.sprite.pixelsPerUnit * 2);
			//Debug.Log(scaleX + " " + scaleY);

			scale.x *= scaleX;
			scale.y *= scaleY;

			scale.x *= id.scale.x;
			scale.y *= id.scale.y;

			if (id.flipX) {
				scale.x = -scale.x;
			}

			if (id.flipY) {
				scale.y = -scale.y;
			}

			//material.color = id.color;
			Color color = id.color;
			color.a = id.alpha;

			material.SetColor ("_TintColor", color);
			Max2D.DrawImage(material, offset.ToVector2() + position + id.offset, scale, id.transform.rotation.eulerAngles.z, z);
		}

		material.mainTexture = null;

		float ratio = (float)Screen.width / Screen.height;
        Camera bufferCamera = LightingMainBuffer2D.Get().bufferCamera;
		Vector2 size = new Vector2(bufferCamera.orthographicSize * ratio, bufferCamera.orthographicSize);
		Vector3 pos = Camera.main.transform.position;

		Max2D.iDrawImage(LightingManager2D.Get().additiveMaterial, new Vector2D(pos), new Vector2D(size), pos.z);
	}

	static void DrawSoftShadowTile(Vector2D offset, float z, float height) {
		float sunDirection = LightingManager2D.GetSunDirection ();

		Polygon2D poly = Polygon2DList.CreateFromRect(new Vector2(1, 1) / 2);
		poly = poly.ToOffset(new Vector2D(0.5f, 0.5f));

		foreach (Pair2D p in Pair2D.GetList(poly.pointsList)) {
			Vector2D vA = p.A.Copy();
			Vector2D vB = p.B.Copy();

			vA.Push (sunDirection, height);
			vB.Push (sunDirection, height);

			Max2DMatrix.DrawTriangle(p.A, p.B, vA, offset, z);
			Max2DMatrix.DrawTriangle(vA, vB, p.B, offset, z);
		}
	}

	static void DrawSoftShadowTileBlur(Vector2D offset, float z, float height) {
		float sunDirection = LightingManager2D.GetSunDirection ();

		Polygon2D poly = Polygon2DList.CreateFromRect(new Vector2(1, 1) / 2);
		offset += new Vector2D(0.5f, 0.5f);
		
		Polygon2D convexHull = Polygon2D.GenerateShadow(new Polygon2D(poly.pointsList), sunDirection, height);
		
		foreach (DoublePair2D p in DoublePair2D.GetList(convexHull.pointsList)) {
			Vector2D zA = new Vector2D (p.A + offset);
			Vector2D zB = new Vector2D (p.B + offset);
			Vector2D zC = zB.Copy();

			Vector2D pA = zA.Copy();
			Vector2D pB = zB.Copy();

			zA.Push (Vector2D.Atan2 (p.A, p.B) + pi2, .5f);
			zB.Push (Vector2D.Atan2 (p.A, p.B) + pi2, .5f);
			zC.Push (Vector2D.Atan2 (p.B, p.C) + pi2, .5f);
			
			GL.TexCoord2 (uv0, uv0);
			Max2D.Vertex3 (pB, z);
			GL.TexCoord2 (0.5f - uv0, uv0);
			Max2D.Vertex3 (pA, z);
			GL.TexCoord2 (0.5f - uv0, uv1);
			Max2D.Vertex3 (zA, z);
		
			GL.TexCoord2 (uv0, uv1);
			Max2D.Vertex3 (zA, z);
			GL.TexCoord2 (0.5f - uv0, uv1);
			Max2D.Vertex3 (zB, z);
			GL.TexCoord2 (0.5f - uv0, uv0);
			Max2D.Vertex3 (pB, z);
			
			GL.TexCoord2 (uv0, uv1);
			Max2D.Vertex3 (zB, z);
			GL.TexCoord2 (0.5f - uv0, uv0);
			Max2D.Vertex3 (pB, z);
			GL.TexCoord2 (0.5f - uv0, uv1);
			Max2D.Vertex3 (zC, z);
		}
	}
}
