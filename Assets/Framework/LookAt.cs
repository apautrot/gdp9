using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class LookAt : MonoBehaviour
{
	public Transform targetTransform;
	public bool targetPositionIsWorld = true;
	public Vector3 targetPosition;
	public bool enableInEditor = true;
	public bool inverseLookAt;

 	void Update ()
 	{
		if ( ! ( Application.isEditor && !Application.isPlaying ) || enableInEditor )
			if ( targetTransform != null )
				LookAtPosition ( targetTransform.position );
			else
			{
				Vector3 lookAtPosition = targetPosition;
				if ( ! targetPositionIsWorld )
				{
					Transform parentTransform = transform.parent;
					if ( parentTransform != null )
						lookAtPosition = parentTransform.TransformPoint ( targetPosition );
				}

				LookAtPosition ( lookAtPosition );
			}
	}

	void LookAtPosition ( Vector3 position )
	{
		if ( inverseLookAt )
		{
			Vector3 selfPosition = transform.position;
			position = selfPosition + ( selfPosition - position );
		}

		transform.LookAt ( position, Vector3.up );
	}
}