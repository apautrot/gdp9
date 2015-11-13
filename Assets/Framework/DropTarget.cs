using UnityEngine;
using System.Collections;


public interface IDropTarget
{
	DragOver IsAccepting ( Draggable dropped );

	void OnDrop ( Draggable dropped );
}