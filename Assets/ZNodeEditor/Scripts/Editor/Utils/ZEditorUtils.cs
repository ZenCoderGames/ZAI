//////////////////////////////////////////////////////////////////////////////////////////////////
/// Class: 	  ZEditorUtils
/// Purpose:  A bunch of helper functions for nodes, gui, rects etc.
/// Author:   Srinavin Nair
//////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;

namespace ZEditor {
	
	public class ZEditorUtils {
		public static Color preTintColor = Color.white;

		public static GUIStyle CreateGUIStyle(string imagePath) {
			GUIStyle newStyle = new GUIStyle();
			newStyle.normal.background = EditorGUIUtility.Load(imagePath) as Texture2D;
			return newStyle;
		}

		public static GUIStyle CreateGUIStyle(Texture image) {
			GUIStyle newStyle = new GUIStyle();
			newStyle.normal.background = image as Texture2D;
			return newStyle;
		}

		public static void StartTint(Color color) {
			preTintColor = GUI.color;
			GUI.color = color;
		}

		public static void ResetTint() {
			GUI.color = preTintColor;
		}

		public static void CreateScriptableObject<T>(string path) {
			ScriptableObject asset = ScriptableObject.CreateInstance(typeof(T));

			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();

			EditorUtility.FocusProjectWindow();

            UnityEditor.Selection.activeObject = asset;
		}

		public static T ParseEnum<T>(string value) {
			return (T) System.Enum.Parse(typeof(T), value, true);
		}
	}

	// Helper Rect extension methods
	public static class RectExtensions
	{
		public static Vector2 TopLeft(this Rect rect)
		{
			return new Vector2(rect.xMin, rect.yMin);
		}
		public static Rect ScaleSizeBy(this Rect rect, float scale)
		{
			return rect.ScaleSizeBy(scale, rect.center);
		}
		public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
		{
			Rect result = rect;
			result.position *= scale;
			result.size *= scale;
			return result;
		}
		public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
		{
			return rect.ScaleSizeBy(scale, rect.center);
		}
		public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
		{
			Rect result = rect;
			result.x -= pivotPoint.x;
			result.y -= pivotPoint.y;
			result.xMin *= scale.x;
			result.xMax *= scale.x;
			result.yMin *= scale.y;
			result.yMax *= scale.y;
			result.x += pivotPoint.x;
			result.y += pivotPoint.y;
			return result;
		}
	}
}
