#if UNITY_EDITOR && DONTCOMPILE
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SkyboxRenderer : MonoBehaviour
{
	static int faceSize = 512;
	static string directory = "Assets/Map/Skybox images";

	static string[] skyBoxImage = new string[] { "front", "left", "back", "right", "top", "bottom" };

	static Vector3[] skyDirection = new Vector3[] { new Vector3 ( 0, 0, 0 ), new Vector3 ( 0, -90, 0 ), new Vector3 ( 0, 180, 0 ), new Vector3 ( 0, 90, 0 ), new Vector3 ( -90, 0, 0 ), new Vector3 ( 90, 0, 0 ) };

	public void RenderToTextures()
	{
		if ( !Directory.Exists ( directory ) )
			Directory.CreateDirectory ( directory );

		string name = System.IO.Path.GetFileNameWithoutExtension ( EditorApplication.currentScene );
		StartCoroutine ( RenderSkyboxTo6PNG ( name, Vector3.zero ) );
	}

	IEnumerator RenderSkyboxTo6PNG ( string name, Vector3 position )
	{
		Camera camera = Camera.main;

		camera.fieldOfView = 90;
		camera.aspect = 1.0f;

		camera.gameObject.transform.position = position;
		camera.gameObject.transform.rotation = Quaternion.identity;

		//Render skybox        
		for ( int orientation = 0; orientation < skyDirection.Length; orientation++ )
		{
			yield return new WaitForEndOfFrame ();

			string assetPath = System.IO.Path.Combine ( directory, name + "_" + skyBoxImage[orientation] + ".png" );
			RenderSkyBoxFaceToPNG ( orientation, camera, assetPath );
		}

        UnityEditor.EditorApplication.isPlaying = false;

		yield break;
	}

	static void RenderSkyBoxFaceToPNG ( int orientation, Camera cam, string assetPath )
	{
// 		cam.transform.eulerAngles = skyDirection[orientation];
// 		RenderTexture rt = new RenderTexture ( faceSize, faceSize, 24 );
// 		cam.camera.targetTexture = rt;
// 		cam.camera.Render ();
// 		RenderTexture.active = rt;
// 
// 		Texture2D screenShot = new Texture2D ( faceSize, faceSize, TextureFormat.RGB24, false );
// 		screenShot.ReadPixels ( new Rect ( 0, 0, faceSize, faceSize ), 0, 0 );
// 
// 		RenderTexture.active = null;
// 		GameObject.DestroyImmediate ( rt );

		cam.transform.eulerAngles = skyDirection[orientation];
		cam.GetComponent<Camera>().Render ();

		Texture2D screenShot = new Texture2D ( Screen.width, Screen.height, TextureFormat.RGB24, false );
		screenShot.ReadPixels ( new Rect ( 0, 0, Screen.width, Screen.height ), 0, 0 );
		screenShot.Apply ();

		// screenShot.Resize ( faceSize, faceSize, TextureFormat.ARGB32, false );
		TextureScale.Bilinear ( screenShot, faceSize, faceSize );

		byte[] bytes = screenShot.EncodeToPNG ();
		File.WriteAllBytes ( assetPath, bytes );

		AssetDatabase.ImportAsset ( assetPath, ImportAssetOptions.ForceUpdate );
	}

	public GameObject Dummy;

	private bool isDragging;
	private Quaternion originalRotation;
//  	private Vector3 rightAtStart;
//  	private Vector3 upAtStart;
	private Vector3 mousePositionAtStart;

	void FixedUpdate ()
	{
		if ( !TouchInput.InstanceCreated )
			TouchInput.CreateInstance ();

		if ( TouchInput.isDragging )
		{
			if ( !isDragging )
			{
				originalRotation = transform.rotation;
				mousePositionAtStart = Input.mousePosition;
//  				rightAtStart = transform.right;
//  				upAtStart = transform.up;
				isDragging = true;
			}

			if ( TouchInput.isJustDragged )
			{
				Vector3 pos = Camera.main.ScreenToViewportPoint ( Input.mousePosition - mousePositionAtStart );
				pos *= 100;

 				Quaternion q = originalRotation;
 				q *= Quaternion.Euler ( Vector3.right * -pos.y );
 				q *= Quaternion.Euler ( Vector3.up * pos.x );
				Vector3 target = q * new Vector3 ( 0, 0, 2000 );
				transform.LookAt ( transform.position + target, Vector3.up );

// 				transform.rotation = originalRotation;
// // 				transform.RotateAround ( transform.position, transform.right, -pos.y );
// // 				transform.RotateAround ( transform.position, transform.up, pos.x );
// 				transform.RotateAround ( transform.position, rightAtStart, -pos.y );
// 				transform.RotateAround ( transform.position, upAtStart, pos.x );

// 				if ( Dummy != null )
// 					Dummy.transform.position = target;
			}
		}
		else
		{
			isDragging = false;
		}
	}
}

#endif