using UnityEngine;
using UnityEditor;

public class LockUnlockInspector
{

	// &#%
	[MenuItem ( "Edit/(Un)Lock inspector &F1", false, 1100 )]
	static void Execute ()
	{
		EditorWindow window = InspectorWindowHelper.GetInspectorWindow ();
		if ( window != null )
			InspectorWindowHelper.set_IsLocked ( window, ! InspectorWindowHelper.get_IsLocked ( window ) );
	}
}
