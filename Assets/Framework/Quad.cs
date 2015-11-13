using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent ( typeof ( MeshFilter ), typeof ( MeshRenderer ) )]
public class Quad : MonoBehaviour
{
	public Color TopLeftColor = Color.gray;
	public Color TopRightColor = Color.gray;
	public Color BottomLeftColor = Color.gray;
	public Color BottomRightColor = Color.gray;
	public Color Color
	{ set { TopLeftColor = TopRightColor = BottomLeftColor = BottomRightColor = value; } }
	public int Width = 1;
	public int Height = 1;
	public SpriteAlignment Alignment;

// 	private Color color;
// 	public Color Color
// 	{
// 		get
// 		{
// 			return color;
// 		}
// 		set
// 		{
// 			if ( color != value )
// 			{
// 				color = value;
// 				MeshFilter filter = gameObject.GetComponent<MeshFilter> ();
// 				MeshUtil.SetMeshColor ( filter.mesh, color );
// 			}
// 		}
// 	}

	void Awake ()
	{
		BuildMeshIfNeeded ();
	}

	void BuildMeshIfNeeded ()
	{
		MeshFilter meshFilter = GetComponent<MeshFilter> ();
		Mesh mesh = meshFilter.sharedMesh;
		if ( mesh == null )
			BuildMesh ();
	}

#if UNITY_EDITOR
	void Update ()
	{
		if ( !Application.isPlaying )
			BuildMeshIfNeeded ();
	}
#endif

	public void BuildMesh ()
	{
		MeshFilter filter = gameObject.GetComponent<MeshFilter> ();
		MeshRenderer renderer = gameObject.GetComponent<MeshRenderer> ();

		if ( renderer.sharedMaterial == null )
		{
			Shader shader = Shader.Find ( "Sprites/Default" );
			if ( shader != null )
			{
				Material material = new Material ( shader );
				material.hideFlags = HideFlags.DontSave;
				renderer.sharedMaterial = material;
			}
		}

//		if ( ( Width > 0 ) && ( Height > 0 ) )
		// if ( Application.isEditor )
		{
			Mesh mesh = MeshUtil.CreateQuad ( null, Width, Height, Alignment, TopLeftColor, TopRightColor, BottomLeftColor, BottomRightColor );
			mesh.hideFlags = HideFlags.DontSave;
			filter.mesh = mesh;
		}
	}
}
