using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightingSource2D))]
public class LightingSource2DEditor : Editor {

	override public void OnInspectorGUI() {
		LightingSource2D script = target as LightingSource2D;

		EditorGUI.BeginDisabledGroup(true);
		EditorGUILayout.EnumPopup("Preset", LightingManager2D.Get().preset);
		EditorGUI.EndDisabledGroup();

		Color newColor = EditorGUILayout.ColorField("Color", script.lightColor);

		if (script.lightColor.Equals(newColor) == false) {
			newColor.a = 1f;
			script.lightColor = newColor;
			LightingMainBuffer2D.ForceUpdate();
		}

		float newAlpha = EditorGUILayout.Slider("Alhpa", script.lightAlpha, 0, 1);
		if (script.lightAlpha != newAlpha) {
			script.lightAlpha = newAlpha;
			LightingMainBuffer2D.ForceUpdate();
		}

		float newLightSize = EditorGUILayout.FloatField("Size", script.lightSize);
		if (newLightSize != script.lightSize) {
			script.lightSize = newLightSize;
			LightingMainBuffer2D.ForceUpdate();
		}
		
		script.textureSize = (LightingSource2D.TextureSize)EditorGUILayout.EnumPopup("Buffer Size", script.textureSize);
		script.lightSprite = (LightingSource2D.LightSprite)EditorGUILayout.EnumPopup("Light Sprite", script.lightSprite);

		if (script.lightSprite == LightingSource2D.LightSprite.Custom) {
			Sprite newSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", script.sprite, typeof(Sprite), true);
			if (newSprite != script.sprite) {
				script.sprite = newSprite;
				script.SetMaterial();
			}
		}

		script.rotationEnabled = EditorGUILayout.Toggle("Enable Rotation", script.rotationEnabled);
	}
}
