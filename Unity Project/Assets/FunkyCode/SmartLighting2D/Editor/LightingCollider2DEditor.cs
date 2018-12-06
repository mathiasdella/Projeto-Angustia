using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightingCollider2D))]
public class LightingCollider2DEditor : Editor {

	override public void OnInspectorGUI() {
		LightingCollider2D script = target as LightingCollider2D;

		EditorGUI.BeginDisabledGroup(true);
		EditorGUILayout.EnumPopup("Preset", LightingManager2D.Get().preset);
		EditorGUI.EndDisabledGroup();
		
		script.dayHeight = EditorGUILayout.Toggle("Day Height", script.dayHeight);
		if (script.dayHeight)  {
			script.height = EditorGUILayout.FloatField("Height", script.height);
			if (script.height < 0) {
				script.height = 0;
			}
		}

		script.ambientOcclusion = EditorGUILayout.Toggle("Ambient Occlusion", script.ambientOcclusion);
		if (script.ambientOcclusion)  {
			script.smoothOcclusionEdges = EditorGUILayout.Toggle("Smooth Edges", script.smoothOcclusionEdges);
			script.occlusionSize = EditorGUILayout.FloatField("Occlussion Size", script.occlusionSize);
			if (script.occlusionSize < 0) {
				script.occlusionSize = 0;
			}
		}

		//script.lighten = EditorGUILayout.Toggle("Lighten", script.lighten);
		//script.darken = EditorGUILayout.Toggle("Darken", script.darken);
	}
}
