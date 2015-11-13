using UnityEngine;
using System.Collections;

static class MeshUtil
{
	internal static Mesh CreateQuad ( Mesh original, int width, int height, Color color )
	{
		return CreateQuad ( original, width, height, SpriteAlignment.Center, color, color, color, color );
	}

	internal static void ComputeAlignmentDecal ( SpriteAlignment alignment, out float dx, out float dy )
	{
		switch ( alignment )
		{
			default:
			case SpriteAlignment.Custom:
			case SpriteAlignment.Center:
			case SpriteAlignment.BottomCenter:
			case SpriteAlignment.TopCenter:
				dx = 0;
				break;

			case SpriteAlignment.BottomLeft:
			case SpriteAlignment.LeftCenter:
			case SpriteAlignment.TopLeft:
				dx = -1;
				break;

			case SpriteAlignment.BottomRight:
			case SpriteAlignment.RightCenter:
			case SpriteAlignment.TopRight:
				dx = 1;
				break;
		}

		switch ( alignment )
		{
			default:
			case SpriteAlignment.Custom:
			case SpriteAlignment.Center:
			case SpriteAlignment.LeftCenter:
			case SpriteAlignment.RightCenter:
				dy = 0;
				break;

			case SpriteAlignment.BottomCenter:
			case SpriteAlignment.BottomLeft:
			case SpriteAlignment.BottomRight:
				dy = -1;
				break;

			case SpriteAlignment.TopCenter:
			case SpriteAlignment.TopLeft:
			case SpriteAlignment.TopRight:
				dy = 1;
				break;
		}
	}

	internal static Mesh CreateQuad ( Mesh original, float width, float height, SpriteAlignment alignment, Color topLeftColor, Color topRightColor, Color bottomLeftColor, Color bottomRightColor )
	{
		Mesh mesh = original != null ? original : new Mesh ();
		mesh.name = "Quad";

		float x1 = 0;
		float y1 = 0;
		float x2 = 0;
		float y2 = 0;

		switch ( alignment )
		{
			case SpriteAlignment.Custom:
			case SpriteAlignment.Center:
			case SpriteAlignment.BottomCenter:
			case SpriteAlignment.TopCenter:
				x1 = -width / 2;
				x2 = width / 2;
				break;

			case SpriteAlignment.BottomLeft:
			case SpriteAlignment.LeftCenter:
			case SpriteAlignment.TopLeft:
				x1 = -width;
				x2 = 0;
				break;

			case SpriteAlignment.BottomRight:
			case SpriteAlignment.RightCenter:
			case SpriteAlignment.TopRight:
				x1 = 0;
				x2 = width;
				break;
		}

		switch ( alignment )
		{
			case SpriteAlignment.Custom:
			case SpriteAlignment.Center:
			case SpriteAlignment.LeftCenter:
			case SpriteAlignment.RightCenter:
				y1 = -height / 2;
				y2 = height / 2;
				break;

			case SpriteAlignment.BottomCenter:
			case SpriteAlignment.BottomLeft:
			case SpriteAlignment.BottomRight:
				y1 = -height;
				y2 = 0;
				break;

			case SpriteAlignment.TopCenter:
			case SpriteAlignment.TopLeft:
			case SpriteAlignment.TopRight:
				y1 = 0;
				y2 = height;
				break;
		}

		Vector3[] vertices = new Vector3[]
        {
            new Vector3( x2, y2, 0),
            new Vector3( x2, y1, 0),
            new Vector3( x1, y2, 0),
            new Vector3( x1, y1, 0)
        };

		Vector2[] uv = new Vector2[]
        {
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(0, 0),
        };

		int[] triangles = new int[]
        {
            0, 1, 2,
            2, 1, 3
        };

		Color[] colors = new Color[]
		{
			topRightColor,
			bottomRightColor,
			topLeftColor,
			bottomLeftColor
		};

		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.colors = colors;

		return mesh;
	}

	internal static Mesh CreatePlane ( Mesh original, float width, float height, int widthSegments, int heightSegments )
	{
		Mesh mesh = original != null ? original : new Mesh ();
		mesh.name = "Plane";

		int hCount2 = widthSegments + 1;
		int vCount2 = heightSegments + 1;
		int numTriangles = widthSegments * heightSegments * 6;
		int numVertices = hCount2 * vCount2;

		Vector3[] vertices = new Vector3[numVertices];
		Vector2[] uvs = new Vector2[numVertices];
		int[] triangles = new int[numTriangles];

		int index = 0;
		float uvFactorX = 1.0f / widthSegments;
		float uvFactorY = 1.0f / heightSegments;
		float scaleX = width / widthSegments;
		float scaleY = height / heightSegments;
		for ( float y = 0.0f; y < vCount2; y++ )
		{
			for ( float x = 0.0f; x < hCount2; x++ )
			{
				vertices[index] = new Vector3 ( x * scaleX - ( width / 2f ), y * scaleY - ( height / 2f ), 0.0f );
				uvs[index++] = new Vector2 ( x * uvFactorX, y * uvFactorY );
			}
		}

		index = 0;
		for ( int y = 0; y < heightSegments; y++ )
		{
			for ( int x = 0; x < widthSegments; x++ )
			{
				triangles[index] = ( y * hCount2 ) + x;
				triangles[index + 1] = ( ( y + 1 ) * hCount2 ) + x;
				triangles[index + 2] = ( y * hCount2 ) + x + 1;

				triangles[index + 3] = ( ( y + 1 ) * hCount2 ) + x;
				triangles[index + 4] = ( ( y + 1 ) * hCount2 ) + x + 1;
				triangles[index + 5] = ( y * hCount2 ) + x + 1;
				index += 6;
			}
		}

		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.RecalculateNormals ();

		return mesh;
	}

	internal static Mesh CreateNinePatch ( Mesh original, float width, float height )
	{
		Mesh mesh = original != null ? original : new Mesh ();
		mesh.name = "NinePatch";

		int numVertices = 16;
		int numTriIndex = 18 * 3;

		Vector3[] vertices = new Vector3[numVertices];
		Vector2[] uvs = new Vector2[numVertices];
		int[] triangles = new int[numTriIndex];

		int index = 0;
		for ( int y = 0; y < 3; y++ )
		{
			for ( int x = 0; x < 3; x++ )
			{
				triangles[index] = ( y * 4 ) + x;
				triangles[index + 1] = ( ( y + 1 ) * 4 ) + x;
				triangles[index + 2] = ( y * 4 ) + x + 1;

				triangles[index + 3] = ( ( y + 1 ) * 4 ) + x;
				triangles[index + 4] = ( ( y + 1 ) * 4 ) + x + 1;
				triangles[index + 5] = ( y * 4 ) + x + 1;

				index += 6;
			}
		}

		

		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;

		return mesh;
	}

	internal static void SetMeshColor ( Mesh mesh, Color color )
	{
		int numVertices = mesh.vertices.Length;

		Color[] colors = new Color[numVertices];

		for ( int i = 0; i < numVertices; i++ )
			colors[i] = color;

		mesh.colors = colors;
	}

	internal static void SetNinePatchXYs ( Mesh mesh, float width, float height, float left, float right, float top, float bottom, SpriteAlignment alignment = SpriteAlignment.Center )
	{
		Vector3[] vertices = mesh.vertices;

		float centerWidth = width - left - right;
		float centerHeight = height - top - bottom;

		float halfWidth = width / 2;
		float halfHeight = height / 2;

		float dx = 0;
		float dy = 0;
		ComputeAlignmentDecal ( alignment, out dx, out dy );
		float x = ( dx * halfWidth ) - halfWidth;
		float y = ( dy * halfHeight ) - halfHeight;

		vertices[0] = new Vector3 ( x,								y );
		vertices[1] = new Vector3 ( x + left,						y );
		vertices[2] = new Vector3 ( x + left + centerWidth,			y );
		vertices[3] = new Vector3 ( x + width,						y );

		vertices[4] = new Vector3 ( x,								y + bottom );
		vertices[5] = new Vector3 ( x + left,						y + bottom );
		vertices[6] = new Vector3 ( x + left + centerWidth,			y + bottom );
		vertices[7] = new Vector3 ( x + width,						y + bottom );

		vertices[8] = new Vector3 ( x,								y + bottom + centerHeight );
		vertices[9] = new Vector3 ( x + left,						y + bottom + centerHeight);
		vertices[10] = new Vector3 ( x + left + centerWidth,		y + bottom + centerHeight );
		vertices[11] = new Vector3 ( x + width,						y + bottom + centerHeight );

		vertices[12] = new Vector3 ( x,								y + height );
		vertices[13] = new Vector3 ( x + left,						y + height );
		vertices[14] = new Vector3 ( x + left + centerWidth,		y + height );
		vertices[15] = new Vector3 ( x + width,						y + height );

		mesh.vertices = vertices;

		mesh.RecalculateNormals ();
	}

	internal static void SetNinePatchUVs ( Mesh mesh, /* float width, float height,*/ float left, float right, float top, float bottom )
	{
		Vector2[] uvs = mesh.uv;

		float width = 1.0f;
		float height = 1.0f;

		float centerWidth = width - left - right;
		float centerHeight = height - top - bottom;

		uvs[0] = new Vector2 ( 0,						0 );
		uvs[1] = new Vector2 ( 0 + left,				0 );
		uvs[2] = new Vector2 ( 0 + left + centerWidth,	0 );
		uvs[3] = new Vector2 ( 0 + width,				0 );

		uvs[4] = new Vector2 ( 0,						top );
		uvs[5] = new Vector2 ( 0 + left,				top );
		uvs[6] = new Vector2 ( 0 + left + centerWidth,	top );
		uvs[7] = new Vector2 ( 0 + width,				top );

		uvs[8] = new Vector2 ( 0,						top + centerHeight );
		uvs[9] = new Vector2 ( 0 + left,				top + centerHeight );
		uvs[10] = new Vector2 ( 0 + left + centerWidth, top + centerHeight );
		uvs[11] = new Vector2 ( 0 + width,				top + centerHeight );

		uvs[12] = new Vector2 ( 0,						height );
		uvs[13] = new Vector2 ( 0 + left,				height );
		uvs[14] = new Vector2 ( 0 + left + centerWidth, height );
		uvs[15] = new Vector2 ( 0 + width,				height );

		mesh.uv = uvs;
	}

	public static void SetMesh ( GameObject gameObject, Mesh mesh, Texture2D texture )
	{
		MeshFilter filter = gameObject.GetOrCreateComponent<MeshFilter> ();
		MeshRenderer renderer = gameObject.GetOrCreateComponent<MeshRenderer> ();
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
		if ( renderer.sharedMaterial != null )
			renderer.sharedMaterial.mainTexture = texture;

		filter.mesh = mesh;
	}

}