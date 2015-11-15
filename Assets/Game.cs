using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : SceneSingleton<Game>
{
	[System.Serializable]
	public class Prefabs
	{
		public GameObject ComboPane;
	}

	public Prefabs prefabs;

	

	internal BoxCollider2D BoxCollider2D;

	internal Rect Rectangle;

	void Awake()
	{
		BoxCollider2D = GetComponent<BoxCollider2D> ();
		if ( BoxCollider2D == null )
			Debug.LogError ( "There is no BoxCollider2D component on Game singleton" );

		Rectangle = new Rect
		(
			BoxCollider2D.bounds.min,
			BoxCollider2D.bounds.size
		);
    }

	internal List<ComboPane> ActiveComboPanes = new List<ComboPane>();

	internal void RegisterComboPane( ComboPane comboPane )
	{
		ActiveComboPanes.Add ( comboPane );
	}

	internal void UnregisterComboPane( ComboPane comboPane )
	{
		ActiveComboPanes.Remove ( comboPane );
	}




	void Start()
	{

	}


	// public Monster MonsterLeft;
	
}
