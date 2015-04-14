using System;
using System.IO;
using System.Net;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;

namespace ZwaveExplorer
{
	public abstract class Message
	{
		public abstract IEnumerable<byte> GetBytes ();
	}

	public abstract class SingleCast : Message
	{
	}

	public class Discover : SingleCast
	{
		public override IEnumerable<byte> GetBytes ()
		{
			return new byte[]{ 0x01, 0x03, 0x00, 0x02, 0xFE };
		}
	}

}
