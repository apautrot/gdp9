using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer ( typeof ( ConditionalMemberAttribute ) )]
public class ConditionalMemberDrawer : PropertyDrawer
{
	bool isInitialized;

	ConditionalMemberAttribute conditionalMemberAttribute;

	SerializedProperty conditionMember;
	MethodInfo conditionMethod;

	void InitializeIfNeeded ( SerializedProperty property )
	{
		if ( !isInitialized )
		{
			conditionalMemberAttribute = (ConditionalMemberAttribute)attribute;

			string path = property.propertyPath;
			int index = path.LastIndexOf ( '.' );
			if ( index == -1 )
				path = conditionalMemberAttribute.memberName;
			else
				path = path.Substring ( 0, index ) + "." + conditionalMemberAttribute.memberName;

			conditionMember = property.serializedObject.FindProperty ( path );

			if ( conditionMember == null )
			{
				Type serializedObjectType = property.serializedObject.targetObject.GetType ();
				conditionMethod = serializedObjectType.GetMethod ( conditionalMemberAttribute.memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
			}

			if ( ! IsConditionFound() )
				Debug.LogError ( "Neither member nor method with name " + conditionalMemberAttribute.memberName + " was found in type " + property.serializedObject.targetObject.GetType ().ToString() + "." );

			isInitialized = true;
		}
	}

	bool IsConditionFound ()
	{
		return ( conditionMember != null ) || ( conditionMethod != null );
	}

	bool IsCondition ( SerializedProperty property )
	{
		if ( conditionMember != null )
			return conditionMember.boolValue;
		
		if ( conditionMethod != null )
		{
			try
			{
				object result = conditionMethod.Invoke ( property.serializedObject.targetObject, null );
				return (bool)result;
			}
			catch ( System.Exception )
			{
			}
		}

		return false;
	}

	public override float GetPropertyHeight ( SerializedProperty property, GUIContent label )
	{
		InitializeIfNeeded ( property );

		if ( IsConditionFound() )
			if ( ! conditionalMemberAttribute.grayWhenDisabled )
				if ( ! IsCondition ( property ) )
					return -2;

		return base.GetPropertyHeight ( property, label );
	}

	public override void OnGUI ( Rect position, SerializedProperty property, GUIContent label )
	{
		InitializeIfNeeded ( property );

		if ( IsConditionFound () )
		{
			bool isCondition = IsCondition ( property );

			if ( !conditionalMemberAttribute.grayWhenDisabled )
				if ( ! isCondition )
					return;

			GUI.enabled = isCondition;
		}

// 		position.x += enabledByMemberAttribute.decal;
// 		position.width -= enabledByMemberAttribute.decal;
		if ( conditionalMemberAttribute.indent )
			EditorGUI.indentLevel++;

		EditorGUI.PropertyField ( position, property );

		if ( conditionalMemberAttribute.indent )
			EditorGUI.indentLevel--;
		
		GUI.enabled = true;
	}
}
