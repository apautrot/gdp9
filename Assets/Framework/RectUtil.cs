using UnityEngine;
using System.Collections;

static class RectUtil
{
	internal static Rect FromCollider ( BoxCollider2D collider )
	{
		float scaleX = 1;
		float scaleY = 1;

		if ( collider.transform.parent != null )
		{
			scaleX = collider.transform.parent.lossyScale.x;
			scaleY = collider.transform.parent.lossyScale.y;
		}

		float sizeX = collider.size.x * scaleX;
		float sizeY = collider.size.y * scaleY;
		float centerX = collider.offset.x * scaleX;
		float centerY = collider.offset.y * scaleY;

		float left = collider.transform.position.x - ( sizeX / 2 ) + centerX;
		float bottom = collider.transform.position.y - ( sizeY / 2 ) + centerY;
		return new Rect ( left, bottom, sizeX, sizeY );
	}
}


static class RandomInt
{
	private static System.Random rnd = new System.Random ();

	//! Range between min (included) anx max (included).
	internal static int Range ( int min, int max )
	{
		return rnd.Next ( min, max+1 );
	}
}


static class RandomBool
{
	private static System.Random rnd = new System.Random ();

	internal static bool Next ()
	{
		return ( ( rnd.Next () & 1 ) == 0 );
	}
}



static class RandomFloat
{
	private static System.Random rnd = new System.Random ();

	internal static float Range ( float min, float max )
	{
		return (float) ( ( rnd.NextDouble () * ( max - min ) ) + min );
	}
}


static class Vector3Utils
{
	public static Vector3 getPointOnPath ( Vector3 from, Vector3 to, float atLength, float minNoise, float maxNoise )
	{
		Vector3 path = ( to - from );
		Vector3 point = ( path * atLength ) + from;
		
		Vector3 direction = path.normalized;
		bool decalLeft = RandomBool.Next ();
		Vector3 decal = new Vector3 ( direction.y, direction.x, 0 ) * Random.Range ( minNoise, maxNoise );
		if ( decalLeft ) decal *= -1;
		// Debug.Log ( "Decal " + decal + " " + decalLeft );
		// Vector3 decal = Quaternion.Euler ( 0, 0, Random.Range ( 0, 360 ) ) * ( Vector3.right * distanceRadius );
		point += decal;
		
		return point;
	}

	public static Vector3 GetRandomVector ( float xRange, float yRange, float zRange )
	{
		return new Vector3
		(
			RandomFloat.Range ( -xRange, xRange ),
			RandomFloat.Range ( -yRange, yRange ),
			RandomFloat.Range ( -zRange, zRange )
		);
	}

	public static Vector3 GetRandomVector ( float xRangeMin, float yRangeMin, float zRangeMin, float xRangeMax, float yRangeMax, float zRangeMax )
	{
		return new Vector3
		(
			RandomFloat.Range ( xRangeMin, xRangeMin ),
			RandomFloat.Range ( yRangeMin, yRangeMax ),
			RandomFloat.Range ( zRangeMin, zRangeMax )
		);
	}
}


static class ArrayExtensionMethods
{
	public static T[] Shuffle<T> ( this T[] self )
	{
		for ( int t = 0; t < self.Length; t++ )
		{
			T tmp = self[t];
			int r = Random.Range ( t, self.Length );
			self[t] = self[r];
			self[r] = tmp;
		}
		return self;
	}
}