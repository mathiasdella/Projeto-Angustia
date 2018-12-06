using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightingSpriteRenderer : MonoBehaviour
{
    public Sprite sprite;
    public Color color = new Color(0.5f, 0.5f, 0.5f, 0.25f);
    public bool flipX = false;
    public bool flipY = false;

	public static List<LightingSpriteRenderer> list = new List<LightingSpriteRenderer>();

	public void OnEnable() {
		list.Add(this);
	}

	public void OnDisable() {
		list.Remove(this);
	}

    static public List<LightingSpriteRenderer> GetList() {
		List<LightingSpriteRenderer> result = new List<LightingSpriteRenderer>(list);
		return(result);
	}

}
