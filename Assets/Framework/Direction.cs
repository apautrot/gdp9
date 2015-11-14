using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Direction
{
	None = 0,
	TopLeft = 1,
	TopRight = 2,
	BottomLeft = 4,
	BottomRight = 8
}

public static class DirectionExtensionMethods
{
	public static Vector3 ToVector ( this Direction direction )
	{
		Vector3 v = Vector3.zero;
		switch ( direction )
		{
			case Direction.TopLeft:
				v = Vector3.left;
				break;
			case Direction.TopRight:
				v = Vector3.forward;
				break;
			case Direction.BottomRight:
				v = Vector3.right;
				break;
			case Direction.BottomLeft:
				v = Vector3.back;
				break;
		}
		return v;
	}

	public static Vector3 ToRotationVector ( this Direction direction )
	{
		Vector3 v = Vector3.zero;
		switch ( direction )
		{
			case Direction.BottomRight:
				v = new Vector3 ( 0, 0, -1 );
				break;
			case Direction.BottomLeft:
				v = new Vector3 ( -1, 0, 0 );
				break;
			case Direction.TopLeft:
				v = new Vector3 ( 0, 0, 1 );
				break;
			case Direction.TopRight:
				v = new Vector3 ( 1, 0, 0 );
				break;
		}
		return v;
	}

	public static Direction Opposite ( this Direction direction )
	{
		switch ( direction )
		{
			case Direction.BottomLeft: return Direction.TopRight;
			case Direction.BottomRight: return Direction.TopLeft;
			case Direction.TopRight: return Direction.BottomLeft;
			case Direction.TopLeft: return Direction.BottomRight;
			default: return Direction.None;
		}
	}

	public static Direction TurnedLeft ( this Direction direction )
	{
		switch ( direction )
		{
			case Direction.BottomLeft: return Direction.BottomRight;
			case Direction.BottomRight: return Direction.TopRight;
			case Direction.TopRight: return Direction.TopLeft;
			case Direction.TopLeft: return Direction.BottomLeft;
			default: return Direction.None;
		}
	}

	public static Direction TurnedRight ( this Direction direction )
	{
		switch ( direction )
		{
			case Direction.BottomLeft: return Direction.TopLeft;
			case Direction.BottomRight: return Direction.BottomLeft;
			case Direction.TopRight: return Direction.BottomRight;
			case Direction.TopLeft: return Direction.TopRight;
			default: return Direction.None;
		}
	}

	public static int ForwardAngleOnY ( this Direction direction )
	{
		switch ( direction )
		{
			case Direction.BottomLeft: return 180;
			case Direction.BottomRight: return 90;
			case Direction.TopRight: return 0;
			case Direction.TopLeft: return 270;
			default: return 0;
		}
	}

	public static Vector3 TurnForwardZOrientedVector ( this Direction self, Vector3 v )
	{
		switch ( self )
		{
			case Direction.TopLeft:
				return new Vector3 ( -v.z, v.y, v.x );
			case Direction.TopRight:
				return v;
			case Direction.BottomLeft:
				return new Vector3 ( -v.x, v.y, -v.z );
			case Direction.BottomRight:
				return new Vector3 ( v.z, v.y, -v.x );
			default:
				return v;
		}
	}

	public static Direction FromVector ( Vector3 v )
	{
		float angle = v.AngleFromForward ();

		if ( angle >= 0 && angle < 45 )
			return Direction.TopRight;
		if ( angle >= 45 && angle < 135 )
			return Direction.BottomRight;
		if ( angle >= 135 && angle < 225 )
			return Direction.BottomLeft;
		if ( angle >= 225 && angle < 315 )
			return Direction.TopLeft;
		if ( angle >= 315 )
			return Direction.TopRight;

		return Direction.None;
	}
}
