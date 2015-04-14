using System;
using System.IO;
using System.Net;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace ZwaveExplorer
{
	
	class ZwaveListener : IDisposable
	{
		public delegate bool HandleMessage(Message Message);

		private SerialPort port;
		private bool disposed = false;
		private AutoResetEvent exit = new AutoResetEvent(false);
		private Thread thread;

		private Queue<byte> incomingBytes = new Queue<byte>();

		public ZwaveListener(string portName)
		{
			port = new SerialPort (portName);
			InitializePort (port);

			this.thread = new Thread (new ThreadStart (Executive));
		}

		public HandleMessage OnMessage;

		private void InitializePort(SerialPort port)
		{
			port.BaudRate = 115200;
			port.Parity = Parity.None;
			port.DataBits = 8;
			port.StopBits = StopBits.One;
			port.Handshake = Handshake.None;
			port.DtrEnable = true;
			port.RtsEnable = true;
			port.NewLine = System.Environment.NewLine;
		}

		private void Executive()
		{
			while (!exit.WaitOne(0)) {

				if(!this.port.IsOpen)
				{
					exit.Set();
				}

				int pendingBytes = this.port.BytesToRead;
				if(pendingBytes > 0)
				{
					byte incomingByte = (byte)this.port.ReadByte();
					this.incomingBytes.Enqueue(incomingByte);
					Console.WriteLine (incomingByte.ToString ("x"));
				}
				Thread.Sleep(10);
			}
		}

		public void Start()
		{
			port.Open ();
			this.thread.Start ();
		}

		public void Dispose()
		{
			this.Dispose (true);
		}

		public void Send (Message message)
		{
			byte[] outgoingBytes = message.GetBytes ().ToArray();
			this.port.Write (outgoingBytes, 0, outgoingBytes.Length);
		}

		private void Dispose(bool disposing)
		{
			if (disposed) {
				return;
			}

			if (exit != null) {
				exit.Set();
				// wait up to 10 seconds for thread to exit
				thread.Join(10000);
				exit.Dispose ();
				exit = null;
				thread = null;
			}
		}

	}
}
