using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class LightingCollider2D : MonoBehaviour {
	public bool dayHeight = false;
	public float height = 1f;

	public bool ambientOcclusion = false;
	public bool smoothOcclusionEdges = false;
	public float occlusionSize = 1f;

	//public bool lighten = false;
	//public bool darken = false;

	private Polygon2D polygon;
	private Mesh mesh;
	private float meshDistance = 0f;

	public bool moved = false;
	public Vector2 movedPosition = Vector2.zero;
	public float movedRotation = 0;

	public bool edgeCollider2D = false;

	public List<Polygon2D> collisions = new List<Polygon2D>();

	public static List<LightingCollider2D> list = new List<LightingCollider2D>();

	public void OnEnable() {
		list.Add(this);
	}

	public void OnDisable() {
		list.Remove(this);
	}

	static public List<LightingCollider2D> GetList() {
		List<LightingCollider2D> result = new List<LightingCollider2D>(list);
		return(result);
	}

	void Start() {
		edgeCollider2D = (GetComponent<EdgeCollider2D>() != null);
		
		mesh = GetMesh();
		
		// Optimization
		if (GetPolygon() != null) {
			foreach (Vector2D id in GetPolygon().pointsList) {
				meshDistance = Mathf.Max(meshDistance, Vector2.Distance(id.ToVector2(), Vector2.zero));
			}
		}
	}

	public void Update() {
		Vector2 position = transform.transform.position;
		if (movedPosition != position) {
			movedPosition = position;
			moved = true;
		} else {
			moved = false;

			float rotation = transform.transform.rotation.eulerAngles.z;
			if (movedRotation != rotation) {
				movedRotation = rotation;
				moved = true;
			} else {
				moved = false;
			}
		}

		if (moved) {
			foreach (LightingSource2D id in LightingSource2D.GetList()) {
				if (Vector2.Distance (id.transform.position, position) < meshDistance + id.lightSize) {
					id.update = true;
					LightingBuffer2D.GetBuffer (id.GetTextureSize(), id).lightSource = id;
				}
			}
		}
	}

	public Mesh GetMesh() {
		if (mesh == null) {
			if (GetPolygon() != null) {
				if (GetPolygon().pointsList.Count > 2) {
					mesh = PolygonTriangulator2D.Triangulate (GetPolygon(), Vector2.zero, Vector2.zero, PolygonTriangulator2D.Triangulation.Advanced);
				}
			}
		}
		return(mesh);
	}

	public Polygon2D GetPolygon() {
		if (polygon == null) {
			List<Polygon2D> polygons = Polygon2DList.CreateFromGameObject (gameObject);
			if (polygons.Count > 0) {
				polygon = polygons[0];
			} else {
				Debug.LogWarning("SmartLighting2D: LightingCollider2D object is missing Collider Component");
			}
		}
		return(polygon);
	}
}
