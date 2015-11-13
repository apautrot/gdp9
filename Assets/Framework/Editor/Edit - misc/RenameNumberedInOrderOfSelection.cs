using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class RenameNumberedInOrderOfSelection : EditorWindow
{
	[MenuItem ( "Window/Rename In Order Of Selection", false, 1100 )]
    static void Init()
    {
		RenameNumberedInOrderOfSelection window = RenameNumberedInOrderOfSelection.GetWindow<RenameNumberedInOrderOfSelection> ();
		window.titleContent = new GUIContent ( "Rename with number" );
		window.maxSize = new Vector2 ( 250, 100 );
		window.minSize = new Vector2 ( 200, 100 );
    }

	string baseName = "";
	int baseNumber;
	bool isActivated;

	private void OnGUI ()
	{
		EditorGUILayout.BeginHorizontal ();
		{
			GUILayout.Label ( "Base name:", GUILayout.Width ( 100 ) );
			GUILayout.FlexibleSpace ();
			baseName = EditorGUILayout.TextField ( GUIContent.none, baseName, GUILayout.MinWidth ( 20 ) );
		}
		EditorGUILayout.EndHorizontal ();
		
		if ( GUI.changed )
			baseNumber = 0;

		EditorGUILayout.BeginHorizontal ();
		{
			GUILayout.Label ( "Next number:", GUILayout.Width ( 100 ) );
			GUILayout.FlexibleSpace ();
			baseNumber = EditorGUILayout.IntField ( GUIContent.none, baseNumber, GUILayout.MinWidth ( 20 ) );
		}
		EditorGUILayout.EndHorizontal ();

		GUI.color = isActivated ? Color.yellow : Color.white;
		if ( GUILayout.Button ( isActivated ? "Inactive" : "Active" ) )
			isActivated = !isActivated;
		GUI.color = Color.white;
	}

	private void OnSelectionChange ()
	{
		if ( isActivated )
		{
			GameObject[] selection = Selection.gameObjects;
			for ( int i = 0; i < selection.Length; i++ )
			{
				GameObject go = selection[i];
				go.name = baseName + " " + baseNumber;
			}

			baseNumber++;

			Repaint ();
		}
	}
}
