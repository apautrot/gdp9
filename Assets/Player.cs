using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	internal SkeletonAnimation spineAnimation;

	void Awake()
	{
		spineAnimation = GetComponent<SkeletonAnimation> ();
    }

	void Update ()
	{
		if ( Input.GetKeyDown ( KeyCode.F1 ) )
			spineAnimation.AnimationName = "IDLE";

		if ( Input.GetKeyDown ( KeyCode.F2 ) )
			spineAnimation.AnimationName = "ATTACK";

		if ( Input.GetKeyDown ( KeyCode.F3 ) )
			spineAnimation.AnimationName = "HIT";

		if ( Input.GetKeyDown ( KeyCode.F4 ) )
			spineAnimation.AnimationName = "ANTE_ATTACK";
	}
}
