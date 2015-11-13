using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor ( typeof ( MegaSprite ) ), CanEditMultipleObjects]
public class MegaSpriteInspector : Editor
{
	[MenuItem ( "GameObject/Create Other/Mega Sprite", false, 5 )]
	public static void CreateMegaSprite ()
	{
		GameObject go = new GameObject ();
		go.name = "Text";

		if ( Selection.activeGameObject != null )
		{
			go.transform.parent = Selection.activeGameObject.transform;
			go.transform.position = Selection.activeGameObject.transform.position;
		}
		Selection.activeObject = go;
	}



	SerializedProperty Texture;

	void OnEnable ()
	{
		SerializedProperty megaSpriteImageTexture = serializedObject.FindProperty ( "_megaSpriteImage" );
		if ( megaSpriteImageTexture != null )
		{
			SerializedObject propObj = new SerializedObject ( megaSpriteImageTexture.objectReferenceValue );
			Texture = propObj.FindProperty ( "Texture" );
		}
	}

	public override void OnInspectorGUI ()
	{
		this.DrawDefaultInspector ();
		bool propertyChanged = GUI.changed;

		// MegaSpriteImage msi = target as MegaSpriteImage;

		if ( Texture != null )
		{
			GUI.changed = false;
			EditorGUILayout.PropertyField ( Texture );
			if ( GUI.changed )
				Texture.serializedObject.ApplyModifiedProperties ();
		}

		UnityToolbag.SortingLayerExposedEditor.DrawSortingLayerGUI ( target, targets );

		if ( propertyChanged )
		{
			for ( int i = 0; i < targets.Length; i++ )
			{
				MegaSprite ms = targets[i] as MegaSprite;
				ms.ForceRecreate ();
			}
		}
	}
}