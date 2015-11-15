using UnityEngine;

enum AngleDomain
{
	FromMinus180To180,
	From0To360,
}


static class RectExtensionMethods
{
	public static bool IntersectsLine ( this Rect r, Vector2 p1, Vector2 p2 )
	{
		return LineIntersectsLine ( p1, p2, new Vector2 ( r.x, r.y ), new Vector2 ( r.x + r.width, r.y ) ) ||
			   LineIntersectsLine ( p1, p2, new Vector2 ( r.x + r.width, r.y ), new Vector2 ( r.x + r.width, r.y + r.height ) ) ||
			   LineIntersectsLine ( p1, p2, new Vector2 ( r.x + r.height, r.y + r.height ), new Vector2 ( r.x, r.y + r.height ) ) ||
			   LineIntersectsLine ( p1, p2, new Vector2 ( r.x, r.y + r.height ), new Vector2 ( r.x, r.y ) ) ||
			   ( r.Contains ( p1 ) && r.Contains ( p2 ) );
	}

	private static bool LineIntersectsLine ( Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4 )
	{
   
		Vector2 a = p2 - p1;
		Vector2 b = p3 - p4;
		Vector2 c = p1 - p3;
   
		float alphaNumerator = b.y*c.x - b.x*c.y;
		float alphaDenominator = a.y*b.x - a.x*b.y;
		float betaNumerator  = a.x*c.y - a.y*c.x;
		float betaDenominator  = a.y*b.x - a.x*b.y;
   
		bool doIntersect = true;
   
		if (alphaDenominator == 0 || betaDenominator == 0)
			doIntersect = false;
		else
		{

			if ( alphaDenominator > 0 )
			{
				if ( alphaNumerator < 0 || alphaNumerator > alphaDenominator )
					doIntersect = false;
			}
			else if ( alphaNumerator > 0 || alphaNumerator < alphaDenominator )
				doIntersect = false;

			if ( doIntersect && betaDenominator > 0 )
			{
				if ( betaNumerator < 0 || betaNumerator > betaDenominator )
					doIntersect = false;
			}
			else if ( betaNumerator > 0 || betaNumerator < betaDenominator )
				doIntersect = false;
		}
 
		return doIntersect;
	}

}

static class BoundsExtensionMethods
{
	public static string ToStringEx ( this Bounds bounds )
	{
		return "c:" + bounds.center.ToStringEx () + " e:" + bounds.size.ToString ();
	}
}

static class Vector3ExtensionMethods
{
	public static Vector3 WithXYReplacedBy ( this Vector3 self, float x, float y )
	{
		self.x = x;
		self.y = y;
		return self;
	}

	public static Vector3 WithXReplacedBy ( this Vector3 self, float x )
	{
		self.x = x;
		return self;
	}

	public static Vector3 WithYReplacedBy ( this Vector3 self, float y )
	{
		self.y = y;
		return self;
	}

	public static Vector3 WithZReplacedBy ( this Vector3 self, float z )
	{
		self.z = z;
		return self;
	}

	public static Vector3 NormalizeValues ( this Vector3 self )
	{
		if ( ( self.x > self.y ) && ( self.x > self.z ) )
		{
			self.y /= self.x;
			self.z /= self.x;
			self.x = 1;
		}
		else if ( ( self.y > self.x ) && ( self.y > self.z ) )
		{
			self.x /= self.y;
			self.z /= self.y;
			self.y = 1;
		}
		else if ( ( self.z > self.x ) && ( self.z > self.y ) )
		{
			self.x /= self.z;
			self.y /= self.z;
			self.z = 1;
		}
		return self;
	}

	public static Vector3 EulerRotate ( this Vector3 self, float xAngle, float yAngle, float zAngle )
	{
		return Quaternion.Euler ( xAngle, yAngle, zAngle ) * self;
	}

	public static Vector3 AxisRotate ( this Vector3 self, float angleInDeg, Vector3 axis )
	{
		return Quaternion.AngleAxis ( angleInDeg, axis ) * self;
	}

	public static Vector3 AxisRotateOn ( this Vector3 self, Vector3 pivot, float angleInDeg, Vector3 axis )
	{
		self -= pivot;
		self = Quaternion.AngleAxis ( angleInDeg, axis ) * self;
		self += pivot;
		return self;
	}

// 	public static float AngleFromUp ( this Vector3 self, AngleDomain resultDomain = AngleDomain.From0To360 )
// 	{
// 		return Vector2.zero.Angle ( self, resultDomain );
// 	}

	// Gives angle of this vector around up vector (Z).
	// Unlike Vector3.Angle that gives angle between vectors
	// Result is between -180..180 or 0..360
	// self + Vector.up is angle 0
	// self + Vector.down is angle 180
	public static float AngleFromUp ( this Vector3 self, AngleDomain resultDomain = AngleDomain.From0To360 )
	{
		return Vector3.zero.AngleFromUp ( self, resultDomain );
	}

	public static float AngleFromForward ( this Vector3 self, AngleDomain resultDomain = AngleDomain.From0To360 )
	{
		return Vector3.zero.AngleFromForward ( self, resultDomain );
	}

	// Gives angle between 2 points.
	// Unlike Vector3.Angle that gives angle between vectors
	// Result is between -180..180 or 0..360
	// self + Vector.up is angle 0
	// self + Vector.down is angle 180
	private static float AngleFromUp ( this Vector3 self, Vector3 other, AngleDomain resultDomain = AngleDomain.From0To360 )
	{
		float angle = Mathf.Atan2 ( other.x - self.x, other.y - self.y ) * Mathf.Rad2Deg;
		if ( resultDomain == AngleDomain.From0To360 )
			if ( angle < 0 )
				angle += 360;
		return angle;
	}

	// Gives angle between 2 points.
	// Unlike Vector3.Angle that gives angle between vectors
	// Result is between -180..180 or 0..360
	// self + Vector.forward is angle 0
	// self + Vector.backward is angle 180
	private static float AngleFromForward ( this Vector3 self, Vector3 other, AngleDomain resultDomain = AngleDomain.From0To360 )
	{
		float angle = Mathf.Atan2 ( other.x - self.x, other.z - self.z ) * Mathf.Rad2Deg;
		if ( resultDomain == AngleDomain.From0To360 )
			if ( angle < 0 )
				angle += 360;
		return angle;
	}

	public static Vector3 AngleNormalize ( this Vector3 self )
	{
		self.x = ( self.x + 360 ) % 360;
		self.y = ( self.y + 360 ) % 360;
		self.z = ( self.z + 360 ) % 360;
		return self;
	}

	public static Vector3 AngleDelta ( this Vector3 self, Vector3 other )
	{
		return new Vector3 ( Mathf.DeltaAngle ( self.x, other.x ), Mathf.DeltaAngle ( self.y, other.y ), Mathf.DeltaAngle ( self.z, other.z ) );
	}

	public static Vector3 RotateX ( this Vector3 self, float angleInRad )
	{
		float sin = Mathf.Sin ( angleInRad );
		float cos = Mathf.Cos ( angleInRad );

		float ty = self.y;
		float tz = self.z;
		self.y = ( cos * ty ) - ( sin * tz );
		self.z = ( cos * tz ) + ( sin * ty );

		return self;
	}

	public static Vector3 RotateY ( this Vector3 self, float angleInRad )
	{
		float sin = Mathf.Sin ( angleInRad );
		float cos = Mathf.Cos ( angleInRad );

		float tx = self.x;
		float tz = self.z;
		self.x = ( cos * tx ) + ( sin * tz );
		self.z = ( cos * tz ) - ( sin * tx );

		return self;
	}

	public static Vector3 RotateZ ( this Vector3 self, float angleInRad )
	{
		float sin = Mathf.Sin ( angleInRad );
		float cos = Mathf.Cos ( angleInRad );

		float tx = self.x;
		float ty = self.y;
		self.x = ( cos * tx ) - ( sin * ty );
		self.y = ( cos * ty ) + ( sin * tx );

		return self;
	}

	public static float GetPitch ( this Vector3 self )
	{
		float len = Mathf.Sqrt ( ( self.x * self.x ) + ( self.z * self.z ) );    // Length on xz plane.
		return ( -Mathf.Atan2 ( self.y, len ) );
	}

	public static float GetYaw ( this Vector3 self )
	{
		return ( Mathf.Atan2 ( self.x, self.z ) );
	}

	public static string ToStringEx ( this Vector3 self )
	{
		return self.x + " , " + self.y + " , " + self.z;
	}

	public static float DistanceTo ( this Vector3 self, Vector3 to )
	{
		return ( self - to ).magnitude;
	}
}



static class Vector2ExtensionMethods
{
	public static Vector2 Rotate ( this Vector2 self, float angleInDegree )
	{
		angleInDegree *= Mathf.Deg2Rad;

		float sin = Mathf.Sin ( angleInDegree );
		float cos = Mathf.Cos ( angleInDegree );

		float tx = self.x;
		float ty = self.y;
		self.x = ( cos * tx ) - ( sin * ty );
		self.y = ( cos * ty ) + ( sin * tx );

		return self;
	}

	public static Vector2 Add ( this Vector2 self, float x, float y )
	{
		self.x += x;
		self.y += y;
		return self;
	}

	public static bool LineIntersectionPoint ( Vector2 axy, Vector2 axy2, Vector2 bxy, Vector2 bxy2, ref Vector2 result )
	{
		// Get A,B,C of first line - points : ps1 to pe1
		float A1 = axy2.y - axy.y;
		float B1 = axy.x - axy2.x;
		float C1 = A1 * axy.x + B1 * axy.y;

		// Get A,B,C of second line - points : ps2 to pe2
		float A2 = bxy2.y - bxy.y;
		float B2 = bxy.x - bxy2.x;
		float C2 = A2 * bxy.x + B2 * bxy.y;

		// Get delta and check if the lines are parallel
		float delta = A1 * B2 - A2 * B1;
		if ( delta == 0 )
		{
			return false;
		}
		else
		{
			// now return the Vector2 intersection point
			result = new Vector2 (
				( B2 * C1 - B1 * C2 ) / delta,
				( A1 * C2 - A2 * C1 ) / delta
			);
			return true;
		}
	}

	public static string ToStringEx ( this Vector2 self )
	{
		return self.x + " , " + self.y;
	}

	public static Vector2 ComputeGridPosition ( int index, int count, int maxCountPerLine, int horizontalSpacing, int verticalSpacing )
	{
		int rowIndex = index % maxCountPerLine;
		int lineIndex = index / maxCountPerLine;

		int rowCount = maxCountPerLine;
		if ( count < ( ( lineIndex + 1 ) * maxCountPerLine ) )
			rowCount = count % maxCountPerLine;

		int lineCount = count / maxCountPerLine;	// full line count
		if ( ( count % maxCountPerLine ) != 0 )
			lineCount += 1;							// +1 for partial line

		Vector2 position = new Vector2 ();
		position.x = horizontalSpacing * rowIndex - ( horizontalSpacing * ( rowCount - 1 ) / 2 );
		position.y = -verticalSpacing * lineIndex + ( verticalSpacing * ( lineCount - 1 ) / 2 );

		return position;
	}
}

static class Matrix4x4ExtensionMethods
{
	public static Quaternion ToQuaternion ( this Matrix4x4 self )
	{
		return Quaternion.LookRotation ( self.GetColumn ( 2 ), self.GetColumn ( 1 ) );
	}
}