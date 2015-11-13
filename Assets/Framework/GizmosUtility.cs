#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;

public enum GizmosLink
{
	Line,
	DoubleLine,
	Arrow,
	DoubleArrow,
}

public static class GizmosUtility
{
	public static void DrawLinkBetween ( GameObject a, GameObject b, GizmosLink link, Color color )
	{

	}

	public static void HandlesDrawArrow ( Vector3 from, Vector3 to, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f )
	{
		HandlesDrawArrow ( from, to, Color.white, arrowHeadLength, arrowHeadAngle );
	}

	public static void HandlesDrawArrow ( Vector3 from, Vector3 to, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f )
	{
		Vector3 direction = to - from;

		Handles.color = color;
		Handles.DrawLine ( from, to );

		if ( direction != Vector3.zero )
		{
			Vector3 right = Quaternion.LookRotation ( direction ) * Quaternion.Euler ( 0, 180 + arrowHeadAngle, 0 ) * new Vector3 ( 0, 0, 1 );
			Vector3 left = Quaternion.LookRotation ( direction ) * Quaternion.Euler ( 0, 180 - arrowHeadAngle, 0 ) * new Vector3 ( 0, 0, 1 );
			Handles.DrawLine ( to, to + right * arrowHeadLength );
			Handles.DrawLine ( to, to + left * arrowHeadLength );
		}
	}

	public static void HandlesLabel ( Vector3 position, string text, Color color )
	{
		GUIStyle style = new GUIStyle ();
		style.normal.textColor = color;
		Handles.Label ( position, text, style );
	}

	public static void GizmosDrawArrow ( Vector3 from, Vector3 to, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f )
	{
		GizmosDrawArrow ( from, to, Color.white, arrowHeadLength, arrowHeadAngle );
	}

	public static void GizmosDrawArrow ( Vector3 from, Vector3 to, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f )
	{
		Vector3 direction = to - from;

		Gizmos.color = color;
		Gizmos.DrawRay ( from, direction );

		Vector3 right = Quaternion.LookRotation ( direction ) * Quaternion.Euler ( 0, 180 + arrowHeadAngle, 0 ) * new Vector3 ( 0, 0, 1 );
		Vector3 left = Quaternion.LookRotation ( direction ) * Quaternion.Euler ( 0, 180 - arrowHeadAngle, 0 ) * new Vector3 ( 0, 0, 1 );
		Gizmos.DrawRay ( to, right * arrowHeadLength );
		Gizmos.DrawRay ( to, left * arrowHeadLength );
	}
}

#endif