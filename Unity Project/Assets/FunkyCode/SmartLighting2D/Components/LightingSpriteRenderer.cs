using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightingSpriteRenderer2D : MonoBehaviour
{
    public Sprite sprite;
    public Color color = new Color(0.5f, 0.5f, 0.5f, 1f);

	[Range(0, 1)]
	public float alpha = 1f;
    public bool flipX = false;
    public bool flipY = false;

	public Vector2 scale = new Vector2(1, 1);
	public Vector2 offset = new Vector2(0, 0);

	public static List<LightingSpriteRenderer2D> list = new List<LightingSpriteRenderer2D>();

	public void OnEnable() {
		list.Add(this);

		color.a = 1f;
	}

	public void OnDisable() {
		list.Remove(this);
	}

    static public List<LightingSpriteRenderer2D> GetList() {
		List<LightingSpriteRenderer2D> result = new List<LightingSpriteRenderer2D>(list);
		return(result);
	}

}
