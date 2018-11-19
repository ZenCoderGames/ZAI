using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using WyrmTale;

public static class ZUtils
{
    //x' = x cos θ − y sin θ
    //y' = x sin θ + y cos θ
    public static Vector3 RotateByAngleOnYAxis(Vector3 baseVector, float angle) {
        float rad = Mathf.Deg2Rad * angle;
        float newX = baseVector.x * Mathf.Cos(rad) - baseVector.z * Mathf.Sin(rad);
        float newZ = baseVector.x * Mathf.Sin(rad) + baseVector.z * Mathf.Cos(rad);
        return new Vector3(newX, baseVector.y, newZ);
    }

    public static Vector3 RotateByAngleOnZAxis(Vector3 baseVector, float angle) {
        float rad = Mathf.Deg2Rad * angle;
        float newX = baseVector.x * Mathf.Cos(rad) - baseVector.y * Mathf.Sin(rad);
        float newY = baseVector.x * Mathf.Sin(rad) + baseVector.y * Mathf.Cos(rad);
        return new Vector3(newX, newY, baseVector.y);
    }

	public static float GetDistanceBetweenVector(Vector3 a, Vector3 b, bool ignoreX, bool ignoreY, bool ignoreZ)
	{
		return Vector3.Distance (new Vector3 (ignoreX ? 0 : a.x, ignoreY ? 0 : a.y, ignoreZ ? 0 : a.z),
		                        	new Vector3 (ignoreX ? 0 : b.x, ignoreY ? 0 : b.y, ignoreZ ? 0 : b.z));
	}

	public static float GetSignedAngle(Vector2 a, Vector2 b) {
		Vector2 v = b - a;
		return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
	}

	public static float GetSignedAngle(Vector3 a, Vector3 b, Vector3 n) {
		// angle in [0,180]
		float angle = Vector3.Angle(a, b);
		float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));

		// angle in [-179,180]
		float signed_angle = angle * sign;

		// angle in [0,360] (not used but included here for completeness)
		//float angle360 =  (signed_angle + 180) % 360;

		return signed_angle;
	}

	public static float Get360Angle(Vector3 v1, Vector3 v2)
	{
		float angle = Vector3.Angle(v1, v2);

		float sign = Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(v1, v2)));
		float signedAngle = angle * sign;

		return (signedAngle <= 0) ? 360 + signedAngle : signedAngle;
	}

	public static void ClampInt(ref int current, int min, int max)
	{
		if (current < min)
			current = min;
		else if (current > max)
			current = max;
	}

	public static bool IsWithin(int value, int min, int max)
	{
		return value >= min && value <= max;
	}

	public static bool IsWithin(float value, float min, float max)
	{
		return value >= min && value <= max;
	}

	public static Vector3 Bezier2(Vector3 Start, Vector3 Control, Vector3 End, float t)
	{
		return (((1-t)*(1-t)) * Start) + (2 * t * (1 - t) * Control) + ((t * t) * End);
	}
	
	public static Vector3 Bezier3(Vector3 s, Vector3 st, Vector3 et, Vector3 e, float t)
	{
		return (((-s + 3*(st-et) + e)* t + (3*(s+et) - 6*st))* t + 3*(st-s))* t + s;
	}

	public static void GetRelativeOrientation(Transform parent, Transform child, out Vector3 position, out Quaternion rotation) {
		position = parent.InverseTransformPoint(child.position);
		rotation = Quaternion.Inverse(parent.rotation) * child.rotation;
	}

	/// <summary>
	/// Gets the direction (a - b)
	/// </summary>
	/// <returns>The direction.</returns>
	/// <param name="a">The alpha component.</param>
	/// <param name="b">The blue component.</param>
	/// <param name="ignoreX">If set to <c>true</c> ignore x.</param>
	/// <param name="ignoreY">If set to <c>true</c> ignore y.</param>
	/// <param name="ignoreZ">If set to <c>true</c> ignore z.</param>
	/// <param name="isNormalized">If set to <c>true</c> is normalized.</param>
	public static Vector3 GetDirection(Vector3 a, Vector3 b, bool ignoreX = false, bool ignoreY = false, bool ignoreZ = false, bool isNormalized = false)
	{
		Vector3 dirn = new Vector3(ignoreX?0:a.x-b.x, ignoreY?0:a.y-b.y, ignoreZ?0:a.z-b.z);
		return isNormalized?dirn.normalized:dirn;
	}
	
	public static Vector3 GetVector(Vector3 a, bool ignoreX = false, bool ignoreY = false, bool ignoreZ = false)
	{
		Vector3 newVec = new Vector3(ignoreX?0:a.x, ignoreY?0:a.y, ignoreZ?0:a.z);
		return newVec;
	}

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angle)
    {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler(angle) * dir;
        point = dir + pivot;
        return point;
    }

	public static void SetLayerRecursively(Transform current, int layer)
	{
		current.gameObject.layer = layer;
		
		// search through child bones for the bone we're looking for
		int numChildren = current.childCount;
		for (int i = 0; i < numChildren; ++i)
		{
			// the recursive step; repeat the search one step deeper in the hierarchy
			SetLayerRecursively(current.GetChild(i), layer);
		}
	}

	#if UNITY_EDITOR
	public static void SetStaticEditorFlagsRecursively(Transform current, UnityEditor.StaticEditorFlags flags, UnityEditor.StaticEditorFlags mask) {
		// set desired flags of current
		UnityEditor.StaticEditorFlags staticFlags = UnityEditor.GameObjectUtility.GetStaticEditorFlags(current.gameObject);
		staticFlags = (staticFlags & ~mask) | (flags & mask);
		UnityEditor.GameObjectUtility.SetStaticEditorFlags(current.gameObject, staticFlags);

		// recursively
		int numChildren = current.childCount;
		for (int i = 0; i < numChildren; ++i) {
			SetStaticEditorFlagsRecursively(current.GetChild(i), flags, mask);
		}
	}
	#endif

	public static Transform FindChildRecursively(Transform current, string name, bool partialMatch = false)
	{
		// check if the current bone is the bone we're looking for, if so return it
		if (partialMatch ? current.name.Contains(name) : (current.name == name))
			return current;
		
		// search through child bones for the bone we're looking for
		int numChildren = current.childCount;
		for (int i = 0; i < numChildren; ++i)
		{
			// the recursive step; repeat the search one step deeper in the hierarchy
			Transform found = FindChildRecursively(current.GetChild(i), name, partialMatch);
			
			// a transform was returned by the search above that is not null,
			// it must be the bone we're looking for
			if (found != null)
				return found;
		}
		
		// bone with name was not found
		return null;
	}

	public static void RemoveAllChildren(Transform parent) {
		Transform[] children = new Transform[parent.childCount];
	
		for (int i = 0; i < parent.childCount; ++i) {
			children[i] = parent.GetChild(i);
		}

		for (int i = 0; i < children.Length; ++i) {
			children[i].transform.SetParent(null, false);

			GameObject.Destroy(children[i].gameObject);
		}
	}

	public static T[] GetInterfaces<T>(GameObject gObj) {
		if (typeof(T).IsInterface) {
			MonoBehaviour[] mObjs = gObj.GetComponents<MonoBehaviour>();

			return (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a).ToArray();
		}
		return null;
	}

	public static T GetInterface<T>(GameObject gObj) {
		return GetInterfaces<T>(gObj).FirstOrDefault();
	}

	public static T GetInterfaceInChildren<T>(GameObject gObj) {
		return GetInterfacesInChildren<T>(gObj).FirstOrDefault();
	}

	public static T[] GetInterfacesInChildren<T>(GameObject gObj) {
		if (typeof(T).IsInterface) {
			MonoBehaviour[] mObjs = gObj.GetComponentsInChildren<MonoBehaviour>();

			return (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a).ToArray();
		}
		return null;
	}

	public static void ApplyFunctionRecursively(Transform current, System.Action<Transform> applyFunc)
	{
		if (current == null)
			return;
		else
			applyFunc(current);

		// search through child bones for the bone we're looking for
		int numChildren = current.childCount;
		for (int i = 0; i < numChildren; ++i)
		{
			// the recursive step; repeat the search one step deeper in the hierarchy
			ApplyFunctionRecursively(current.GetChild(i), applyFunc);
		}
		
		// bone with name was not found
		return;
	}

	/*public static void TintParticleEffectRecursively(Transform current, Color color)
	{
		if (current == null)
			return;
		else 
		{
			ParticleSystem particleSystem = current.GetComponent<ParticleSystem>();
			if(particleSystem!=null)
				particleSystem.startColor = color;
		}
		
		// search through child bones for the bone we're looking for
		int numChildren = current.childCount;
		for (int i = 0; i < numChildren; ++i)
		{
			// the recursive step; repeat the search one step deeper in the hierarchy
			TintParticleEffectRecursively(current.GetChild(i), color);
		}
		
		// bone with name was not found
		return;
	}*/

	public static List<string> GetStringList(string value)
	{	
		List<string> stringList = new List<string> ();
		char[] delimiterChars = { ' ', ',', ':', '[', ']' };
		string[] vals = value.Split (delimiterChars);				
		for (int i=0; i<vals.Length; ++i) {
			if(string.IsNullOrEmpty(vals[i]))
				continue;
			
			stringList.Add(vals[i]);
		}
		
		return stringList;
	}

	// Fisher-Yates shuffle
	public static void RandomizeList<T>(ref List<T> list)
	{
		int n = list.Count;
		T obj;
		for (int i = 0; i < n; i++)
		{
			// NextDouble returns a random number between 0 and 1.
            int r = Random.Range(i, n);
			obj = list[r];
			list[r] = list[i];
			list[i] = obj;
		}
	}

	// Fisher-Yates shuffle
	public static void RandomizeArray<T>(ref T[] array)
	{		
		int n = array.Length;
		T obj;
		for (int i = 0; i < n; i++)
		{
			// NextDouble returns a random number between 0 and 1.
			int r = i + (int)(Random.Range(0.0f, 1.0f) * (n - i));
			obj = array[r];
			array[r] = array[i];
			array[i] = obj;
		}
	}

	/// <summary>
	/// Evaluates the chance.
	/// </summary>
	/// <returns><c>true</c>, if chance was evaluated, <c>false</c> otherwise.</returns>
	/// <param name="percent">Percent (out of 100).</param>
	public static bool EvaluateChance(float percent)
	{
		return Random.Range(0, 100) < percent;
	}

	public static bool DoesArrayContain<T>(T[] array, T id)
	{
		if (array == null)
			return false;

		for (int i=0; i<array.Length; ++i) {
			if(array[i].Equals(id))
				return true;
		}

		return false;
	}

	public static T GetRandomEntryFromList<T>(List<T> vals)
	{
		int randomNum = Random.Range (0, vals.Count);
		return vals[randomNum];
	}

    public static T GetRandomEntryFromArray<T>(T[] vals)
    {
        int randomNum = Random.Range (0, vals.Length);
        return vals[randomNum];
    }

    public static bool IsPointWithinCameraView(ref Camera cam, Vector3 pos)
    {
        Vector3 viewportPos = cam.WorldToViewportPoint(pos);

        if (viewportPos.x >= 0 && viewportPos.x < 1 &&
           viewportPos.y >= 0 && viewportPos.y < 1)
            return true;

        return false;
    }

	public static string ToCamelCase(string value, bool firstLowerCase = false) {
		string result = "";
		bool upper = !firstLowerCase;

		for (int i = 0; i < value.Length; ++i) {
			char chr = value[i];
			if (chr == '_' || chr == ' ') {
				upper = true;
			} else {
				result += (upper ? char.ToUpper(chr) : char.ToLower(chr));
				upper = false;
			}
		}

		return result;
	}

	public static string ToUnderscoreCase(string value, bool uppercase = false) {
		string result = "";
		string segment = "";

		for (int i = 0; i < value.Length; ++i) {
			char chr = value[i];
			if (i > 0 && char.IsUpper(chr)) {
				segment += chr;
			} else {
				if (!string.IsNullOrEmpty(segment)) {
					if (segment.Length > 1) {
						result += "_" + segment.Substring(0, segment.Length - 1);
					}
					result += "_" + segment.Substring(segment.Length - 1);
					segment = "";
				}

				result += chr;
			}
		}
		if (segment.Length > 0) {
			result += "_" + segment;
		}

		result = (uppercase ? result.ToUpper() : result.ToLower());
		return result;
	}

    public static int GetChildIndexOfParent(Transform child, Transform parent)
    {
        for (int i = 0; i < parent.childCount; ++i)
        {
            if (parent.GetChild(i) == child)
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Gets the random point around position
    /// </summary>
    /// <returns>The random point around position.</returns>
    /// <param name="pos">Position.</param>
    /// <param name="min">Min Radius</param>
    /// <param name="max">Max Radius</param>
    public static Vector3 GetRandomPointAroundPosition(Vector3 pos, float min, float max)
    {
        float randomX = Random.Range(min, max) * (EvaluateChance(50) ? 1 : -1);
        float randomZ = Random.Range(min, max) * (EvaluateChance(50) ? 1 : -1);
        return pos + Vector3.right * randomX + Vector3.forward * randomZ;
    }

	public static Color GetColorFromHexString(string hex)
	{
		hex = hex.Replace("0x", "");
		hex = hex.Replace("#", "");
		byte a = 255;
		byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
		if (hex.Length == 8)
		{
			a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
		}

		return new Color(r/255.0f, g/255.0f, b/255.0f, a/255.0f);
	}

    #region JSON_HELPERS
    public static JSON FindJSONOfId(ref JSON[] listOfJSON, string id)
    {
        for(int i=0; i<listOfJSON.Length; ++i)
        {
            if(listOfJSON[i].ToString("id") == id)
                return listOfJSON[i];
        }

        //Debug.Log(id + " not found");
        return null;
    }

    public static Vector3 GetVector(ref JSON js, string value)
    {
        Vector3 vec = Vector3.zero;
        float[] offsetFloats = js.ToArray<float>(value);
        if (offsetFloats != null) {
            if(offsetFloats.Length == 0)
                vec = Vector3.zero;
            else if (offsetFloats.Length != 3)
				Debug.Log ("Error " + value + " should be Vector3");
            else
                vec = new Vector3 (offsetFloats [0], offsetFloats [1], offsetFloats [2]);
        }

        return vec;
    }

    public static Color GetColor(ref JSON js, string value)
    {
        Color color = Color.black;
        float[] offsetFloats = js.ToArray<float>(value);
        if (offsetFloats != null) {
            if(offsetFloats.Length == 0)
                color = Color.black;
            else if (offsetFloats.Length == 3)
                color = new Color (offsetFloats [0], offsetFloats [1], offsetFloats [2], 1);
            else if(offsetFloats.Length == 4)
                color = new Color (offsetFloats [0], offsetFloats [1], offsetFloats [2], offsetFloats [3]);
        }

        return color;
    }

	public static Color GetColorFromExcelConverted(ref JSON js, string value, Color? defaultValue = null)
    {
		Color color = defaultValue.HasValue ? defaultValue.Value : Color.black;
        string colorStr = js.ToString (value);

		if (!string.IsNullOrEmpty(colorStr)) {
			if ((colorStr.StartsWith("#") || char.IsLetter(colorStr[0]))) {
				if (!ColorUtility.TryParseHtmlString(colorStr, out color)) {
					Debug.LogWarning("Invalid Color HEX value (" + colorStr + ") found in data for key: " + value);
				}
			} else {
				char[] delimiterChars = { ' ', ',', ':', '[', ']' };
				string[] vals = colorStr.Split(delimiterChars);
				float[] rgb = new float[3];
				int counter = 0;
				for (int i = 0; i < vals.Length; ++i) {
					if (string.IsNullOrEmpty(vals[i]))
						continue;

					rgb[counter++] = float.Parse(vals[i]);
				}
				color = new Color(rgb[0], rgb[1], rgb[2], 1);
			}
		}

        return color;
    }

    public static List<string> GetStringListFromExcelConverted(ref JSON js, string value)
    {
        List<string> stringList = new List<string> ();
        string stringStr = js.ToString (value);
        char[] delimiterChars = { ' ', ',', ':', '[', ']' };
        string[] vals = stringStr.Split (delimiterChars);
        for (int i=0; i<vals.Length; ++i) {
            if(string.IsNullOrEmpty(vals[i]))
                continue;

            stringList.Add(vals[i]);
        }

        return stringList;
    }

    public static List<int> GetIntListFromExcelConverted(ref JSON js, string value)
    {
        List<int> intList = new List<int> ();
        string stringStr = js.ToString (value);
        char[] delimiterChars = { ' ', ',', ':', '[', ']' };
        string[] vals = stringStr.Split (delimiterChars);
        for (int i=0; i<vals.Length; ++i) {
            if(string.IsNullOrEmpty(vals[i]))
                continue;

            intList.Add(int.Parse(vals[i]));
        }

        return intList;
    }

	public static Vector3 GetVector3FromExcelConverted(ref JSON js, string value)
	{
		Vector3 vec = new Vector3(0, 0, 0);
		string stringStr = js.ToString (value);
		char[] delimiterChars = { ' ', ',', ':', '[', ']' };
		string[] vals = stringStr.Split (delimiterChars);
		int j = 0;
		for (int i=0; i<vals.Length; ++i) {
			if(string.IsNullOrEmpty(vals[i]))
				continue;

			vec[j++] = float.Parse(vals[i]);
		}

		return vec;
	}

    public static int GetIntValue(ref JSON js, string id, int defaultVal)
    {
        return js.Contains(id) ? js.ToInt(id) : defaultVal;
    }

    public static bool GetBoolValue(ref JSON js, string id, bool defaultVal)
    {
        return js.Contains(id) ? js.ToBoolean(id) : defaultVal;
    }

    public static float GetFloatValue(ref JSON js, string id, float defaultVal)
    {
        return js.Contains(id) ? js.ToFloat(id) : defaultVal;
    }

    public static string GetStringValue(ref JSON js, string id, string defaultVal)
    {
        return js.Contains(id) ? js.ToString(id) : defaultVal;
    }
    #endregion

    #region DEBUG_DRAWING_UTILS
    public static void DrawGizmoArrow(Vector3 pos, Vector3 direction, bool isXYPlane, float arrowHeadLength = 0.5f, float arrowHeadAngle = 45.0f)
    {
        Gizmos.DrawRay(pos, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * Vector3.forward;
        if(isXYPlane) {
            right = Quaternion.LookRotation(direction, -Vector3.forward) * Quaternion.Euler(0,180+arrowHeadAngle,0) * Vector3.forward;
            left = Quaternion.LookRotation(direction, -Vector3.forward) * Quaternion.Euler(0,180-arrowHeadAngle,0) * Vector3.forward;
        }
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    }
    #endregion
}