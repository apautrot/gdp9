using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TextureHotLoad : MonoBehaviour
{
//	Texture2D texture;

	void Start ()
	{
		SpriteRenderer sr = GetComponent<SpriteRenderer> ();
		if ( sr != null )
		{
			Sprite sprite = sr.sprite;
			
// 			texture = sprite.texture;

 			TextureManager.ReplaceTexture ( sprite.texture );

			Vector2[] vertices = sprite.vertices;
			ushort[] triangles = sprite.triangles;
			for ( int i = 0 ; i < vertices.Length; i++ )
				vertices[i] += sprite.pivot;

			sprite.OverrideGeometry ( vertices, triangles );

// 			sprite.vertices[0] = new Vector2 ( -100, -100 );
// 			sprite.OverrideGeometry ( new Vector2[0], new ushort[0] );

// 			/*Texture2D*/ texture = TextureManager.LoadReplacementTexture ( sprite.texture );
// 			if ( texture == null )
// 			{
// 				Debug.LogError ( "Texture " + sprite.texture.name + " not found. You must probably rebuild the Build Textures." );
// 				return;
// 			}

// 			MaterialPropertyBlock myblock = new MaterialPropertyBlock ();
// 			myblock.AddTexture ( "_MainTex", texture );
// 			sr.SetPropertyBlock ( myblock );

// 			Besoin de post process
// 			- remplacer toutes les textures
// 			- post process les sprites et utiliser outil de génération géométrie de mesh sur "véritable texture" pour overrider géom sprite (OverrideGeometry)


	
// 			Rect rect = new Rect ( 0,0, texture.width, texture.height );
// 			float pixelsPerUnit = sprite.pixelsPerUnit;
// 			Vector2 pivot = new Vector2 ( sprite.pivot.x / rect.width, sprite.pivot.y / rect.height );
// 
// 			Sprite newSprite = Sprite.Create ( texture, rect, pivot, pixelsPerUnit, 0, SpriteMeshType.Tight );
// 			// newSprite.OverrideGeometry ( sprite.vertices, sprite.triangles );
// 
// 			// for ( int i = 0 ; i < sprite.vertices.Length; i++ )
// 			//	decals vertices par autant que le pivot implique
// 			sr.sprite = newSprite;

			Resources.UnloadUnusedAssets ();
		}
	}
}
