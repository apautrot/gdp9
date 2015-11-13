using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class NinePatch : MonoBehaviour
{
	public Color Color = Color.white;
	public float Width = 3;
	public float Height = 3;
	public float Left = 1;
	public float Top = 1;
	public float Right = 1;
	public float Bottom = 1;
	public SpriteAlignment Alignment;
	public Texture Texture;

	void Awake ()
	{
		Recreate ();
	}

	public void Recreate ()
	{
		MeshFilter filter = gameObject.GetOrCreateComponent<MeshFilter> ();
		MeshRenderer renderer = gameObject.GetOrCreateComponent<MeshRenderer> ();

		// if ( ( renderer.sharedMaterial == null ) || ( ( renderer.sharedMaterial.mainTexture != Texture ) && ( Texture != null ) ) )
		{
			Shader shader = Shader.Find ( "Sprites/Default" );
			if ( shader != null )
			{
				Material material = new Material ( shader );
				material.hideFlags = HideFlags.DontSave;
				material.mainTexture = Texture;
				renderer.sharedMaterial = material;
			}
		}

		float textureWidth = 0;
		float textureHeight = 0;

		if ( renderer.sharedMaterial.mainTexture != null )
		{
			textureWidth = renderer.sharedMaterial.mainTexture.width;
			textureHeight = renderer.sharedMaterial.mainTexture.height;
		}

		Width = Mathf.Max ( Width, Left + Right );
		Height = Mathf.Max ( Height, Top + Bottom );

		// if ( Application.isEditor )
		{
			Mesh mesh = MeshUtil.CreateNinePatch ( null, Width, Height );
			mesh.hideFlags = HideFlags.DontSave;

			MeshUtil.SetMeshColor ( mesh, Color );
			MeshUtil.SetNinePatchXYs ( mesh, Width, Height, Left, Right, Top, Bottom, Alignment );
			
			float leftUv = Left / textureWidth;
			float rightUv = Right / textureWidth;
			float topUv = Top / textureHeight;
			float bottomUv = Bottom / textureHeight;
			MeshUtil.SetNinePatchUVs ( mesh, leftUv, rightUv, topUv, bottomUv );
			
			filter.mesh = mesh;
		}
	}
}
