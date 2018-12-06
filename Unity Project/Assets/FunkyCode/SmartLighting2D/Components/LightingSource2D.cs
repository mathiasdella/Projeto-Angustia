using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightingSource2D : MonoBehaviour {
	public enum TextureSize {px1024, px512, px256, px128};
	public enum LightSprite {Default, Custom};

	public Color lightColor = new Color(.5f,.5f, .5f, 1);
	public float lightAlpha = 1f;
	public float lightSize = 5f;
	public TextureSize textureSize = TextureSize.px1024;
	public bool rotationEnabled = false;

	public LightSprite lightSprite = LightSprite.Default;

	public Sprite sprite;
	private Material material;

	private Vector3 updatePosition = Vector3.zero;
	private Color updateColor = Color.white;
	private float updateRotation = 0;
	private float updateSize = 0;
	private float updateAlpha = 0.5f;
	
	public bool update = true;

	public static List<LightingSource2D> list = new List<LightingSource2D>();

	public void OnEnable() {
		list.Add(this);
	}

	public void OnDisable() {
		list.Remove(this);
	}

	static public List<LightingSource2D> GetList() {
		List<LightingSource2D> result = new List<LightingSource2D>(list);
		return(result);
	}

	public int GetTextureSize() {
		switch(textureSize) {
			case TextureSize.px1024:
				return(1024);

			case TextureSize.px512:
				return(512);

			case TextureSize.px256:
				return(256);
			
			default:
				return(128);
		}
	}

	void Start () {
		SetMaterial ();
	}

	public void SetMaterial() {
		material = new Material (Shader.Find (LightingManager2D.shaderPath + "Particles/Multiply"));
		material.mainTexture = GetSprite().texture;

		update = true;
	}

	public Sprite GetSprite() {
		if (sprite == null) {
			sprite = Resources.Load <Sprite> ("Sprites/gfx_light") ;
		}
		return(sprite);
	}
		
	public Material GetMaterial() {
		return(material);
	}

	void Update() {
		if (updatePosition != transform.position) {
			updatePosition = transform.position;

			update = true;
		}

		if (updateRotation != transform.rotation.eulerAngles.z) {
			updateRotation = transform.rotation.eulerAngles.z;

			update = true;
		}

		if (updateSize != lightSize) {
			updateSize = lightSize;

			update = true;
		}

		if (updateColor.Equals(lightColor) == false) {
			updateColor = lightColor;
		}

		if (updateAlpha != lightAlpha) {
			updateAlpha = lightAlpha;
		}

		if (update == true) {
			LightingBuffer2D buffer = LightingBuffer2D.GetBuffer (GetTextureSize(), this);
			buffer.lightSource = this;
			//buffer.bufferCamera.enabled=true;
		}
	}
}
