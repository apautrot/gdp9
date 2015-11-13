using UnityEngine;
using System.Collections;



[ExecuteInEditMode]
public class MegaSprite : MonoBehaviour
{
	[SerializeField]
	private MegaSpriteImage _megaSpriteImage = null;

// 	public MegaSpriteImage MegaSpriteImage
// 	{
// 		get { return _megaSpriteImage; }
// 		set
// 		{
// 			_megaSpriteImage = value;
// 			megaSpriteRenderer.MegaSpriteImage = value;
// 		}
// 	}

	[SerializeField]
	private Color _color = UnityEngine.Color.white;

	public Color Color
	{
		get { return _color; }
		set
		{
			_color = value;
			megaSpriteRenderer.Color = value;
		}
	}

	[SerializeField]
	private MegaSpriteRenderer megaSpriteRenderer = new MegaSpriteRenderer ();
	public MegaSpriteRenderer Renderer
	{
		get { return megaSpriteRenderer; }
	}

	void Awake ()
	{
		ForceRecreate ();
	}

	void Restart ()
	{
		ForceRecreate ();
	}

	public void ForceRecreate ()
	{
// 		megaSpriteRenderer.MegaSpriteImage = _megaSpriteImage;
// 		megaSpriteRenderer.Color = Color;

		UpdateGeometry ();
	}

	public void UpdateGeometry ()
	{
		MeshFilter filter = gameObject.GetOrCreateComponent<MeshFilter> ();
		MeshRenderer renderer = gameObject.GetOrCreateComponent<MeshRenderer> ();

		if ( _megaSpriteImage != null )
		{
			Shader shader = Shader.Find ( "Sprites/Default" );
			if ( shader != null )
			{
				Material material = new Material ( shader );
				material.hideFlags = HideFlags.DontSave;
				renderer.sharedMaterial = material;
			}

			filter.mesh = megaSpriteRenderer.Mesh;

// 			if ( Application.isEditor && !Application.isPlaying )
// 			{
// 				filter.sharedMesh = megaSpriteRenderer.Mesh;
// //				renderer.sharedMaterial = _megaSpriteImage.Material;
// 			}
// 			else
// 			{
// 				filter.mesh = megaSpriteRenderer.Mesh;
// 
// // 				if ( renderer.sharedMaterial == null )
// // 					renderer.sharedMaterial = Font.material;
// 			}
		}
		else
			filter.mesh = null;
	}

	private void OnWillRenderObject ()
	{
#if UNITY_EDITOR
		if ( Application.isEditor && !Application.isPlaying )
			return;
#endif
		if ( megaSpriteRenderer.IsDirty () )
			UpdateGeometry ();
	}
}
