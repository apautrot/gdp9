using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ReplaceSelectionWindow : EditorWindow
{
	[MenuItem ( "Window/Replace Selection" )]
	static void Init ()
	{
		EditorWindow window = EditorWindow.GetWindow ( typeof ( ReplaceSelectionWindow ) );
		window.titleContent = new GUIContent ( "Replace Selection" );
		window.minSize = new Vector2 ( 250, 150 );
		window.Show ();
	}

	void OnEnable ()
	{
		// LoadSettings ();
		// ApplySettings ();
	}

	void OnSelectionChange ()
	{
 		Repaint ();
	}

	List<GameObject> list = new List<GameObject> ();
	GameObject replaceObject;

	void OnGUI ()
	{
		EditorGUILayout.Space ();

		EditorGUILayout.LabelField ( "Replace what ?" );

		CenteredLabel ( list.Count.ToString () + " object(s) selected" );

		GUI.enabled = ( Selection.gameObjects.Length > 0 );
		if ( CenteredButton ( "From selection" ) )
		{
			list.Clear();

			foreach ( GameObject go in Selection.gameObjects )
			{
				bool isPrefab = PrefabUtility.GetPrefabParent ( go ) == null && PrefabUtility.GetPrefabObject ( go ) != null;
				if ( !isPrefab )
					list.Add ( go );
			}

			Repaint ();
		}
		GUI.enabled = true;

		EditorGUILayout.Space ();

		EditorGUILayout.LabelField ( "Replace with ", GUILayout.Width ( 80 ) );

		CenteredLabel ( replaceObject != null ? replaceObject.name : "(none)" );

		GUI.enabled = Selection.activeGameObject != null;
		if ( CenteredButton ( "From selection" ) )
		{
			replaceObject = Selection.activeGameObject;
			Repaint();
		}
		GUI.enabled = true;

		if ( CenteredButton ( "Pick" ) )
		{ }

		EditorGUILayout.Space ();
		GUI.enabled = ( ( list.Count > 0 ) && ( replaceObject != null ) );
		if ( CenteredButton ( "Replace selection" ) )
		{ }
		GUI.enabled = true;
	}

	void CenteredLabel ( string text, float width = 150 )
	{
		EditorGUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		EditorGUILayout.LabelField ( text, GUILayout.Width ( width ) );
		GUILayout.FlexibleSpace ();
		EditorGUILayout.EndHorizontal ();
	}

	bool CenteredButton ( string text, float width = 150 )
	{
		EditorGUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		bool isClicked = GUILayout.Button ( text, GUILayout.Width ( width ) );
		GUILayout.FlexibleSpace ();
		EditorGUILayout.EndHorizontal ();

		return isClicked;
	}
}






static class ReplaceSelection
{
	static private GameObject source;

	[MenuItem ( "Edit/Replace selection/Copy selection as source", true, 1101 )]
	static bool SelectObject_Validate ()
	{
		return Selection.gameObjects.Length != 0;
	}

	[MenuItem ( "Edit/Replace selection/Copy selection as source", false, 1101 )]
	static void SelectObject_Command ()
	{
		source = null;

		if ( Selection.gameObjects.Length != 0 )
		{
			if ( Selection.gameObjects.Length == 1 )
				source = Selection.gameObjects[0];
			else
				Debug.LogWarning ( "Select only one object as a source for Replace selection" );
		}
	}

	[MenuItem ( "Edit/Replace selection/Replace selection", true, 1100 )]
	static bool ReplaceSelection_Validate ()
	{
		return ( source != null );
	}
	
	[MenuItem ( "Edit/Replace selection/Replace selection", false, 1100 )]
	static void ReplaceSelection_Command ()
	{
		List<GameObject> newSelection = new List<GameObject> ();

		foreach ( GameObject s in Selection.gameObjects )
		{
			if ( s != source )
			{
				GameObject go = GameObject.Instantiate ( source ) as GameObject;
				go.transform.parent = s.transform.parent;
				go.transform.position = s.transform.position;
				go.name = s.name;

				newSelection.Add ( go );

				GameObject.DestroyImmediate ( s );
			}
		}

		Selection.objects = newSelection.ToArray ();
	}
}