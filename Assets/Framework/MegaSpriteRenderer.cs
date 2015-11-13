using UnityEngine;
using System.Collections;

[System.Serializable]
public class MegaSpriteRenderer
{
	private bool quadBatchUpdateNeeded = true;
	private bool meshUpdateNeeded = true;

	protected QuadBatch quadBatch = new QuadBatch ();
	protected Mesh mesh;

	[SerializeField]
	public string MegaSpritePathname;

// 	MegaSpriteImage megaSpriteImage;
// 	public MegaSpriteImage MegaSpriteImage
// 	{
// 		get { return megaSpriteImage; }
// 		set
// 		{
// 			megaSpriteImage = value;
// 			quadBatchUpdateNeeded = true;
// 		}
// 	}

	Color color;
	public Color Color
	{
		get { return color; }
		set
		{
			color = value;
			quadBatch.colorize ( 0, quadBatch.Count, color );
			meshUpdateNeeded = true;
		}
	}



	public QuadBatch QuadBatch
	{
		get
		{
			RecreateQuadBatchIfNeeded ();
			return quadBatch;
		}
		set
		{
			quadBatch = value;
			meshUpdateNeeded = true;
		}
	}

	private void RecreateQuadBatchIfNeeded ()
	{
		if ( quadBatchUpdateNeeded )
		{
			RecreateQuadBatch ();
			meshUpdateNeeded = true;
			quadBatchUpdateNeeded = false;
		}
	}

	public void RecreateQuadBatch ()
	{
		quadBatch.setSize ( 1 );
		quadBatch.setQuad ( 0, Vector3.zero, Vector2.one, Vector2.zero, Vector2.one, Color.white );


		// ...
	}

	public bool IsDirty ()
	{
		return ( meshUpdateNeeded || quadBatchUpdateNeeded );
	}

	public Mesh Mesh
	{
		get
		{
// 			if ( ( megaSpriteImage != null ) && megaSpriteImage.RecreateIfNeeded() )
// 				quadBatchUpdateNeeded = true;

			if ( quadBatchUpdateNeeded )
				RecreateQuadBatchIfNeeded ();

			if ( meshUpdateNeeded )
			{
				mesh = QuadBatch.CreateMesh ( mesh );
				if ( mesh != null )
					mesh.hideFlags = HideFlags.DontSave;
				meshUpdateNeeded = false;
			}

			return mesh;
		}
	}

}
