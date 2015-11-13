using UnityEngine;
using System.Collections;
using System.Reflection;

public abstract class MegaSpriteImageCompiler
{
	public static MegaSpriteImageCompiler Instance { get; set; }

	public abstract void Compile ( MegaSpriteImage image );
}
