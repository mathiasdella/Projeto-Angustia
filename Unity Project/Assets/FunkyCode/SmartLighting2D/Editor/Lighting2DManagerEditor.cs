using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightingManager2D))]
public class Lighting2DManagerEditor : Editor {
	override public void OnInspectorGUI() {
		LightingManager2D script = target as LightingManager2D;
		
		script.enable = EditorGUILayout.Toggle("Enable", script.enable);

		EditorGUI.BeginDisabledGroup(script.enable == false);

		script.preset = (LightingManager2D.Preset)EditorGUILayout.EnumPopup("Preset", script.preset);

		Color newDarknessColor = EditorGUILayout.ColorField("Darkness Color", script.darknessColor);
		if (script.darknessColor.Equals(newDarknessColor) == false) {
			script.darknessColor = newDarknessColor;

			LightingMainBuffer2D.ForceUpdate();	
		}

		float newShadowDarkness = EditorGUILayout.Slider("Shadow Darkness", script.shadowDarkness, 0, 1);
		if (newShadowDarkness != script.shadowDarkness) {
			script.shadowDarkness = newShadowDarkness;

			LightingMainBuffer2D.ForceUpdate();
		}
		
		float newSunDirection = EditorGUILayout.FloatField("Sun Rotation", script.sunDirection);
		if (newSunDirection != script.sunDirection) {
			script.sunDirection = newSunDirection;

			LightingMainBuffer2D.ForceUpdate();
		}
		
		script.debug = EditorGUILayout.Toggle("Debug", script.debug);

		EditorGUI.EndDisabledGroup();
	}
}
 