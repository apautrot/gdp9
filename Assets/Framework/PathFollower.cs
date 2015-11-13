using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class PathFollower : MonoBehaviour
{
	public GameObject Path;
	public float Duration = 1;
	public float StartTime = 0;
	public GoEaseType Ease = GoEaseType.Linear;
	public bool LoopInfinitely = true;
	public bool IsRelative;

	GoTween tween;

	void OnDestroy ()
	{
		if ( tween != null )
			tween.destroy ();
	}

	void Start ()
	{
		if ( Path != null )
		{
			tween = Path.GetComponent<PointPath> ().Animate ( gameObject, Duration, Ease, LoopInfinitely, IsRelative );

			if ( StartTime != 0 )
				tween.goTo ( StartTime );
		}
	}

	void Restart ()
	{
		Start ();
	}

#if UNITY_EDITOR

	[MenuItem ( "GameObject/Create Other/Make Path Follower", true, 1000 )]
	public static bool CreateValidate ()
	{
		return Selection.activeGameObject != null;
	}

	[MenuItem ( "GameObject/Create Other/Make Path Follower", false, 1000 )]
	public static void Create ()
	{
		if ( Selection.activeGameObject != null )
		{
			GameObject path = new GameObject ( "Path" );
			path.GetOrCreateComponent<PointPath> ();
			if ( Selection.activeGameObject.transform.parent != null )
				path.transform.parent = Selection.activeGameObject.transform.parent.transform;
			path.transform.position = Selection.activeGameObject.transform.position;

			PathFollower pathFollower = Selection.activeGameObject.GetOrCreateComponent<PathFollower> ();
			pathFollower.Path = path;
		}
	}
#endif
}
