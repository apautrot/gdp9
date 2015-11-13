using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

delegate void WriteChunkDelegate ( BinaryWriter bw );
delegate void ReadChunkDelegate ( BinaryReader br );

static class BinaryWriterExtensionMethods
{
	private const uint StartBytes = 0xDEADBEEF;
	private const uint EndBytes = 0x0BADF00D;

	static public void Write ( this BinaryWriter self, Vector3 v )
	{
		self.Write ( v.x );
		self.Write ( v.y );
		self.Write ( v.z );
	}

	static public void WriteChunck ( this BinaryWriter self, WriteChunkDelegate writeAction )
	{
		MemoryStream ms = new MemoryStream ();
		BinaryWriter bw2 = new BinaryWriter ( ms );

		writeAction.Invoke ( bw2 );

		// self.Write ( StartBytes );
		self.Write ( (uint)ms.Length );
		if ( ms.Length > 0 )
			self.Write ( ms.GetBuffer (), 0, (int)ms.Length );
		// self.Write ( EndBytes );
	}
}

static class BinaryReaderExtensionMethods
{
	private const uint StartBytes = 0xDEADBEEF;
	private const uint EndBytes = 0x0BADF00D;

	static public Vector3 ReadVector3 ( this BinaryReader self )
	{
		Vector3 v = new Vector3 ();
		v.x = self.ReadSingle ();
		v.y = self.ReadSingle ();
		v.z = self.ReadSingle ();
		return v;
	}

	static public void ReadGuardBytes ( this BinaryReader self, uint bytes )
	{
		uint readBytes = self.ReadUInt32 ();
		if ( readBytes != bytes )
			throw new System.Exception ( string.Format ( "Error reading control bytes, found {0:X8} instead of {1:x8}", readBytes, bytes ) );
	}

	static public void ReadChunk ( this BinaryReader self, ReadChunkDelegate readAction )
	{
		// self.ReadGuardBytes ( StartBytes );

		uint chunckSize = self.ReadUInt32 ();
		if ( chunckSize > 0 )
		{
			long position = self.BaseStream.Position;

			byte[] bytes = self.ReadBytes ( (int)chunckSize );
			MemoryStream ms = new MemoryStream ( bytes );

			try { readAction.Invoke ( new BinaryReader ( ms ) ); }
			catch ( System.Exception ex )
			{
				Debug.LogError ( "Error reading chunk stream. Got an exception." );
				Debug.LogException ( ex );
			}

			long positionToBe = ( position + chunckSize );
			if ( self.BaseStream.Position != positionToBe )
			{
				Debug.LogError ( "Error reading stream, chunk size mismatch. Seeking in stream ( position is " + self.BaseStream.Position + ", should be : " + positionToBe + "." );
				self.BaseStream.Seek ( positionToBe, SeekOrigin.Begin );
			}

		}

		// self.ReadGuardBytes ( EndBytes );
	}
}