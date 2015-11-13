using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

public class ColliderDisplay : MonoBehaviour
{
	public bool display = true;
	public bool destroyOnAwake = true;
	public Color color = Color.gray;

	public void Awake ()
	{
		if ( destroyOnAwake )
		{
			Object.Destroy ( this );
		}
	}

	

	public static void GizmosDrawCollider ( GameObject go, Color color )
	{
		GizmosDrawCollider ( go, color, Vector3.zero );
	}

	public static void GizmosDrawCollider ( GameObject go, Color color, Vector3 offset, bool recursive = false )
	{
		if ( recursive )
		for ( int i = 0 ; i < go.transform.childCount; i++ )
			GizmosDrawCollider ( go.transform.GetChild ( i ).gameObject, color, offset, recursive );

		PolygonCollider2D poly = go.GetComponent<PolygonCollider2D>();
		if ( poly != null )
		{
			Vector3 position = go.transform.position + (Vector3)poly.offset + offset;

			Gizmos.color = color;

			for ( int i = 0; i < poly.points.Length - 1; i++ )
				Gizmos.DrawLine
				(
					position + (Vector3)poly.points[i],
					position + (Vector3)poly.points[i + 1]
				);

			Gizmos.DrawLine
			(
				position + (Vector3)poly.points[0],
				position + (Vector3)poly.points[poly.points.Length - 1]
			);

			Gizmos.color = Color.white;

			return;
		}

		CircleCollider2D circ = go.GetComponent<CircleCollider2D> ();
		if ( circ != null )
		{
			Vector3 position = go.transform.position + (Vector3)circ.offset + offset;

// 			Gizmos.color = Color.clear;
// 			float r2 = circ.radius * 1.5f;
// 			Gizmos.DrawCube ( position, new Vector3 ( r2, r2, r2 ) );
// 			Gizmos.color = Color.white;

			Handles.color = color;
			Handles.DrawWireDisc ( position, Vector3.back, circ.radius );
			Handles.color = Color.white;

			return;
		}

		BoxCollider2D box2d = go.GetComponent<BoxCollider2D> ();
		if ( box2d != null )
		{
			Vector3 position = go.transform.position + (Vector3)box2d.offset + offset;

// 			Gizmos.color = Color.clear;
// 			Gizmos.DrawCube ( position, box.size );
// 			Gizmos.color = Color.white;

			Gizmos.color = color;
			Gizmos.DrawWireCube ( position, box2d.size );
			Gizmos.color = Color.white;

			return;
		}

		BoxCollider box = go.GetComponent<BoxCollider> ();
		if ( box != null )
		{
			Vector3 position = go.transform.position + (Vector3)box.center + offset;

			// 			Gizmos.color = Color.clear;
			// 			Gizmos.DrawCube ( position, box.size );
			// 			Gizmos.color = Color.white;

			Matrix4x4 matrixCopy = Gizmos.matrix;
			Gizmos.matrix = Matrix4x4.TRS ( position, go.transform.rotation, go.transform.lossyScale );

			Gizmos.color = color;
			Gizmos.DrawWireCube ( /*position*/Vector3.zero, box.size );
			Gizmos.color = Color.white;

			Gizmos.matrix = matrixCopy;

			return;
		}

		Bounds bounds = go.GetBounds ( GetBoundsOption.ChildOnly );
		if ( bounds.size != Vector3.zero )
		{
			Vector3 position = bounds.center + offset;

// 			Gizmos.color = Color.clear;
// 			Gizmos.DrawCube ( offset, bounds.size );
// 			Gizmos.color = Color.white;

			Gizmos.color = color;
			Gizmos.DrawWireCube ( position, bounds.size );
			Gizmos.color = Color.white;

			return;
		}
	}


	public static void HandlesDrawCollider ( GameObject go, Color color )
	{
		HandlesDrawCollider ( go, color, Vector3.zero );
	}

	public static void HandlesDrawCollider ( GameObject go, Color color, Vector3 offset )
	{
		PolygonCollider2D poly = go.GetComponent<PolygonCollider2D> ();
		if ( poly != null )
		{
			Vector3 position = go.transform.position + (Vector3)poly.offset + offset;

			Handles.color = color;

			for ( int i = 0; i < poly.points.Length - 1; i++ )
				Handles.DrawLine
				(
					position + (Vector3)poly.points[i],
					position + (Vector3)poly.points[i + 1]
				);

			Handles.DrawLine
			(
				position + (Vector3)poly.points[0],
				position + (Vector3)poly.points[poly.points.Length - 1]
			);

			Gizmos.color = Color.white;

			return;
		}

		CircleCollider2D circ = go.GetComponent<CircleCollider2D> ();
		if ( circ != null )
		{
			Vector3 position = go.transform.position + (Vector3)circ.offset + offset;

			Handles.color = Color.clear;
			float r2 = circ.radius * 1.5f;
			HandlesDrawWireCube ( position, new Vector3 ( r2, r2, r2 ) );
			Handles.color = Color.white;

			return;
		}

		BoxCollider2D box = go.GetComponent<BoxCollider2D> ();
		if ( box != null )
		{
			Vector3 position = go.transform.position + (Vector3)box.offset + offset;

			Handles.color = color;
			HandlesDrawWireCube ( position, box.size );
			Handles.color = Color.white;

			return;
		}

		Bounds bounds = go.GetBounds ( GetBoundsOption.ChildOnly );
		if ( bounds.size != Vector3.zero )
		{
			Vector3 position = bounds.center + offset;

			Handles.color = color;
			HandlesDrawWireCube ( position, bounds.size );
			Handles.color = Color.white;

			return;
		}
	}

	public static void HandlesDrawWireCube ( Vector3 position, Vector3 size )
	{
		var half = size / 2;
		// draw front
		Handles.DrawLine ( position + new Vector3 ( -half.x, -half.y, half.z ), position + new Vector3 ( half.x, -half.y, half.z ) );
		Handles.DrawLine ( position + new Vector3 ( -half.x, -half.y, half.z ), position + new Vector3 ( -half.x, half.y, half.z ) );
		Handles.DrawLine ( position + new Vector3 ( half.x, half.y, half.z ), position + new Vector3 ( half.x, -half.y, half.z ) );
		Handles.DrawLine ( position + new Vector3 ( half.x, half.y, half.z ), position + new Vector3 ( -half.x, half.y, half.z ) );
		// draw back
		Handles.DrawLine ( position + new Vector3 ( -half.x, -half.y, -half.z ), position + new Vector3 ( half.x, -half.y, -half.z ) );
		Handles.DrawLine ( position + new Vector3 ( -half.x, -half.y, -half.z ), position + new Vector3 ( -half.x, half.y, -half.z ) );
		Handles.DrawLine ( position + new Vector3 ( half.x, half.y, -half.z ), position + new Vector3 ( half.x, -half.y, -half.z ) );
		Handles.DrawLine ( position + new Vector3 ( half.x, half.y, -half.z ), position + new Vector3 ( -half.x, half.y, -half.z ) );
		// draw corners
		Handles.DrawLine ( position + new Vector3 ( -half.x, -half.y, -half.z ), position + new Vector3 ( -half.x, -half.y, half.z ) );
		Handles.DrawLine ( position + new Vector3 ( half.x, -half.y, -half.z ), position + new Vector3 ( half.x, -half.y, half.z ) );
		Handles.DrawLine ( position + new Vector3 ( -half.x, half.y, -half.z ), position + new Vector3 ( -half.x, half.y, half.z ) );
		Handles.DrawLine ( position + new Vector3 ( half.x, half.y, -half.z ), position + new Vector3 ( half.x, half.y, half.z ) );
	}

	public void OnDrawGizmos ()
	{
		if ( !display )
			return;

		GizmosDrawCollider ( gameObject, color );
	}
}

#else

public class ColliderDisplay : MonoBehaviour
{
}

#endif