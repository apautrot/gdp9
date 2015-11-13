using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

public class MessageSender : MonoBehaviour
{
	public GameObject target;
	public string messageName;

	[System.Serializable]
	public class MessageSenderParams
	{
		public string paramString;
		public GameObject paramObject;
		public bool paramSelf;
		public bool recursiveBroadcast;
	}

	public MessageSenderParams messageParameters = new MessageSenderParams();

	protected void SendMessage ()
	{
		if ( target != null )
		{
			if ( messageParameters.recursiveBroadcast )
			{
				if ( messageParameters.paramSelf )					target.BroadcastMessage ( messageName, gameObject );
				else if ( messageParameters.paramObject != null )	target.BroadcastMessage ( messageName, messageParameters.paramObject );
				else												target.BroadcastMessage ( messageName, messageParameters.paramString );
			}
			else
			{
				if ( messageParameters.paramSelf )					target.SendMessage ( messageName, gameObject );
				else if ( messageParameters.paramObject != null )	target.SendMessage ( messageName, messageParameters.paramObject );
				else												target.SendMessage ( messageName, messageParameters.paramString );
			}
		}
		else
		{
			GameObjects.BroadcastMessageToScene ( messageName, messageParameters.paramString );
		}
	}
}

public class OnClick : MessageSender
{
	[System.Serializable]
	public class ScaleParameters
	{
		public bool scaleOnClick = true;
		public float scaleFactor = 1.1f;
	}

	public ScaleParameters scaleParameters = new ScaleParameters ();

	private Vector3 originalScale;
	private List<GoTween> tweeners;

	void OnMouseDown ()
	{
		if ( scaleParameters.scaleOnClick )
		{
			originalScale = transform.localScale;
			transform.localScale = new Vector3 ( originalScale.x * scaleParameters.scaleFactor, originalScale.y * scaleParameters.scaleFactor, originalScale.z * scaleParameters.scaleFactor );
			tweeners = Go.tweensWithTarget ( transform, true );
			foreach ( GoTween t in tweeners )
				t.pause ();
		}
	}

	void OnMouseUp ()
	{
		if ( scaleParameters.scaleOnClick )
		{
			transform.localScale = originalScale;
			foreach ( GoTween t in tweeners )
				t.play ();
		}

		if ( gameObject.IsUnderMouse () )
		{
			OnClicked ();
		}
	}

	protected virtual void OnClicked ()
	{
		SendMessage ();
	}
}