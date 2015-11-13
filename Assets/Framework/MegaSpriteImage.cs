using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class MegaSpriteImage : ScriptableObject
{
#if UNITY_EDITOR
	[MenuItem ( "Assets/Create/Mega Sprite Image" )]
	public static void CreatePacksSet ()
	{
		ScriptableObjectUtility.CreateAsset<MegaSpriteImage> ();
	}
#endif

	[SerializeField]
	private System.DateTime? LastWriteTime = null;

#if UNITY_EDITOR
	private System.DateTime? TextureLastWriteTime
	{
		get
		{
			if ( Texture != null )
				return File.GetLastWriteTime ( AssetDatabase.GetAssetPath ( Texture ) );
			else
				return null;
		}
	}
#endif

	public Texture Texture;

	public int[,] tilemap;

#if UNITY_EDITOR
	internal bool RecreateIfNeeded ()
	{
		if ( LastWriteTime != TextureLastWriteTime )
		{
			// LastWriteTime = TextureLastWriteTime;

			if ( MegaSpriteImageCompiler.Instance == null )
				Debug.LogError ( "Can't find an instance of MegaSpriteImageCompiler" );
			else
				MegaSpriteImageCompiler.Instance.Compile ( this );

			return true;
		}
		else
			return false;
	}



		/*
		if ( Texture != null )
		{
			
		}
		else
		{
			tilemap = new int[0, 0];
		}
		*/

#endif

}





/*
public static class BitmapExtensionsMethods
{
	public unsafe static bool IsEqualTo ( this Bitmap self, Bitmap compared )
	{
		if ( self.Size != compared.Size )
		{
			return false;
		}

		if ( self.PixelFormat != compared.PixelFormat )
		{
			return false;
		}

		Rectangle rect = new Rectangle ( 0, 0, self.Width, self.Height );
		BitmapData data1 = self.LockBits ( rect, ImageLockMode.ReadOnly, self.PixelFormat );
		BitmapData data2 = compared.LockBits ( rect, ImageLockMode.ReadOnly, self.PixelFormat );

		bool result = true;

		int* p1 = (int*)data1.Scan0;
		int* p2 = (int*)data2.Scan0;
		int byteCount = self.Height * data1.Stride / 4; //only Format32bppArgb 
		for ( int i = 0; i < byteCount; ++i )
		{
			if ( *p1++ != *p2++ )
			{
				result = false;
				break;
			}
		}


		self.UnlockBits ( data1 );
		compared.UnlockBits ( data2 );

		return result;
	}

	public unsafe static bool HasAlpha ( this Bitmap self )
	{
		Rectangle bmpBounds = new Rectangle ( 0, 0, self.Width, self.Height );

		BitmapData bmpData = self.LockBits ( bmpBounds, ImageLockMode.ReadOnly, self.PixelFormat );

		bool hasAlpha = false;

		byte* ptrAlpha = ( (byte*)bmpData.Scan0.ToPointer () ) + 3;
		for ( int i = bmpData.Width * bmpData.Height; i > 0; --i )  // prefix-- should be faster
		{
			if ( *ptrAlpha < 255 )
			{
				hasAlpha = true;
				break;
			}

			ptrAlpha += 4;
		}

		self.UnlockBits ( bmpData );

		return hasAlpha;
	}
}
*/