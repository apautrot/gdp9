using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

using System.IO;

[ExecuteInEditMode]
public class SandboxWindow : EditorWindow
{
	[MenuItem ( "Window/Sandbox" )]
	static void Init ()
	{
		EditorWindow window = EditorWindow.GetWindow ( typeof ( SandboxWindow ) );
		window.titleContent = new GUIContent ( "Sandbox" );
		// window.minSize = new Vector2 ( 250, 150 );
		window.Show ();
	}

	void OnGUI ()
	{
// 		if ( GUILayout.Button ( "Test" ) )
// 		{
// 			System.Type inspectorType = System.Type.GetType ( "UnityEditor.InspectorWindow,UnityEditor" );
// 			if ( inspectorType != null )
// 			{
// 				EditorWindow window = EditorWindow.GetWindow ( inspectorType );
// 				if ( window != null )
// 				{
// 					EditorWindow inspector = (EditorWindow)EditorWindow.CreateInstance ( inspectorType );
// 					inspector.titleContent.text = "Inspector";
// 					inspector.Show ();
// 
// 					InspectorWindowHelper.set_IsLocked ( inspector, true );
// 					
// // 					// Cache previous selected gameObject
// // 					GameObject prevSelection = Selection.activeGameObject;
// //  
// // 					// Set the selection to GO we want to inspect
// // 					Selection.activeGameObject = null;
// // 
// // 					PropertyInfo isLocked = inspectorType.GetProperty ( "isLocked", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
// //  
// // 					// Invoke `isLocked` setter method passing 'true' to lock the inspector
// // 					isLocked.GetSetMethod().Invoke ( inspector, new object[] { true } );
// //  
// // 					// Finally revert back to the previous selection so that other inspectors continue to inspect whatever they were inspecting...
// // 					Selection.activeGameObject = prevSelection;
// 				}
// 			}
// 		}

		OnGUIRepeatTool ();
		OnGUIBuildTextures ();
	}

	//

	bool buildTexturesFold;

	void OnGUIBuildTextures()
	{
		GUILayout.BeginVertical ( "Box" );
		buildTexturesFold = EditorGUILayout.Foldout ( buildTexturesFold, "Build textures" );
		if ( buildTexturesFold )
		{
			EditorGUI.indentLevel++;
			if ( GUILayout.Button ( "Build textures" ) )
			{
				string[] pngs = Directory.GetFiles ( Application.dataPath, "*.png", SearchOption.AllDirectories );
				for ( int i = 0 ; i < pngs.Length; i++ )
					BuildTexture ( pngs[i] );

				string[] jpgs = Directory.GetFiles ( Application.dataPath, "*.jpg", SearchOption.AllDirectories );
				for ( int i = 0; i < jpgs.Length; i++ )
					BuildTexture ( jpgs[i] );

				string[] jpegs = Directory.GetFiles ( Application.dataPath, "*.jpeg", SearchOption.AllDirectories );
				for ( int i = 0; i < jpegs.Length; i++ )
					BuildTexture ( jpegs[i] );

				AssetDatabase.Refresh ();
			}

			EditorGUI.indentLevel--;
		}
		GUILayout.EndVertical ();
	}

	void BuildTexture ( string sourcePathname )
	{
		string resourcesPath = Application.dataPath + "/Resources";
		string targetPathname = resourcesPath + "/" + Path.GetFileName ( sourcePathname );
		File.Copy ( sourcePathname, targetPathname + ".bytes", true );

		if ( sourcePathname.StartsWith ( Application.dataPath ) )
		{
			string assetRelativeSourcePathname = sourcePathname.Substring ( Application.dataPath.Length );
			assetRelativeSourcePathname = assetRelativeSourcePathname.Replace ( '\\', '/' );
			assetRelativeSourcePathname = "Assets" + assetRelativeSourcePathname;
			Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D> ( assetRelativeSourcePathname );
			if ( texture != null )
			{
				// Debug.Log ( "Texture " + assetRelativeSourcePathname + " : " + ( ( texture != null ) ? "yes" : "no" ) );

				string metadataFilePathname = targetPathname + ".meta";
				Debug.Log ( "- " + metadataFilePathname );

				TextureManager.SaveTextureMetadata ( texture, metadataFilePathname );
			}
		}

	}

	

	//

	bool repeatFold;
	int repeatCount = 0;
	Vector3 repeatDisp;

	void OnGUIRepeatTool ()
	{
		GUILayout.BeginVertical ( "Box" );
		repeatFold = EditorGUILayout.Foldout ( repeatFold, "Repeat selection" );
		if ( repeatFold )
		{
			EditorGUI.indentLevel++;
			GUI.enabled = ( Selection.activeGameObject != null );

			repeatCount = EditorGUILayout.IntField ( "Repeat", repeatCount );
			repeatDisp = EditorGUILayout.Vector3Field ( "Displacement", repeatDisp );
			if ( GUILayout.Button ( "Generate" ) )
			{
				GameObject go = Selection.activeGameObject;
				GameObject previous = go;
				for ( int i = 0; i < repeatCount; i++ )
				{
					GameObject created = GameObjectExtensions.Instantiate ( go, repeatDisp, go.name );
					created.transform.parent = go.transform.parent;
					created.transform.position = go.transform.position + ( repeatDisp * ( i + 1 ) );
					
					HingeJoint2D hinge = created.GetComponent<HingeJoint2D>();
					if ( hinge != null )
					{
						hinge.connectedBody = previous.GetComponent<Rigidbody2D> ();
						hinge.anchor = - repeatDisp;
					}

					previous = created;
				}
			}

			GUI.enabled = true;
			EditorGUI.indentLevel--;
		}
		GUILayout.EndVertical ();
	}

	void OnSelectionChange ()
	{
		Repaint ();
	}
}


public static class Test
{

	[MenuItem ( "GameObject/Open In Inspector", false, 0 )]
	static void Init ()
	{
		System.Type inspectorType = System.Type.GetType ( "UnityEditor.InspectorWindow,UnityEditor" );
		if ( inspectorType != null )
		{
			EditorWindow inspector = (EditorWindow)EditorWindow.CreateInstance ( inspectorType );
			inspector.titleContent.text = "Inspector";
			inspector.Show ();

			InspectorWindowHelper.set_IsLocked ( inspector, true );
		}
	}
}


class InspectorWindowHelper
{
	internal static EditorWindow GetInspectorWindow ()
	{
// 		System.Type inspectorType = System.Type.GetType ( "UnityEditor.InspectorWindow,UnityEditor" );
// 
// 		FieldInfo currentInspectorFieldInfo = inspectorType.GetField ( "s_CurrentInspectorWindow", BindingFlags.Public | BindingFlags.Static );
// 
// 		object o = currentInspectorFieldInfo.GetValue ( null );
// 		return o as EditorWindow;

		System.Type inspectorType = System.Type.GetType ( "UnityEditor.InspectorWindow,UnityEditor" );
		return EditorWindow.GetWindow ( inspectorType );
	}

	internal static bool get_IsLocked ( EditorWindow window )
	{
		System.Type inspectorType = System.Type.GetType ( "UnityEditor.InspectorWindow,UnityEditor" );

		PropertyInfo isLocked = inspectorType.GetProperty ( "isLocked", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );

		System.Object result = isLocked.GetGetMethod ().Invoke ( window, new object[] { } );

		return ( (bool)result == true );
	}

	internal static void set_IsLocked ( EditorWindow window, bool value )
	{
		System.Type inspectorType = System.Type.GetType ( "UnityEditor.InspectorWindow,UnityEditor" );

		PropertyInfo isLocked = inspectorType.GetProperty ( "isLocked", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );

		isLocked.GetSetMethod ().Invoke ( window, new object[] { value } );
	}
}