using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class ConversionWindow : EditorWindow
{
	private Vector2 _scroll;

	// To have rich text
	private GUIStyle _missingLabelStyle;
	private GUIStyle _missingFoldoutStyle;

	private bool _sceneFoldout = false;
	private bool _projectFoldout = false;

	private List<GameObject> _sceneList = null;
	private List<GameObject> _projectList = null;

	[MenuItem ( "Window/Conversion Window" )]
	static void Init ()
	{
		ConversionWindow window = (ConversionWindow)EditorWindow.GetWindow ( typeof ( ConversionWindow ) );
		window.name = "Conversion pf MonoBehavior";
	}

	void OnGUI ()
	{
		if ( GUILayout.Button ( "SCAN", GUILayout.Height ( 30f ) ) )
			Scan ();

		_scroll = GUILayout.BeginScrollView ( _scroll );
		{
			_missingLabelStyle = _missingLabelStyle ?? new GUIStyle ( GUI.skin.label );
			_missingLabelStyle.richText = true;
			_missingFoldoutStyle = _missingFoldoutStyle ?? new GUIStyle ( EditorStyles.foldout );
			_missingFoldoutStyle.richText = true;

			if ( ( _sceneList != null && _sceneList.Count > 0 ) || ( _projectList != null && _projectList.Count > 0 ) )
			{
				_sceneFoldout = EditorGUILayout.Foldout ( _sceneFoldout, "Scene GameObjects" );
				if ( _sceneFoldout )
					DrawGameObjectList ( _sceneList );

				_projectFoldout = EditorGUILayout.Foldout ( _projectFoldout, "Project GameObjects" );
				if ( _projectFoldout )
					DrawGameObjectList ( _projectList );
			}
			else
			{
				GUILayout.Label ( "Hit scan" );
			}
		}
		GUILayout.EndScrollView ();
	}

	/// <summary>
	/// Scan for every game object inside the scene AND the project folder
	/// </summary>
	private void Scan ()
	{
// 		_sceneList = ( FindObjectsOfType ( typeof ( GameObject ) ) as GameObject[] ).ToList ();
// 		foreach ( GameObject go in _sceneList )
// 		{
// 		}
// 
// 		_projectList = new List<GameObject> ();
// 		LoadAllPrefabs ( ref _projectList, Application.dataPath );
// 		foreach ( GameObject go in _projectList )
// 		{
// 		}
	}

	/// <summary>
	/// Load every prefabs recursively from the asset folder
	/// </summary>
	/// <param name="prefabs">List of prefab being updated by the recursive function</param>
	/// <param name="path">Current path</param>
	private void LoadAllPrefabs ( ref List<GameObject> prefabs, string path )
	{
		string[] directories = System.IO.Directory.GetDirectories ( path );
		foreach ( string directorie in directories )
			LoadAllPrefabs ( ref prefabs, directorie );

		path = path.Replace ( '\\', '/' );
		path = "Assets" + path.Substring ( Application.dataPath.Length ) + "/";
		string[] assetsPath = System.IO.Directory.GetFiles ( path, "*.prefab" );
		foreach ( string assetPath in assetsPath )
		{
			GameObject asset = AssetDatabase.LoadAssetAtPath ( assetPath, typeof ( GameObject ) ) as GameObject;
			if ( asset != null && PrefabUtility.GetPrefabType ( asset ) != PrefabType.None )
				prefabs.Add ( asset );
		}
	}

	/// <summary>
	/// Display the list of game objects inside the window vertically. If a component is missing, 
	/// the label [Missing script] is drawn in red in front of it.
	/// </summary>
	/// <param name="list">List of game object to display</param>
	private void DrawGameObjectList ( List<GameObject> list )
	{
		if ( list != null )
		{
			// GameObject destroyGO = null;

			foreach ( GameObject obj in list )
			{
				if ( obj == null )
					continue;

				GUILayout.BeginHorizontal ();
				{
					GUILayout.Space ( 20f );

					// Selectable, read-only field
					EditorGUILayout.ObjectField ( obj, obj.GetType (), true, GUILayout.ExpandWidth ( true ) );

					// Delete obj, but only after the loop
					if ( GUILayout.Button ( new GUIContent ( "X", "Delete" ), GUILayout.Width ( 20f ) ) )
					{
// 						if ( EditorUtility.DisplayDialog ( "Exterminate !", "Are you sure you want to delete " + obj.name + " ?", "Yes", "No" ) )
// 							destroyGO = obj;
					}

					// Select obj in hierarchy
					if ( GUILayout.Button ( new GUIContent ( "S", "Select" ), GUILayout.Width ( 20f ) ) )
						Selection.activeGameObject = obj;
				}
				GUILayout.EndHorizontal ();

			}

// 			if ( destroyGO != null )
// 				GameObject.DestroyImmediate ( destroyGO );
		}
	}
}