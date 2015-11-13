using UnityEngine;
using System.Collections;

class TouchInput : Singleton<TouchInput>
{
	internal static bool isJustDown;
	internal static bool isJustDragged;
	internal static bool isJustUp;

	internal static bool isDown;
	internal static bool isUp;
	internal static bool isMoved;
	internal static bool isDragging;

	internal static Vector3 previousPosition;
	internal static Vector3 position;
	internal static Vector3 dragVector;
	internal static float downTime;

	public Camera Camera = null;

	static internal Vector3 GetWorldPositionOnXYPlane ( Camera camera, Vector3 position, float z = 0 )
	{
		Ray ray = camera.ScreenPointToRay ( position );
		Plane xy = new Plane ( Vector3.forward, new Vector3 ( 0, 0, z ) );
		float distance;
		xy.Raycast ( ray, out distance );
		Vector3 point = ray.GetPoint ( distance );

		return point;
	}

	static internal Vector3 GetInputPositionOnXYPlane ( float z = 0 )
	{
		return GetWorldPositionOnXYPlane ( Camera.main, Input.mousePosition, z );
	}

	const int MAXIMUM_TOUCH = 4;

	static internal GameObject[] underTouch = new GameObject[MAXIMUM_TOUCH];
	static internal Vector2[] dragVectors = new Vector2[MAXIMUM_TOUCH];
	static internal float[] downTimes = new float[MAXIMUM_TOUCH];
	static internal Vector3[] previousPositions = new Vector3[MAXIMUM_TOUCH];
	static internal Vector3[] positions = new Vector3[MAXIMUM_TOUCH];

	internal void Update ()
	{
		Camera inputCamera = Camera != null ? Camera : Camera.main;

		previousPosition = position;
		if ( inputCamera.orthographic )
			position = inputCamera.ScreenToWorldPoint ( Input.mousePosition );
		else
			position = GetWorldPositionOnXYPlane ( inputCamera, Input.mousePosition, 5 );

// 		{
// 			DebugWindow.Log ( "TouchInput", "Position", position );
// 			Ray ray = inputCamera.ScreenPointToRay ( position );
// 			RaycastHit2D[] hits2D = new RaycastHit2D[4];
// 			int count = Physics2D.RaycastNonAlloc ( ray.origin, ray.direction, hits2D );
// 			DebugWindow.Log ( "TouchInput", "Under", count );
// 			for ( int j = 0; j < count; j++ )
// 			{
// 				RaycastHit2D hit2D = hits2D[j];
// 				DebugWindow.Log ( "TouchInput", "Under[" + j + "]", hit2D.collider );
// 			}
// 		}

		isDown = Input.GetMouseButton ( 0 );
		isUp = !isDown;
		isMoved = ( previousPosition != position );
		isJustDown = Input.GetMouseButtonDown ( 0 );
		isJustUp = Input.GetMouseButtonUp ( 0 );

		isJustDragged = false;
		if ( isDown )
		{
			isJustDragged = isMoved;
			isDragging |= isMoved;

			if ( isJustDown )
			{
				dragVector = Vector2.zero;
				downTime = 0;
			}
			else
			{
				dragVector.x += position.x - previousPosition.x;
				dragVector.y += position.y - previousPosition.y;
				downTime += Time.deltaTime;
			}
		}
		else
		{
			isDragging = false;
		}

		int touchCount = Mathf.Min ( Input.touchCount, MAXIMUM_TOUCH );
		for ( int i = 0; i < touchCount; i++ )
		{
			Touch touch = Input.GetTouch ( i );

			previousPositions[i] = positions[i];
			if ( inputCamera.orthographic )
				positions[i] = inputCamera.ScreenToWorldPoint ( touch.position );
			else
				positions[i] = GetWorldPositionOnXYPlane ( inputCamera, touch.position, 5 );

			if ( touch.phase == TouchPhase.Began )
			{
				dragVectors[i] = Vector2.zero;
				downTimes[i] = 0;
			}
			else
			{
				dragVectors[i] += (Vector2)( positions[i] - previousPositions[i] );
				downTimes[i] += Time.deltaTime;
			}
		}

		for ( int i = 0; i < touchCount; i++ )
		{
			Touch touch = Input.GetTouch ( i );
			if ( ( touch.phase != TouchPhase.Stationary ) && ( touch.phase != TouchPhase.Moved ) )
			{
				int fid = touch.fingerId;

				if ( touch.phase == TouchPhase.Began )
				{
					Ray ray = inputCamera.ScreenPointToRay ( touch.position );
					RaycastHit2D[] hits2D = new RaycastHit2D[4];
					int count = Physics2D.RaycastNonAlloc ( ray.origin, ray.direction, hits2D );
					for ( int j = 0; j < count; j++ )
					{
						RaycastHit2D hit2D = hits2D[j];
						//						Debug.Log ( "Collider [" + j + "]" + hit2D.collider + " under touch" );

						GameObject go = hit2D.transform.gameObject;
						TouchInputAction touchZone = go.GetComponent<TouchInputAction> ();
						if ( touchZone != null )
						{
							underTouch[fid] = go;
							touchZone.OnTouch ( touch.phase );

							// 							DebugWindow.Log ( "Touch", fid.ToString (), touch.phase.ToString () );
							// 							DebugWindow.Log ( "Touch", fid.ToString () + " name", touchZone.name );

							break;
						}
					}
				}
				else if ( underTouch[fid] != null )
				{
					TouchInputAction touchZone = underTouch[fid].GetComponent<TouchInputAction> ();
					if ( touchZone != null )
					{
						touchZone.OnTouch ( touch.phase );

						// 						DebugWindow.Log ( "Touch", fid.ToString (), touch.phase.ToString () );
						// 						DebugWindow.Log ( "Touch", fid.ToString () + " name", touchZone.name );
					}

					underTouch[fid] = null;
				}
			}
		}
	}
}