using UnityEditor;
using UnityEngine;

[CustomEditor ( typeof ( NinePatch ) ), CanEditMultipleObjects]
class NinePatchInspector : Editor
{
	[MenuItem ( "GameObject/Create Other/Nine Patch (extended)", false, 5 )]
	public static void CreateNinePatch ()
	{
		GameObject go = new GameObject ();
		go.name = "NinePatch";

		/* NinePatch ninePatch = */ go.GetOrCreateComponent<NinePatch> ();

		if ( Selection.activeGameObject != null )
		{
			go.transform.parent = Selection.activeGameObject.transform;
			go.transform.position = Selection.activeGameObject.transform.position;
		}
		Selection.activeObject = go;
	}

	SerializedProperty Color;
	SerializedProperty Width;
	SerializedProperty Height;
	SerializedProperty Left;
	SerializedProperty Top;
	SerializedProperty Right;
	SerializedProperty Bottom;
	SerializedProperty Alignment;
	SerializedProperty Texture;
	
	void OnEnable ()
	{
		Color = serializedObject.FindProperty ( "Color" );
		Width = serializedObject.FindProperty ( "Width" );
		Height = serializedObject.FindProperty ( "Height" );
		Left = serializedObject.FindProperty ( "Left" );
		Top = serializedObject.FindProperty ( "Top" );
		Right = serializedObject.FindProperty ( "Right" );
		Bottom = serializedObject.FindProperty ( "Bottom" );
		Alignment = serializedObject.FindProperty ( "Alignment" );
		Texture = serializedObject.FindProperty ( "Texture" );
	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();

		EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.BeginVertical ();

				EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ( "Size", GUILayout.Width ( 60 ) );
					// GUILayout.FlexibleSpace ();
					EditorGUILayout.PropertyField ( Width, GUIContent.none, GUILayout.Width ( 40 ) );
					EditorGUILayout.LabelField ( "x", GUILayout.Width ( 12 ) );
					EditorGUILayout.PropertyField ( Height, GUIContent.none, GUILayout.Width ( 40 ) );
					
					EditorGUILayout.LabelField ( "", GUILayout.Width ( 25 ) );
		
					EditorGUILayout.LabelField ( "Color", GUILayout.Width ( 40 ) );
					EditorGUILayout.PropertyField ( Color, GUIContent.none, GUILayout.Width ( 60 ) );
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ( "Alignment", GUILayout.Width ( 60 ) );
					EditorGUILayout.PropertyField ( Alignment, GUIContent.none, GUILayout.Width ( 60 ) );
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ( "Corners", GUILayout.Width ( 60 ) );
					// GUILayout.FlexibleSpace ();
					EditorGUILayout.PropertyField ( Left, GUIContent.none, GUILayout.Width ( 40 ) );
					EditorGUILayout.LabelField ( "x", GUILayout.Width ( 12 ) );
					EditorGUILayout.PropertyField ( Top, GUIContent.none, GUILayout.Width ( 40 ) );

					EditorGUILayout.LabelField ( "-", GUILayout.Width ( 12 ) );

					EditorGUILayout.PropertyField ( Right, GUIContent.none, GUILayout.Width ( 40 ) );
					EditorGUILayout.LabelField ( "x", GUILayout.Width ( 12 ) );
					EditorGUILayout.PropertyField ( Bottom, GUIContent.none, GUILayout.Width ( 40 ) );
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ( "Texture", GUILayout.Width ( 60 ) );
					EditorGUILayout.PropertyField ( Texture, GUIContent.none );
				EditorGUILayout.EndHorizontal ();

			UnityToolbag.SortingLayerExposedEditor.DrawSortingLayerGUI ( target, targets );

			EditorGUILayout.EndVertical ();

			GUILayout.FlexibleSpace ();

		EditorGUILayout.EndHorizontal ();

		serializedObject.ApplyModifiedProperties ();

		if ( GUI.changed )
		{
			foreach ( NinePatch q in targets )
				q.Recreate ();
		}
	}
}