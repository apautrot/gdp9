#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public sealed class CutOutParameter : MonoBehaviour
{
	[SerializeField]
	[RangeAttribute(0,1)]
	private float _CutOut;

	public Texture2D Texture;

	new Renderer renderer;

	public CutOutParameter ()
	{
	}

	void Awake ()
	{
		renderer = GetComponent<Renderer> ();
		UpdateCutOutParameter ();
		UpdateCutOutTexture ();
	}

	void Reawake ()
	{
		Awake ();
	}

	public float CutOut
	{
		get { return _CutOut; }
		set
		{
			_CutOut = value;
			UpdateCutOutParameter ();
		}
	}

	public void UpdateCutOutParameter ()
	{
#if UNITY_EDITOR
		if ( Application.isEditor && !Application.isPlaying )
			if ( renderer == null )
				renderer = GetComponent<Renderer>();
#endif

		if ( renderer != null )
		{
			Material material = null;
#if UNITY_EDITOR
			if ( PrefabUtility.GetPrefabType ( gameObject ) == PrefabType.Prefab )
				material = renderer.sharedMaterial;
			else
#endif
				material = renderer.material;

			if ( material != null )
				material.SetFloat ( "_CutOut", CutOut );
		}
		else
			Debug.Log ( "No renderer" );
	}

	public void UpdateCutOutTexture ()
	{
#if UNITY_EDITOR
		if ( Application.isEditor && !Application.isPlaying )
			if ( renderer == null )
				renderer = GetComponent<Renderer> ();
#endif

		if ( renderer != null )
		{
			Material material = null;
#if UNITY_EDITOR
			if ( PrefabUtility.GetPrefabType ( gameObject ) == PrefabType.Prefab )
				material = renderer.sharedMaterial;
			else
#endif
				material = renderer.material;
			if ( material != null )
				material.SetTexture ( "_CutTex", Texture );
		}
		else
			Debug.Log ( "No renderer" );
	}
}