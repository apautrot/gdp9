using UnityEngine;

public class ConditionalMemberAttribute : PropertyAttribute
{
	public string memberName;
	public bool grayWhenDisabled;
	public bool indent;

	public ConditionalMemberAttribute ( string memberName, bool grayWhenDisabled = true, bool indent = true )
	{
		this.memberName = memberName;
		this.grayWhenDisabled = grayWhenDisabled;
		this.indent = indent;
	}
}
