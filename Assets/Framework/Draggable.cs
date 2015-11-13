using UnityEngine;
using System.Collections;

public enum DragOver
{
	Accept,
	Refuse,
	Ignore
}


public class Draggable : MonoBehaviour
{
	internal void OnMouseDown ()
	{
		if ( IsDraggable() )
			if ( DragDrop.Instance.DragSource == null )
				DragDrop.Instance.DragSource = this;
	}

	internal void OnDestroy ()
	{
		if ( DragDrop.InstanceCreated )
			if ( DragDrop.Instance.DragSource == this )
				DragDrop.Instance.DragSource = null;
	}

	internal virtual void DragTo ( Vector3 position, Vector3 delta )
	{
		transform.position = position;
	}

	internal virtual void DragBackTo ( Vector3 position )
	{
		DragDrop.Instance.DontSendToBack = true;
		float duration = ( transform.position - position ).magnitude * 0.0005f;
		transform.positionTo ( duration, position )
			.setOnCompleteHandler ( c => OnSendToBack () );
	}

	internal virtual bool IsDraggable ()
	{
		return true;
	}

	internal virtual void OnStartDrag ()
	{
	}

	internal virtual void OnStopDrag ()
	{
	}

	internal virtual void OnBringToFront ()
	{
	}

	internal virtual void OnSendToBack ()
	{
	}

	internal virtual DragOver IsAccepting ( IDropTarget dropTarget )
	{
		return DragOver.Accept;
	}

	internal virtual void OnDrop ( IDropTarget dropTarget )
	{
	}

	internal virtual void OnClick ()
	{
	}
}
