using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class BackTransforms : EditorWindow
{
	[MenuItem ( "Window/Back Transforms" )]
	static void Init ()
	{
		EditorWindow window = EditorWindow.GetWindow ( typeof ( BackTransforms ) );
		window.titleContent = new GUIContent ( "Back Transforms" );
		window.minSize = new Vector2 ( 250, 50 );
		window.Show ();
	}

	void OnSelectionChange ()
	{
		Repaint ();
	}

	void OnGUI ()
	{
		GUI.enabled = ( Selection.gameObjects.Length > 0 );
		
		if ( GUILayout.Button ( "Copy Transforms" ) )
			CopyTransforms ();

		if ( GUILayout.Button ( "Restore Transforms" ) )
			PasteTransforms ();
	}

	struct TransformData
	{
		internal Vector3 localPosition;
		internal Vector3 localScale;
		internal Quaternion localRotation;
	}

	Dictionary<int,TransformData> backedTransforms = new Dictionary<int,TransformData>();

	void CopyTransforms ()
	{
		backedTransforms.Clear ();

		for ( int i = 0 ; i < Selection.gameObjects.Length; i++ )
		{
			GameObject go = Selection.gameObjects[i];
			Transform t = go.transform;
			int id = go.GetInstanceID();
			TransformData data = new TransformData ()
			{
				localPosition = t.localPosition,
				localScale = t.localScale,
				localRotation = t.localRotation
			};

			backedTransforms.Add ( id, data );
		}

		Debug.Log ( "Copied " + backedTransforms.Count + " transforms." );
	}

	void PasteTransforms ()
	{
		int count = 0;

		for ( int i = 0; i < Selection.gameObjects.Length; i++ )
		{
			GameObject go = Selection.gameObjects[i];
			int id = go.GetInstanceID();
			TransformData data;
			if ( backedTransforms.TryGetValue ( id, out data ) )
			{
				Transform t = go.transform;
				t.localPosition = data.localPosition;
				t.localScale = data.localScale;
				t.localRotation = data.localRotation;

				count++;
			}
		}

		Debug.Log ( "Restored " + count + " transforms." );
	}
}