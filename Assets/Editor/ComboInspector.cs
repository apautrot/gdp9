using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;


[CustomPropertyDrawer ( typeof ( ComboAttribute ) )]
public class ComboInspector : PropertyDrawer
{
	public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
	{
		SerializedProperty actions = property.FindPropertyRelative ( "actions" );

		int count = actions.arraySize;
		string actionsAsString = "";
		for ( int i = 0; i < count; i++ )
		{
			SerializedProperty sp = actions.GetArrayElementAtIndex ( i );
			InputActionName name = (InputActionName) sp.enumValueIndex;

			if ( i > 0 )
				actionsAsString += " ";

			actionsAsString += name.ToString ();
		}

		Rect propRect = new Rect ( position.x, position.y, position.width - 100, position.height );
		EditorGUI.PropertyField ( propRect, property, new GUIContent ( actionsAsString ), true );

		SerializedProperty isActiveProperty = property.FindPropertyRelative ( "isActive" );
		Rect isActiveRect = new Rect ( position.xMax - 50, position.y, 50, position.height );
		EditorGUI.PropertyField ( isActiveRect, isActiveProperty, GUIContent.none );
	}

	public override float GetPropertyHeight ( SerializedProperty property, GUIContent label )
	{
		return EditorGUI.GetPropertyHeight ( property );
	}

}

