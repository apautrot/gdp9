using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class LabelNames : EditorWindow
{
	[MenuItem ( "Window/Names Label In Scene", false, 1100 )]
	static void Init ()
	{
		LabelNames.GetWindow<LabelNames> ();
	}

	void OnFocus ()
	{
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
		SceneView.onSceneGUIDelegate += this.OnSceneGUI;
	}

	void OnEnable ()
	{
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
		SceneView.onSceneGUIDelegate += this.OnSceneGUI;
	}

	void OnDestroy ()
	{
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
	}

	static GUIStyle textfieldStyle;

	private void OnSceneGUI ( SceneView sceneView )
	{
		/*
		Vector3 screenPos = new Vector3 ( 200, 200, 0 );
		Vector2 size = new Vector2 ( 200, 50 );
		Rect rect = new Rect ( screenPos.x - size.x / 2, Screen.height - screenPos.y - size.y / 2, size.x, size.y );
		EditorGUI.TextField ( rect, "Hello" );
		*/

		if ( textfieldStyle == null )
		{
			textfieldStyle = new GUIStyle ( GUI.skin.textField );
			// textfieldStyle.alignment = TextAnchor.MiddleCenter;
		}

		Handles.BeginGUI ();
		{
			GUI.color = Color.white;

			GameObject[] all = GameObject.FindObjectsOfType<GameObject> ();
			for ( int i = 0; i < all.Length; i++ )
			{
				GameObject gameObject = all[i];

				Vector3 screenPos = Camera.current.WorldToScreenPoint ( gameObject.transform.position );
				screenPos += new Vector3 ( 0, 38, 0 ); // viewport decoration insets to remove

				Vector2 size = textfieldStyle.CalcSize ( new GUIContent ( gameObject.name + "." ) );

				Rect rect = new Rect ( screenPos.x - size.x / 2, Screen.height - screenPos.y - size.y / 2, size.x, size.y );

				string name = EditorGUI.TextField ( rect, gameObject.name, textfieldStyle );
				if ( GUI.changed )
					gameObject.name = name;
			}
		}
		Handles.EndGUI ();
	}

	static GUIStyle buttonStyle;
	static GUIStyle labelStyle;
	static bool isEnabled;
	static bool isFilteredByTypes;
	static bool isFilteringByInstanceList;
	static bool isInstanceListFoldedOut = true;

//	static List<System.Type> listOfTypes = new List<System.Type> ();
	static List<GameObject> listOfInstances = new List<GameObject>();

//	[DrawGizmo ( GizmoType.SelectedOrChild | GizmoType.NotSelected | GizmoType.Pickable )]
	static void DrawGameObjectName ( GameObject gameObject, GizmoType gizmoType )
	{
		// Debug.Log ( "Yep" );

		if ( ! isEnabled )
			return;

		Transform transform = gameObject.transform;

		if ( isFilteringByInstanceList && !listOfInstances.Contains ( gameObject ) )
			return;

		bool isSelected = Selection.activeGameObject == gameObject;
// 		if ( unlabelSelected && isSelected )
//  			return;

		if ( buttonStyle == null )
			buttonStyle = GUI.skin.GetStyle ( "Button" );

		if ( labelStyle == null )
		{
			labelStyle = GUI.skin.GetStyle ( "Label" );
			labelStyle.fontSize = 11;
			labelStyle.normal.textColor = Color.white;
		}

		GUIStyle style = /* isSelected ? labelStyle : */ buttonStyle;

		Vector2 size = style.CalcSize ( new GUIContent ( gameObject.name ) );

		Vector3 screenPos = Camera.current.WorldToScreenPoint ( transform.position );
		screenPos += new Vector3 ( 0, 38, 0 ); // viewport decoration insets to remove

		if ( isSelected )
		{
// 			screenPos += new Vector3 ( 0, -30, 0 );
// 			GUI.color = new Color ( 0, 1, 0, 0.5f );
		}
		else
		{
			float pixelSize = HandleUtility.GetHandleSize ( transform.position ) / 80;
			Vector3 sizeInWorld = size * pixelSize;
			sizeInWorld.z = 1;

			Matrix4x4 cachedMatrix = Gizmos.matrix;
			Quaternion cameraRotation = Camera.current.transform.rotation;
			Gizmos.matrix = Matrix4x4.TRS ( transform.position, cameraRotation, Vector3.one );

			Gizmos.color = Color.clear;
			Gizmos.DrawCube ( Vector3.zero, sizeInWorld );
			Gizmos.color = Color.white;

			Gizmos.matrix = cachedMatrix;
		}

		Handles.BeginGUI ();
		{
			GUI.color = Color.white;
			Rect rect = new Rect ( screenPos.x - size.x / 2, Screen.height - screenPos.y - size.y / 2, size.x, size.y );
// 			if ( isSelected )
// 			{
// 				GUI.color = new Color ( 0, 1, 0 );
// 				GUI.Button ( rect, gameObject.name );
// 				// 				GUI.color = Color.white;
// 				// 				GUI.Label ( rect, obj.name );
// 			}
// 			else
			{
				/* gameObject.name = */ EditorGUI.TextField ( rect, gameObject.name );
				//GUI.Button ( rect, gameObject.name );
			}
		}
		Handles.EndGUI ();

		Event e = Event.current;
		switch ( e.type )
		{
			case EventType.Layout:
			case EventType.Repaint:
				break;
			default:
				e.Use ();
				break;
		}

	}

	private void OnSelectionChange ()
	{
		Repaint ();
	}

	private void OnGUI ()
	{
		// if ( Selection.gameObjects.Length > 0 )
		{
			GUILayout.Space ( 10 );
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			isEnabled = GUILayout.Toggle ( isEnabled, "Display Labels on scene objects" );
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
			GUILayout.Space ( 10 );
			if ( GUI.changed )
				RepaintGameView ();

			if ( isEnabled )
			{
				// unlabelSelected = GUILayout.Toggle ( unlabelSelected, "Unlabel selected" );

				isFilteredByTypes = GUILayout.Toggle ( isFilteredByTypes, "Restrict to types..." );
				if ( GUI.changed )
					RepaintGameView ();

				isFilteringByInstanceList = GUILayout.Toggle ( isFilteringByInstanceList, "Restrict to instances" );
				if ( GUI.changed )
					RepaintGameView ();

				// GUI.color = isActivated ? Color.yellow : Color.white;
				if ( isFilteringByInstanceList )
				{
					if ( Selection.gameObjects.Length > 0 )
					{
						GUILayout.BeginHorizontal ();

						GUILayout.Label ( "", GUILayout.Width ( 20 ) );
						GUILayout.Label ( "Selection", GUILayout.Width ( 60 ) );

						if ( GUILayout.Button ( "Add To List", GUILayout.Width ( 120 ) ) )
						{
							for ( int i = 0; i < Selection.gameObjects.Length; i++ )
							{
								GameObject go = Selection.gameObjects[i];
								if ( !listOfInstances.Contains ( go ) )
									listOfInstances.Add ( go );
							}
							if ( GUI.changed )
								RepaintGameView ();
						}

						if ( GUILayout.Button ( "Remove From List", GUILayout.Width ( 120 ) ) )
						{
							for ( int i = 0; i < Selection.gameObjects.Length; i++ )
							{
								GameObject go = Selection.gameObjects[i];
								listOfInstances.Remove ( go );
							}
							if ( GUI.changed )
								RepaintGameView ();
						}

						GUILayout.EndHorizontal ();
					}

					if ( listOfInstances.Count > 0 )
					{
						GUILayout.BeginHorizontal ();

						GUILayout.Label ( "", GUILayout.Width ( 20 ) );
						
						isInstanceListFoldedOut = EditorGUILayout.Foldout ( isInstanceListFoldedOut, "Instances" );
						if ( isInstanceListFoldedOut )
						{
							string list = "";
							for ( int i = 0; i < listOfInstances.Count; i++ )
							{
								list += ( ( i > 0 ) ? "\n\r" : "" ) + listOfInstances[i].name;
							}

							GUILayout.Label ( list );
						}

						GUILayout.EndHorizontal ();
					}
				}
			}

			// GUI.color = Color.white;
		}
	}

	private void RepaintGameView ()
	{
		System.Reflection.Assembly assembly = typeof ( UnityEditor.EditorWindow ).Assembly;
		System.Type type = assembly.GetType ( "UnityEditor.SceneView" );
		EditorWindow gameview = EditorWindow.GetWindow ( type );
		gameview.Repaint ();

		this.Focus ();
	}
}
