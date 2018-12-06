using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightingTileMap))]
public class Lighting2DTilemapEditor : Editor {
	override public void OnInspectorGUI() {
		LightingTileMap script = target as LightingTileMap;

		EditorGUI.BeginDisabledGroup(true);
		EditorGUILayout.EnumPopup("Preset", LightingManager2D.Get().preset);
		EditorGUI.EndDisabledGroup();

		script.mapType = (LightingTileMap.MapType)EditorGUILayout.EnumPopup("MapType", script.mapType);
		
		script.dayHeight = EditorGUILayout.Toggle("Day Height", script.dayHeight);
		if (script.dayHeight)  {
			script.height = EditorGUILayout.FloatField("Height", script.height);
		}
		
		script.ambientOcclusion = EditorGUILayout.Toggle("Ambient Occlusion", script.ambientOcclusion);
		if (script.ambientOcclusion)  {
			script.occlusionSize = EditorGUILayout.FloatField("Occlussion Size", script.occlusionSize);
		}

		script.area = EditorGUILayout.BoundsIntField("Area", script.area);
	}
}
