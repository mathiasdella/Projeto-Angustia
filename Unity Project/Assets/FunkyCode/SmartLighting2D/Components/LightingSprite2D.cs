using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingSprite2D : MonoBehaviour {
	private SpriteRenderer spriteRenderer;

	public static List<LightingSprite2D> list = new List<LightingSprite2D>();

	public void OnEnable() {
		list.Add(this);
	}

	public void OnDisable() {
		list.Remove(this);
	}
	
	static public List<LightingSprite2D> GetList() {
		List<LightingSprite2D> result = new List<LightingSprite2D>(list);

		return(result);
	}

	public SpriteRenderer GetSpriteRenderer() {
		if (spriteRenderer == null) {
			spriteRenderer = GetComponent<SpriteRenderer>();
			if (spriteRenderer == null) {
				Debug.LogWarning("SmartLighting2D: LightingSprite2D object is missing SpriteRenderer Component");
			}
		}
		return(spriteRenderer);
	}
}
