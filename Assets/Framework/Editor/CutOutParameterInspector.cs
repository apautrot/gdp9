#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor ( typeof ( CutOutParameter ) ), CanEditMultipleObjects]
class CutOutParameterInspector : Editor
{

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		if ( GUI.changed )
		{
			foreach ( CutOutParameter cop in targets )
			{
				cop.UpdateCutOutParameter ();
				cop.UpdateCutOutTexture ();
			}
		}
	}

}
#endif