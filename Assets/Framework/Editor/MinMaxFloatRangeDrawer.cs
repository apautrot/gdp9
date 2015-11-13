/* MinMaxRangeDrawer.cs
* by Eddie Cameron - For the public domain
* ———————————————————-
* — EDITOR SCRIPT : Place in a subfolder named ‘Editor’ —
* ———————————————————-
* Renders a MinMaxRange field with a MinMaxRangeAttribute as a slider in the inspector
* Can slide either end of the slider to set ends of range
* Can slide whole slider to move whole range
* Can enter exact range values into the From: and To: inspector fields
*
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer ( typeof ( MinMaxRangeAttribute ) )]
public class MinMaxFloatRangeDrawer : PropertyDrawer
{
	public override float GetPropertyHeight ( SerializedProperty property, GUIContent label )
	{
		var range = attribute as MinMaxRangeAttribute;
		return base.GetPropertyHeight ( property, label ) + ( range.showSlider ? 16 : 0 );
	}

	// Draw the property inside the given rect
	public override void OnGUI ( Rect position, SerializedProperty property, GUIContent label )
	{
		bool isIntMinMax = ( property.type == "MinMaxIntRange" );
		bool isFloatMinMax = ( property.type == "MinMaxFloatRange" );

		// Now draw the property as a Slider or an IntSlider based on whether it’s a float or integer.
		if ( ! ( isIntMinMax || isFloatMinMax ) )
			Debug.LogWarning ( "Use only with MinMaxRange type" );
		else
		{
			var range = attribute as MinMaxRangeAttribute;
			var minValue = property.FindPropertyRelative ( "min" );
			var maxValue = property.FindPropertyRelative ( "max" );
			float newMin = 0;
			float newMax = 0;

			if ( isIntMinMax )
			{
				newMin = minValue.intValue;
				newMax = maxValue.intValue;
			}
			else
			{
				newMin = minValue.floatValue;
				newMax = maxValue.floatValue;
			}

			var xDivision = position.width * 0.33f;
			var yDivision = position.height * ( range.showSlider ? 0.5f : 1.0f );
			// EditorGUI.PropertyField ( new Rect ( position.x, position.y, xDivision, yDivision ), property/*, label*/ );
			EditorGUI.LabelField ( new Rect ( position.x, position.y, xDivision + 30, yDivision ), label );

			if ( range.showSlider )
			{
				EditorGUI.LabelField ( new Rect ( position.x, position.y + yDivision, position.width, yDivision ), range.minLimit.ToString ( "0.##" ) );
				EditorGUI.LabelField ( new Rect ( position.x + position.width - 28.0f, position.y + yDivision, position.width, yDivision ), range.maxLimit.ToString ( "0.##" ) );
				EditorGUI.MinMaxSlider ( new Rect ( position.x + 32.0f, position.y + yDivision, position.width - 64.0f, yDivision ), ref newMin, ref newMax, range.minLimit, range.maxLimit );
			}

			if ( isIntMinMax )
			{
				newMin = (int)newMin;
				newMax = (int)newMax;
			}

			if ( isIntMinMax )
			{
				if ( minValue.intValue != (int)newMin )
					minValue.intValue = (int)newMin;
				if ( maxValue.intValue != (int)newMax )
					maxValue.intValue = (int)newMax;
			}
			else
			{
				if ( minValue.floatValue != newMin )
					minValue.floatValue = newMin;
				if ( maxValue.floatValue != newMax )
					maxValue.floatValue = newMax;
			}

			// EditorGUI.LabelField ( new Rect ( position.x + xDivision, position.y, xDivision, yDivision ), "From: " );
			EditorGUI.PropertyField (
				new Rect ( position.x + xDivision + 30, position.y, xDivision - 20, yDivision ),
				minValue,
				GUIContent.none );

			EditorGUI.PropertyField (
				new Rect ( position.x + xDivision * 2f + 14, position.y, xDivision - 14, yDivision ),
				maxValue,
				GUIContent.none );

// 			bool minChanged = GUI.changed;
// 			GUI.changed = false;
// 			//EditorGUI.LabelField ( new Rect ( position.x + xDivision * 2f, position.y, xDivision, yDivision ), "To: " );
// 			newMax = Mathf.Clamp ( EditorGUI.FloatField ( new Rect ( position.x + xDivision * 2f + 14, position.y, xDivision - 14, yDivision ), newMax ), newMin, range.maxLimit );
// 			bool maxChanged = GUI.changed;

// 			if ( minChanged )
// 			{
// 				Debug.Log ( "Min changed" );
// 				if ( isIntMinMax )
// 					minValue.intValue = (int)newMin;
// 				else
// 					minValue.floatValue = newMin;
// 			}
// 
// 			if ( maxChanged )
// 			{
// 				Debug.Log ( "Min changed" );
// 				if ( isIntMinMax )
// 					maxValue.intValue = (int)newMax;
// 				else
// 					maxValue.floatValue = newMax;
// 			}
		}
	}
}
