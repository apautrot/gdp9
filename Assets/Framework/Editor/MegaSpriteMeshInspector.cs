using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor ( typeof ( MegaSpriteMesh ) ), CanEditMultipleObjects]
public class MegaSpriteMeshInspector : Editor
{
	[MenuItem ( "GameObject/Create Other/Mega Sprite Mesh (extended)", false, 5 )]
	public static void Create ()
	{
		GameObject go = new GameObject();
		go.name = "Text";

		if ( Selection.activeGameObject != null )
		{
			go.transform.parent = Selection.activeGameObject.transform;
			go.transform.position = Selection.activeGameObject.transform.position;
		}
		Selection.activeObject = go;
	}

	public override void OnInspectorGUI ()
	{
		this.DrawDefaultInspector ();

		if ( GUILayout.Button ( "Regenerate" ) )
		{
			( target as MegaSpriteMesh ).ForceRecreate ();
		}
	}
}