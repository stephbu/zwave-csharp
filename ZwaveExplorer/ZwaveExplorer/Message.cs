using System;
using System.IO;
using System.Net;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace ZwaveExplorer
{
	public abstract class Message
	{
		public byte Length = 0;

		public abstract IEnumerable<byte> GetBytes ();

		public static Message Parse(int bytes, ConcurrentQueue<byte> queue)
		{
			var messageBytes = new byte[bytes];
			for (int index = 0; index < bytes; index++) {
				queue.TryDequeue (out messageBytes [index]);
			}

			switch ((Preamble)messageBytes [0]) {
			case Preamble.SOF:
				{
					UInt16 length = messageBytes [1];
					FrameType frameType = (FrameType) messageBytes [2];
					MessageFunction function = (MessageFunction)messageBytes [3];
					//	Home ID ..
					//	Source Node ID
					//	Frame header ..
					//	Length
					//	Destination address ..
					//	Data byte 0-x
					//		..
					//		..
					//	Checksum
					break;
				}
			case Preamble.ACK:
				{
					return new Acknowledge ();
				}
			}

			return null;
		}

		public IEnumerable<byte> GetChecksumBytes()
		{
			var byteEnumerator = this.GetBytes ().GetEnumerator ();;
			byteEnumerator.MoveNext ();

			// skip preamble

			yield return byteEnumerator.Current;

			byte checksum = 0xff;
			while (byteEnumerator.MoveNext ()) {

				var currentByte = byteEnumerator.Current;
				checksum ^= currentByte;
				yield return currentByte;
			}

			yield return checksum;
		}
	}

	public enum FrameType : byte
	{
		Request = 0x00,
		Response = 0x01
	}

	public abstract class SingleCast : Message
	{
	}
		
	public class Acknowledge : Message
	{
		public override IEnumerable<byte> GetBytes ()
		{
			return new byte[] { (byte) Preamble.ACK };
		}
	}

	public class Discovery : SingleCast
	{
		public Discovery()
		{
			this.Length = 0x03;
		}

		public override IEnumerable<byte> GetBytes ()
		{
			yield return (byte) Preamble.SOF;
			yield return this.Length;
			yield return (byte) FrameType.Request;
			yield return (byte) MessageFunction.DiscoveryNodes;
		}
	}

}
