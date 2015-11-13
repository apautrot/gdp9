using UnityEngine;
using System.Collections;

using System.Diagnostics;
using System.IO;

public class MegaSpriteMesh : MonoBehaviour
{
	private bool reloadFromFileNeeded = true;
	private bool meshUpdateNeeded = true;

	protected Mesh mesh;
	public Texture texture;

	// EditorUtility.OpenFilePanel
	public string megaSpritePathname = "";

	public int TileSize = 16;

	int width;
	int height;

	int hCount;
	int vCount;

	int[,] tilemap;

	public bool IsDirty ()
	{
		return ( meshUpdateNeeded );
	}

	public Mesh Mesh
	{
		get
		{
			if ( reloadFromFileNeeded )
			{
				string sourceImagePathname = Application.dataPath + "/" + megaSpritePathname;
				string tilemapPathName = Path.GetDirectoryName ( sourceImagePathname ) + "\\" + Path.GetFileNameWithoutExtension ( sourceImagePathname ).Trim ( '.' ) + ".tilemap";

				using ( BinaryReader br = new BinaryReader ( new FileStream ( tilemapPathName, FileMode.Open ) ) )
				{
					hCount = br.ReadInt32 ();
					vCount = br.ReadInt32 ();
					tilemap = new int[hCount, vCount];

					width = TileSize * hCount;
					height = TileSize * vCount;

					for ( int j = 0; j < vCount; j++ )
						for ( int i = 0; i < hCount; i++ )
							tilemap[i, j] = br.ReadInt32 ();
				}

			}

			if ( meshUpdateNeeded )
			{
				mesh = MeshUtil.CreatePlane ( mesh, (int)width, (int)height, hCount, vCount );
				if ( mesh != null )
					mesh.hideFlags = HideFlags.DontSave;
				meshUpdateNeeded = false;
			}

			return mesh;
		}
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
		meshUpdateNeeded = true;
		UpdateGeometry ();
	}

	public void UpdateGeometry ()
	{
		MeshFilter filter = gameObject.GetOrCreateComponent<MeshFilter> ();
		MeshRenderer renderer = gameObject.GetOrCreateComponent<MeshRenderer> ();

		if ( megaSpritePathname != null )
		{
			Shader shader = Shader.Find ( "Sprites/Default" );
			if ( shader != null )
			{
				Material material = new Material ( shader );
				material.hideFlags = HideFlags.DontSave;
				renderer.sharedMaterial = material;
			}

			filter.mesh = Mesh;

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
// #if UNITY_EDITOR
// 		if ( Application.isEditor && !Application.isPlaying )
// 			return;
// #endif
		if ( IsDirty () )
			UpdateGeometry ();
	}

	void RecreateQuadBatch ()
	{
		// megaSpritePathname
		ProcessStartInfo psi = new ProcessStartInfo ();
		psi.Arguments = Application.dataPath + "/" + megaSpritePathname;
		psi.FileName = Application.dataPath + "/Third Party/.TilemapImage/imagetotilemap.exe";
		psi.WindowStyle = ProcessWindowStyle.Normal;
		psi.WorkingDirectory = Path.GetDirectoryName ( psi.FileName );
		Process p = Process.Start ( psi );
		p.WaitForExit ();


	}

	/*
	public void RecreateQuadBatch ()
	{
		quadBatch.setSize ( 1 );
		quadBatch.setQuad ( 0, Vector3.zero, Vector2.one, Vector2.zero, Vector2.one, Color.white );

		string inputPathName = megaSpritePathname;
		if ( !File.Exists ( inputPathName ) )
		{
			Debug.Log ( "Input path name does not exists : " + inputPathName );
			return;
		}

		const int tileSize = 8;

		Texture2D input = LoadImage ( inputPathName );

		if ( input.format != TextureFormat.ARGB32 )
			// input = input.Clone ( new Rectangle ( 0, 0, input.Width, input.Height ), PixelFormat.Format32bppArgb );
			input = input.ConvertToARGB32 ();

		bool inputHasAlpha = input.HasAlpha ();

		int hMod = ( input.width % tileSize );
		int vMod = ( input.height % tileSize );
		if ( ( hMod != 0 ) || ( vMod != 0 ) )
		{
			int width = input.width + ( ( hMod > 0 ) ? tileSize - hMod : 0 );
			int height = input.height + ( ( vMod > 0 ) ? tileSize - vMod : 0 );
			Texture2D resized = new Texture2D ( width, height, input.format, false );

			resized.DrawImageUnscaled ( input, 0, 0, 0, 0, input.width, input.height );

			input = resized;
		}

		int hCount = input.width / tileSize;
		int vCount = input.height / tileSize;

		int[,] tilemap = new int[hCount, vCount];

		for ( int j = 0; j < vCount; j++ )
			for ( int i = 0; i < hCount; i++ )
			{
				int x = i * tileSize;
				int y = j * tileSize;
				Texture2D tile = input.Copy ( x, y, tileSize, tileSize, input.format );

				int index = RegisterTileBitmap ( tile );
				tilemap[i, j] = index;

				// tile.Save ( "D:\\tile-" + index + ".png" );
			}

		int outputH = 1;
		int outputV = 1;
		int tileCountInOutput = tiles.Keys.Count;
		while ( ( outputH * outputV ) < tileCountInOutput )
		{
			if ( outputH <= outputV )
				outputH *= 2;
			else
				outputV *= 2;
		}


		int outputWidth = ( outputH * tileSize );
		int outputHeight = ( outputV * tileSize );

		Texture2D output = new Texture2D ( outputWidth, outputHeight, input.PixelFormat );
		output.SetResolution ( input.HorizontalResolution, input.VerticalResolution );
		Graphics outputGraphics = Graphics.FromImage ( output );
		outputGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
		for ( int i = 0; i < tileCountInOutput; i++ )
		{
			int x = ( i % outputH ) * tileSize;
			int y = ( i / outputH ) * tileSize;

			Tile tile = tiles[i];

			tile.XInOutputImage = x;
			tile.YInOutputImage = y;

			outputGraphics.DrawImageUnscaled ( tile.Bitmap, x, y, tileSize, tileSize );
		}
	}



	private class Tile
	{
		internal Tile ( Texture2D bitmap )
		{
			this.Bitmap = bitmap;
		}

		internal Texture2D Bitmap;
		internal int XInOutputImage;
		internal int YInOutputImage;
	}

	private static Dictionary<int, Tile> tiles = new Dictionary<int, Tile> ();

	private static int RegisterTileBitmap ( Texture2D bitmap )
	{
		foreach ( KeyValuePair<int, Tile> kvp in tiles )
			if ( bitmap.IsEqualTo ( kvp.Value.Bitmap ) )
				return kvp.Key;

		int index = tiles.Keys.Count;
		tiles.Add ( index, new Tile ( bitmap ) );
		return index;
	}
	*/
}


/*
public static class Texture2DExtensionMethods
{
	public static Texture2D ConvertToARGB32 ( this Texture2D self )
	{
		if ( self.format == TextureFormat.ARGB32 )
			return self;

		if ( self.format == TextureFormat.RGB24 )
		{
			Color32[] pixels = self.GetPixels32 ();
			for ( int i = 0; i < pixels.Length; i++ )
				pixels[i].a = 255;

			Texture2D clone = new Texture2D ( self.width, self.height, TextureFormat.ARGB32, false );
			clone.SetPixels32 ( pixels );
		}

		Debug.LogError ( "Don't know how to convert the texture format " + self.format.ToString () + " to ARGB32" );
		return null;
	}

	public static bool HasAlpha ( this Texture2D self )
	{
		Color32[] pixels = self.GetPixels32 ();
		for ( int i = 0; i < pixels.Length; i++ )
			if ( pixels[i].a < 255 )
				return true;

		return false;
	}

	public static void DrawImageUnscaled ( this Texture2D self, Texture2D image, int sx, int sy, int dx, int dy, int width, int height )
	{

	}

	public static Texture2D Copy ( this Texture2D self, int x, int y, int widht, int height, TextureFormat format )
	{
		return null;
	}

	public static bool IsEqualTo ( this Texture2D self, Texture2D other )
	{
		return true;
	}

	public static Texture2D LoadImage ( string filePath )
	{
		Texture2D tex = null;
		byte[] fileData;

		if ( File.Exists ( filePath ) )
		{
			fileData = File.ReadAllBytes ( filePath );
			tex = new Texture2D ( 1, 1 );
			tex.LoadImage ( fileData );
		}
		return tex;
	}
}
*/