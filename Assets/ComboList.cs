using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Combo
{
	public List<InputActionName> actions;
}

public class ComboList : SceneSingleton<ComboList>
{
	public float ScrollingSpeed = 100;
	public float InterComboSpacing = 50;

	public List<Combo> list;

	internal Combo GetCombo()
	{
		if ( list.Count == 0 )
			Debug.LogError ( "There is no combo in ComboList" );

		return list.GetRandomElement ();
	}
}
