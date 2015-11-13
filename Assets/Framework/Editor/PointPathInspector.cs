using UnityEngine;
using UnityEditor;
using System.Collections;


[CanEditMultipleObjects, CustomEditor ( typeof ( PointPath ) )]
public class PointPathInspector : Editor
{
	private static Vector3 pointSnap = Vector3.one * 0.1f;

	void OnSceneGUI ()
	{
		PointPath pointPath = (PointPath)target;

		float pixelSize = HandleUtility.GetHandleSize ( Vector3.zero ) / 160;

		// Handles.color = Color.white;

		Handles.color = Color.clear;

		for ( int i = 0; i < pointPath.Points.Count; i++ )
		{
			Vector3 oldPoint = pointPath.Points[i];
			Vector3 newPoint = Handles.FreeMoveHandle ( oldPoint, Quaternion.identity, pixelSize * 7, pointSnap, Handles.DotCap );
			if ( oldPoint != newPoint )
			{
				SerializedObject so = new SerializedObject ( target );
				if ( so != null )
				{
					so.Update ();

					SerializedProperty PointsArray = so.FindProperty ( "Points" );

					SerializedProperty point = PointsArray.GetArrayElementAtIndex ( i );
					if ( point != null )
						point.vector3Value = newPoint;

					so.ApplyModifiedProperties ();
				}
			}
		}
	}
}
