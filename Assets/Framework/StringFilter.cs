using System.Collections.Generic;

public class StringFilter
{
	List<string> canBeFound = new List<string> ();	// can be found
	List<string> mustBeFound = new List<string> ();	// must be found 
	List<string> mustBeNotFound = new List<string> ();	// must be not found 

	string filterString;

	public string FilterString
	{
		get { return filterString; }
		set
		{
			filterString = value.ToLower ();

			string[] oSplittedPatterns = filterString.Split ( new char[] { ' ', '\t', '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries );

			canBeFound.Clear ();
			mustBeFound.Clear ();
			mustBeNotFound.Clear ();

			for ( int ui = 0; ui < oSplittedPatterns.Length; ui++ )
			{
				string pattern = oSplittedPatterns[ui];

				if ( pattern[0] == '+' )
				{
					if ( pattern.Length > 1 )
						canBeFound.Add ( pattern.Substring ( 1 ) );
				}
				else if ( pattern[0] == '-' )
				{
					if ( pattern.Length > 1 )
						mustBeNotFound.Add ( pattern.Substring ( 1 ) );
				}
				else
				{
					mustBeFound.Add ( pattern );
				}
			}

			if ( mustBeFound.Count == 1 )
			{
				canBeFound.Add ( mustBeFound[0] );
				mustBeFound.Clear ();
			}
		}
	}

	//! Returns if the given text pass through the match pattern.
	public bool DoesMatch ( string cstrText )
	{
		string strText = cstrText.ToLower ();

		// match by default if nothing to match
		bool bMatch = ( canBeFound.Count == 0 );

		// if found, be sure to match all "must be found"
		if ( bMatch )
			for ( int ui = 0; ui < mustBeFound.Count; ui++ )
				if ( !strText.Contains ( mustBeFound[ui] ) )
					return false;

		// else try to find a match in "can be found"
		if ( !bMatch )
			for ( int ui = 0; ui < canBeFound.Count; ui++ )
				if ( strText.Contains ( canBeFound[ui] ) )
				{
					bMatch = true;
					break;
				}

		// if still ok, be sure to not match any "must be not found"
		if ( bMatch )
			for ( int ui = 0; ui < mustBeNotFound.Count; ui++ )
				if ( strText.Contains ( mustBeNotFound[ui] ) )
				{
					bMatch = false;
					break;
				}

		return bMatch;
	}

}
