using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor ( typeof ( Go ) )]
public class GoInspector : Editor
{
	static string previousDump;

	public override void OnInspectorGUI ()
	{
		this.DrawDefaultInspector ();
		Go go = target as Go;

		string dump = go.getTweensDump ();
		EditorGUILayout.LabelField ( dump, GUILayout.MaxHeight ( 500 ) );

		if ( previousDump != dump )
		{
			EditorUtility.SetDirty ( target );
			previousDump = dump;
		}
	}
}