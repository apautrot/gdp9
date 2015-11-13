using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer ( typeof ( EnumFlagAttribute ) )]
public class EnumFlagDrawer : PropertyDrawer
{
	bool initialized;
	string propName;
	string[] options;

	public override void OnGUI ( Rect position, SerializedProperty property, GUIContent label )
	{
		if ( !initialized )
		{
			Enum targetEnum = GetBaseProperty<Enum> ( property );

			EnumFlagAttribute flagSettings = (EnumFlagAttribute)attribute;
			propName = flagSettings.enumName;
			if ( string.IsNullOrEmpty ( propName ) )
				propName = property.name.Capitalize ();

			Type enumType = targetEnum.GetType ();
			Array values = Enum.GetValues ( enumType );
			List<string> enumNames = new List<string> ();
			foreach ( var enumValue in values )
				if ( (int)enumValue != 0 )
					enumNames.Add ( enumValue.ToString () );
			options = enumNames.ToArray ();

			initialized = true;
		}

		EditorGUI.BeginProperty ( position, label, property );
		int newValue = EditorGUI.MaskField ( position, propName, property.intValue, options );
		if ( newValue != property.intValue )
			property.intValue = newValue;
// 		Enum enumNew = EditorGUI.EnumMaskField ( position, propName, targetEnum );
// 		property.intValue = (int)Convert.ChangeType ( enumNew, targetEnum.GetType () );
		EditorGUI.EndProperty ();
	}

	static T GetBaseProperty<T> ( SerializedProperty prop )
	{
		// Separate the steps it takes to get to this property
		string[] separatedPaths = prop.propertyPath.Split ( '.' );

		// Go down to the root of this serialized property
		System.Object reflectionTarget = prop.serializedObject.targetObject as object;
		// Walk down the path to get the target object
		foreach ( var path in separatedPaths )
		{
			FieldInfo fieldInfo = reflectionTarget.GetType ().GetField ( path );
			reflectionTarget = fieldInfo.GetValue ( reflectionTarget );
		}
		return (T)reflectionTarget;
	}
}
