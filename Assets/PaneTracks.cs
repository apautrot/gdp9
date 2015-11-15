using UnityEngine;
using System.Collections;

public class PaneTracks : SceneSingleton<PaneTracks>
{
	float[] timeBeforeNextOnTrack = new float[3];
	
	void Generate ( int index )
	{
		timeBeforeNextOnTrack[index] -= Time.fixedDeltaTime;
		if ( timeBeforeNextOnTrack[index] <= 0 )
		{
			GameObject prefab = Game.Instance.prefabs.ComboPane;
			if ( prefab == null )
				Debug.LogError ( "Missing Game.prefabs.ComboPane prefab" );

			GameObject paneGO = gameObject.InstantiateChild ( prefab );
			paneGO.transform.localScale = Vector3.one;
			ComboPane pane = paneGO.GetComponent<ComboPane> ();
			if ( pane == null )
				Debug.LogError ( "Missing ComboPane component on Game.prefabs.ComboPane prefab" );

			// paneGO.transform.localPosition = transform.localPosition;

			float y = ( index - 1 ) * 180;
			pane.AlignLeftOn ( 640, y );

			timeBeforeNextOnTrack[index] += ( pane.Size * 0.75f );
        }
    }

	void FixedUpdate()
	{
		Generate ( 0 );
		Generate ( 1 );
		Generate ( 2 );
	}
}
