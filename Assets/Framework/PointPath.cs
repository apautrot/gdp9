using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PointPath : MonoBehaviour
{
	public bool UseStraightLines;
	public bool UseControlPoints;
	public bool ClosePath = true;
	public bool DrawGizmo = true;

	public List<Vector3> Points = new List<Vector3>();

	void Awake ()
	{
		gameObject.SetActive ( false );
	}

	private GoSpline CreateSpline ()
	{
		List<Vector3> points = new List<Vector3> ( Points );

		if ( !UseControlPoints || ClosePath )
		{
			points.Insert ( 0, points[0] );
			points.Add ( points[points.Count - 1] );
		}

		GoSpline spline = new GoSpline ( points, UseStraightLines );
		if ( ClosePath )
			spline.closePath ();

		return spline;
	}

	internal GoTween Animate ( GameObject target, float duration, GoEaseType ease, bool loopsInfinitely, bool isRelative = false )
	{
		GoTween tween = null;
		if ( Points.Count > 2 )
		{
			tween = Go.to ( target.transform, duration, new GoTweenConfig ().positionPath ( CreateSpline (), isRelative ) );
			tween.eases ( ease );
			if ( loopsInfinitely )
				tween.loopsInfinitely ();
		}
		else if ( Points.Count > 1 )
		{
			target.transform.position = Points[0];
			tween = target.transform.positionTo ( duration, Points[1] );
			tween.eases ( ease );
			if ( loopsInfinitely )
				tween.loopsInfinitely ( GoLoopType.PingPong );
		}
		return tween;
	}

#if UNITY_EDITOR

	private void DrawGizmos ( bool drawPoints, Color color )
	{
		float pixelSize = HandleUtility.GetHandleSize ( Vector3.zero ) / 160;

		float size = pixelSize * 10;
		Vector3 sizeVector = new Vector3 ( size, size, size );

		if ( drawPoints )
		{
			Gizmos.color = Color.clear;
			for ( int i = 0; i < Points.Count; i++ )
				Gizmos.DrawCube ( Points[i], sizeVector );

			Handles.color = color;
			for ( int i = 0; i < Points.Count; i++ )
				Handles.DotCap ( 0, Points[i], Quaternion.identity, pixelSize * 5 );
		}

		Gizmos.color = color;
		for ( int i = 1; i < Points.Count; i++ )
			Gizmos.DrawLine ( Points[i - 1], Points[i] );
	}

	public void OnDrawGizmos ()
	{
		if ( ! DrawGizmo )
			return;

		if ( !UseStraightLines )
			if ( Points.Count > 2 )
			{
				Gizmos.color = Color.magenta;
				CreateSpline().drawGizmos ( 5 );
			}

		if ( Selection.Contains ( gameObject ) )
			DrawGizmos ( false, Color.white );
		else
			DrawGizmos ( true, ColorUtil.fromColor ( Color.white, 0.35f ) );
	}
#endif

}
