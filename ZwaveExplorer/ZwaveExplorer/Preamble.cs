using System;

namespace ZwaveExplorer
{
	public enum Preamble : byte
	{
		SOF = 0x01,
		ACK = 0x06,
		NAK = 0x15,
		CAN = 0x18
	}
}
