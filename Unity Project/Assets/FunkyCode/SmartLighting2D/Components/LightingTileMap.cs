using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class LightingTileMap : MonoBehaviour {
	public enum MapType {Default, Super};
	public MapType mapType = MapType.Default;
	public bool dayHeight = false;
	public float height = 1;
	
	public bool ambientOcclusion = false;
	public float occlusionSize = 1f;

	Tilemap tilemap2D;
	public BoundsInt area;

	public bool[,] map;

	public static List<LightingTileMap> list = new List<LightingTileMap>();

	public void OnEnable() {
		list.Add(this);
	}

	public void OnDisable() {
		list.Remove(this);
	}

	static public List<LightingTileMap> GetList() {
		List<LightingTileMap> result = new List<LightingTileMap>(list);
		return(result);
	}

	void Start () {
		switch(mapType) {
			case MapType.Default:
				tilemap2D = GetComponent<Tilemap>();

				TileBase[] tileArray = tilemap2D.GetTilesBlock(area);

				map = new bool[area.size.x, area.size.y];
				
				for (int index = 0; index < tileArray.Length; index++) {
					if (tileArray[index] == null) {
						continue;
					}
					map[(index % area.size.x), (index / area.size.y)] = true;
				}

				// print((index % area.size.x) + ", " + (index / area.size.y)+  " " + tileArray[index]);

			break;

			case MapType.Super:
		
				
			break;
		}


	}
}

	

	/* 
		PolygonCollider2D[] colliders = FindObjectsOfType<PolygonCollider2D>();
		Debug.Log(colliders.Length);

		//polygon = new Polygon2D();

		foreach(PolygonCollider2D collider in colliders) {
			List<Polygon2D> polygons = Polygon2DList.CreateFromGameObject (collider.gameObject);

			//string name = collider.name;
			//string[] split = name.Split("_"[0]);
			//int scaleX = int.Parse(split[0]);
			//int scaleY = int.Parse(split[1]);

			//if (scaleX == 0) {
			//	scaleX = 1;
			//}

		
		//	if (scaleY == 0) {
			//	scaleY = 1;
		//	}

			//Debug.Log(scaleX + "__" + scaleY);
			Polygon2D poly = polygons[0].ToWorldSpace(collider.transform);
			//poly = poly.ToScale(new Vector2(scaleX, scaleY));
			//poly.pointsList.Remove(poly.pointsList.Last());
			poly.Normalize();
			collisions.Add(poly);
			//polygon.AddPoints(poly.pointsList);

			Debug.Log(poly.pointsList.Count +  " " + collider.pathCount);

			foreach(Vector2D v in poly.pointsList) { 
				Debug.Log(v.ToVector2());
			}

			
			
		}

		meshDistance = 10100f; 
 */
