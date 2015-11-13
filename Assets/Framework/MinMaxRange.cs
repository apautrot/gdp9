using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* MinMaxRangeAttribute.cs
* by Eddie Cameron – For the public domain
* —————————-
* Use a MinMaxRange class to replace twin float range values (eg: float minSpeed, maxSpeed; becomes MinMaxRange speed)
* Apply a [MinMaxRange( minLimit, maxLimit )] attribute to a MinMaxRange instance to control the limits and to show a
* slider in the inspector
*/

[System.Serializable]
public struct MinMaxFloatRange
{
	public MinMaxFloatRange ( float value )
	{
		this.min = value;
		this.max = value;
	}

	public MinMaxFloatRange ( float min, float max )
	{
		this.min = min;
		this.max = max;
	}

	public float min, max;

	public float GetRandomValue ()
	{
		if ( min == max )
			return min;

		return Random.Range ( min, max );
	}

	public float GetRange ()
	{
		return max - min;
	}

	public bool IsZero ()
	{
		return ( min == 0.0f && max == 0.0f );
	}
}

[System.Serializable]
public struct MinMaxIntRange
{
	public int min, max;

	public MinMaxIntRange ( int value )
	{
		min = value;
		max = value;
	}

	public int GetRandomValue ()
	{
		if ( min == max )
			return min;

		return Random.Range ( min, max );
	}

	public float GetRange ()
	{
		return max - min;
	}
}

public class MinMaxRangeAttribute : PropertyAttribute
{
	public float minLimit, maxLimit;
	public bool showSlider;
	public MinMaxRangeAttribute ( float minLimit, float maxLimit, bool showSlider = false )
	{
		this.minLimit = minLimit;
		this.maxLimit = maxLimit;
		this.showSlider = showSlider;
	}
}