using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

// [CustomEditor ( typeof ( Transform ), true ), CanEditMultipleObjects()]
public class TransformEditor : Editor
{
	static bool isLocal = false;

	/*
	public static Vector3 Vector3FieldEx ( string label, Vector3 value, params GUILayoutOption[] options )
	{
		bool smallScreen = Screen.width < 333;

		// Debug.Log ( "Screen.width = " + Screen.width );

		if ( smallScreen )
			EditorGUILayout.PrefixLabel ( label );

		EditorGUILayout.BeginHorizontal ();

		if ( smallScreen )
			EditorGUILayout.LabelField ( "", GUILayout.Width ( 12 ) );
		else
			EditorGUILayout.PrefixLabel ( label );

		EditorGUILayout.LabelField ( "X", GUILayout.Width ( 12 ) );
		value.x = EditorGUILayout.FloatField ( value.x );

		EditorGUILayout.LabelField ( "Y", GUILayout.Width ( 12 ) );
		value.y = EditorGUILayout.FloatField ( value.y );

		EditorGUILayout.LabelField ( "Z", GUILayout.Width ( 12 ) );
		value.z = EditorGUILayout.FloatField ( value.z );

		EditorGUILayout.EndHorizontal ();

		return value;
	}
	*/

	private static float FloatField ( string label, float value )
	{
		EditorGUILayout.LabelField ( label, GUILayout.Width ( 12 ) );
		
		if ( float.IsNaN ( value ) )
		{
			string valueAsText = EditorGUILayout.TextField ( "--", GUILayout.MaxWidth ( 60 ) );
			float.TryParse ( valueAsText, out value );
		}
		else
			value = EditorGUILayout.FloatField ( value, GUILayout.MaxWidth ( 60 ) );

		return value;
	}

	public static Vector3 Vector3FieldEx ( string label, Vector3 value )
	{
 		EditorGUILayout.BeginHorizontal ();

		EditorGUILayout.LabelField ( label, GUILayout.Width ( Screen.width/6 ) );
		GUILayout.FlexibleSpace ();

		float x = FloatField ( "X", value.x );
		float y = FloatField ( "Y", value.y );
		float z = FloatField ( "Z", value.z );

		EditorGUILayout.EndHorizontal ();

		return new Vector3 ( x, y, z );
	}

	public static Vector3 GetLocalPosition ( Transform[] transforms )
	{
		if ( transforms.Length > 0 )
		{
			Vector3 value = transforms[0].localPosition;
			for ( int i = 1; i < transforms.Length; i++ )
			{
				Vector3 valueEx = transforms[i].localPosition;
				if ( ( value.x != float.NaN ) && ( value.x != valueEx.x ) ) value.x = float.NaN;
				if ( ( value.y != float.NaN ) && ( value.y != valueEx.y ) ) value.y = float.NaN;
				if ( ( value.z != float.NaN ) && ( value.z != valueEx.z ) ) value.z = float.NaN;
			}
			return value;
		}
		else
			return new Vector3 ( float.NaN, float.NaN, float.NaN );
	}

	public static Vector3 GetWorldPosition ( Transform[] transforms )
	{
		if ( transforms.Length > 0 )
		{
			Vector3 value = transforms[0].position;
			for ( int i = 1; i < transforms.Length; i++ )
			{
				Vector3 valueEx = transforms[i].position;
				if ( ( value.x != float.NaN ) && ( value.x != valueEx.x ) ) value.x = float.NaN;
				if ( ( value.y != float.NaN ) && ( value.y != valueEx.y ) ) value.y = float.NaN;
				if ( ( value.z != float.NaN ) && ( value.z != valueEx.z ) ) value.z = float.NaN;
			}
			return value;
		}
		else
			return new Vector3 ( float.NaN, float.NaN, float.NaN );
	}

	public static Vector3 GetLocalEulerAngles ( Transform[] transforms )
	{
		if ( transforms.Length > 0 )
		{
			Vector3 value = transforms[0].localEulerAngles;
			for ( int i = 1; i < transforms.Length; i++ )
			{
				Vector3 valueEx = transforms[i].localEulerAngles;
				if ( ( value.x != float.NaN ) && ( value.x != valueEx.x ) ) value.x = float.NaN;
				if ( ( value.y != float.NaN ) && ( value.y != valueEx.y ) ) value.y = float.NaN;
				if ( ( value.z != float.NaN ) && ( value.z != valueEx.z ) ) value.z = float.NaN;
			}
			return value;
		}
		else
			return new Vector3 ( float.NaN, float.NaN, float.NaN );
	}

	public static Vector3 GetWorldEulerAngles ( Transform[] transforms )
	{
		if ( transforms.Length > 0 )
		{
			Vector3 value = transforms[0].eulerAngles;
			for ( int i = 1; i < transforms.Length; i++ )
			{
				Vector3 valueEx = transforms[i].eulerAngles;
				if ( ( value.x != float.NaN ) && ( value.x != valueEx.x ) ) value.x = float.NaN;
				if ( ( value.y != float.NaN ) && ( value.y != valueEx.y ) ) value.y = float.NaN;
				if ( ( value.z != float.NaN ) && ( value.z != valueEx.z ) ) value.z = float.NaN;
			}
			return value;
		}
		else
			return new Vector3 ( float.NaN, float.NaN, float.NaN );
	}

	public static Vector3 GetLocalScale ( Transform[] transforms )
	{
		if ( transforms.Length > 0 )
		{
			Vector3 value = transforms[0].localScale;
			for ( int i = 1; i < transforms.Length; i++ )
			{
				Vector3 valueEx = transforms[i].localScale;
				if ( ( value.x != float.NaN ) && ( value.x != valueEx.x ) ) value.x = float.NaN;
				if ( ( value.y != float.NaN ) && ( value.y != valueEx.y ) ) value.y = float.NaN;
				if ( ( value.z != float.NaN ) && ( value.z != valueEx.z ) ) value.z = float.NaN;
			}
			return value;
		}
		else
			return new Vector3 ( float.NaN, float.NaN, float.NaN );
	}

	public static Vector3 GetWorldLossyScale ( Transform[] transforms )
	{
		if ( transforms.Length > 0 )
		{
			Vector3 value = transforms[0].lossyScale;
			for ( int i = 1; i < transforms.Length; i++ )
			{
				Vector3 valueEx = transforms[i].lossyScale;
				if ( ( value.x != float.NaN ) && ( value.x != valueEx.x ) ) value.x = float.NaN;
				if ( ( value.y != float.NaN ) && ( value.y != valueEx.y ) ) value.y = float.NaN;
				if ( ( value.z != float.NaN ) && ( value.z != valueEx.z ) ) value.z = float.NaN;
			}
			return value;
		}
		else
			return new Vector3 ( float.NaN, float.NaN, float.NaN );
	}

	public override void OnInspectorGUI ()
	{
// 		if ( GUILayout.Button ( "Reset" ) )
// 		{
// 			var transform = target as Transform;
// 
// 			transform.position = Vector3.zero;
// 			transform.localScale = Vector3.zero;
// 			transform.rotation = Quaternion.identity;
// 		}
 
// 		DrawDefaultInspector ();
		
		EditorGUILayout.BeginHorizontal ();
		string space = isLocal ? "Local" : "World";
		if ( GUILayout.Button ( space ) )
		{
			GUI.changed = false;
			isLocal = !isLocal;
		}
		EditorGUILayout.EndHorizontal ();

		Transform[] transforms = new Transform[targets.Length];
		for ( int i = 0 ; i < targets.Length; i++ )
			transforms[i] = targets[i] as Transform;

		EditorGUIUtility.LookLikeControls ();
		EditorGUI.indentLevel = 0;

  		Vector3 position = Vector3FieldEx ( "Position", isLocal ? GetLocalPosition ( transforms ) : GetWorldPosition ( transforms ) );
  		Vector3 eulerAngles = Vector3FieldEx ( "Rotation", isLocal ? GetLocalEulerAngles ( transforms ) : GetWorldEulerAngles ( transforms ) );
		Vector3 scale = Vector3FieldEx ( "Scale", isLocal ? GetLocalScale ( transforms ) : GetWorldLossyScale ( transforms ) );

		if ( GUI.changed )
		{
			Undo.RecordObjects ( transforms, "Transform Change" );

			foreach ( Transform transform in targets )
			{
				if ( isLocal )
				{
					transform.localPosition = position;
					transform.localEulerAngles = eulerAngles;
					transform.localScale = scale;
				}
				else
				{
					transform.position = position;
					transform.eulerAngles = eulerAngles;
					Vector3 lossyScale = GetWorldLossyScale ( transforms );
					Vector3 newScale = new Vector3 (
						transform.localScale.x * ( scale.x / lossyScale.x ),
						transform.localScale.y * ( scale.y / lossyScale.y ),
						transform.localScale.z * ( scale.z / lossyScale.z )
						);
					transform.localScale = newScale;
				}

				EditorUtility.SetDirty ( transform );
			}
		}
	}
}