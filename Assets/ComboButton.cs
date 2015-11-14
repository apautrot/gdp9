using UnityEngine;
using System.Collections;

public class ComboButton : MonoBehaviour
{
	SpriteRenderer spriteRenderer;

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();
		if ( spriteRenderer == null )
			Debug.LogError ( "Missing component SpriteRenderer on " + name );
    }

	private bool isActivated;
	internal bool Activated
	{
		get
		{
			return isActivated;
		}
		set
		{
			if ( isActivated != value )
			{
				isActivated = value;
				// spriteRenderer.material.SetAlpha ( isActivated ? 0.5f : 1.0f );
				if ( isActivated )
				{
					transform.localScale = new Vector3 ( 1.2f, 1.2f, 1.2f );
					transform.localPosition = transform.localPosition.WithYReplacedBy ( 20 );
				}
				else
				{
					transform.localScale = new Vector3 ( 1.0f, 1.0f, 1.0f );
					transform.localPosition = transform.localPosition.WithYReplacedBy ( 0 );
				}
			}
        }
	}
}
