using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TextureManager : Singleton<TextureManager>
{
	internal static Texture2D ReplaceTexture ( Texture2D texture )
	{
		string pngFilePathname = texture.name + ".png";
		texture = LoadImage ( pngFilePathname, texture );
		if ( texture == null )
		{
			string jpgFilePathname = texture.name + ".jpg";
			texture = LoadImage ( jpgFilePathname, texture );

			if ( texture == null )
			{
				string jpegFilePathname = texture.name + ".jpeg";
				texture = LoadImage ( jpegFilePathname, texture );
			}
		}

		return texture;
	}

	internal static Texture2D LoadReplacementTexture ( Texture2D original )
	{
		return LoadReplacementTexture ( original.name );
	}

	internal static Texture2D LoadReplacementTexture ( string textureName )
	{
		Texture2D texture = null;

		string pngFilePathname = textureName + ".png";
		texture = LoadImage ( pngFilePathname );
		if ( texture == null )
		{
			string jpgFilePathname = textureName + ".jpg";
			texture = LoadImage ( jpgFilePathname );

			if ( texture == null )
			{
				string jpegFilePathname = textureName + ".jpeg";
				texture = LoadImage ( jpegFilePathname );
			}
		}

		return texture;
	}

	public static Texture2D LoadImage ( string filePathname, Texture2D texture = null )
	{
		TextureMetadata metadata; 
		if ( LoadTextureMetadata ( filePathname + ".meta", out metadata ) )
		{
			TextAsset bindata = Resources.Load ( filePathname ) as TextAsset;
			if ( bindata != null )
			{
				if ( texture == null )
					texture = new Texture2D ( 1, 1, metadata.format, metadata.hasMipmap );

				texture.LoadImage ( bindata.bytes );
				texture.wrapMode = metadata.wrapMode;
				texture.filterMode = metadata.filterMode;
			}
		}

		return texture;
	}

	struct TextureMetadata
	{
		internal bool hasMipmap;
		internal TextureFormat format;
		internal FilterMode filterMode;
		internal TextureWrapMode wrapMode;
	}

	private static bool LoadTextureMetadata ( string metadataFilePathname, out TextureMetadata metadata )
	{
		metadata = new TextureMetadata ();

		TextAsset bindata = Resources.Load < TextAsset > ( metadataFilePathname );
		if ( bindata != null )
		{
			using ( BinaryReader reader = new BinaryReader ( new MemoryStream ( bindata.bytes ) ) )
			{
				metadata.hasMipmap = reader.ReadBoolean ();
				metadata.format = (TextureFormat)reader.ReadInt32 ();
				metadata.filterMode = (FilterMode)reader.ReadInt32 ();
				metadata.wrapMode = (TextureWrapMode)reader.ReadInt32 ();
			}
			
			return true;
		}
		else
			return false;

		// release TextAsset memory ?
	}

#if UNITY_EDITOR
	public static void SaveTextureMetadata ( Texture2D texture, string metadataFilePathname )
	{
		bool hasMipmap = ( texture.mipmapCount > 1 );
		TextureFormat format = texture.format;
		FilterMode filterMode = texture.filterMode;
		TextureWrapMode wrapMode = texture.wrapMode;

		using ( FileStream fs = new FileStream ( metadataFilePathname + ".bytes", FileMode.Create, FileAccess.Write ) )
		{
			BinaryWriter writer = new BinaryWriter ( fs );
			writer.Write ( hasMipmap );
			writer.Write ( (int)format );
			writer.Write ( (int)filterMode );
			writer.Write ( (int)wrapMode );
		}
	}
#endif

}