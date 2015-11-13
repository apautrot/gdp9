using UnityEngine;
using System.Collections;

public class DragDrop : SceneSingleton<DragDrop>
{
	private bool dragStarted;
	private Draggable dragSource;
	internal Draggable DragSource
	{
		get { return dragSource; }
		set
		{
			if ( dragStarted )
				StopDrag ( DragOver.Ignore );

			dragSource = value;
		}
	}

	internal bool IsDragging
	{
		get { return dragSource; }
	}

	private float MinDragDistance = 20;
	private IDropTarget dropTarget;
// 	internal DropTarget DropTarget
// 	{
// 		get { return dropTarget; }
// 		set { dropTarget = value; }
// 	}

	private Vector3 dragStartPosition;
	internal Vector3 DragBackPosition
	{
		get { return dragStartPosition; }
	}

	internal bool DontSendToBack;

	void FixedUpdate ()
	{
		// Draggable dragSource = DragSource;

//		DebugWindow.Log ( "DragDrop", "Draggable", ( dragSource != null ) ? dragSource.ToString () : "none" );

		if ( ( DragSource != null ) && TouchInput.isDragging )
		{
			RaycastHit2D[] result = new RaycastHit2D[5];
			int count = Physics2D.RaycastNonAlloc ( TouchInput.position, Vector3.forward, result );
			if ( count > 0 )
			{
				for ( int i = 0; i < count; i++ )
				{
					dropTarget = result[i].collider.gameObject.GetComponentAs<IDropTarget> ();
					if ( dropTarget != null )
						break;
				}
			}
			else
				dropTarget = null;

//			DebugWindow.Log ( "DragDrop", "DropTarget", ( dropTarget != null ) ? dropTarget.ToString () : "none" );
		}

		if ( dragSource != null )
		{
			if ( TouchInput.isDragging )
			{
				if ( !dragStarted )
					if ( TouchInput.dragVector.magnitude > MinDragDistance )
						StartDrag ();

				if ( dragStarted )
				{
					Vector3 position = ( dragStartPosition + TouchInput.dragVector ).WithZReplacedBy ( dragSource.transform.position.z );
					dragSource.DragTo ( position, TouchInput.dragVector );
				}
			}

			if ( !TouchInput.isDown )
			{
				if ( dragStarted )
					StopDrag ( IsAccepting () );
				else
					dragSource.OnClick ();

				dragSource = null;
			}
		}
	}

	private DragOver IsAccepting ()
	{
		DragOver response = dragSource.IsAccepting ( dropTarget );

		if ( response == DragOver.Accept )
			if ( dropTarget != null )
				response = dropTarget.IsAccepting ( dragSource );

		return response;
	}

	private void StartDrag ()
	{
		dragStarted = true;

		dragStartPosition = dragSource.transform.position;

		dragSource.OnStartDrag ();
		dragSource.OnBringToFront ();
		dragSource.GetComponent<Collider2D> ().enabled = false;
	}

	private void StopDrag ( DragOver response )
	{
		dragSource.GetComponent<Collider2D> ().enabled = true;
		
		dragStarted = false;

		if ( response == DragOver.Accept )
		{
			dragSource.OnDrop ( dropTarget );

			if ( dropTarget != null )
				dropTarget.OnDrop ( dragSource );

			if ( DontSendToBack )
				DontSendToBack = false;
			else
				dragSource.OnSendToBack ();
		}
		else if ( response == DragOver.Refuse )
		{
			dragSource.DragBackTo ( dragStartPosition );
		}

		dragSource.OnStopDrag ();

		dropTarget = null;
		dragSource = null;
	}
}
