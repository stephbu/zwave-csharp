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
	
	class ZwaveListener : IDisposable
	{
		public delegate bool HandleMessage(Message Message);

		private SerialPort port;
		private bool disposed = false;
		private AutoResetEvent exit = new AutoResetEvent(false);
		private Thread thread;

		private ConcurrentQueue<byte> incomingBytes = new ConcurrentQueue<byte>();
		private ConcurrentQueue<Message> outboundMessages = new ConcurrentQueue<Message> ();

		private Message pendingAcknowledgement;

		public ZwaveListener(string portName)
		{
			port = new SerialPort (portName);
			InitializePort (port);

			this.thread = new Thread (new ThreadStart (Pump));
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

		private void Pump()
		{
			while (!exit.WaitOne(0)) {

				if(!this.port.IsOpen)
				{
					exit.Set();
				}

				var slowLoop = true;

				// Read bytes

				int pendingBytes = this.port.BytesToRead;
				if(pendingBytes > 0)
				{
					slowLoop = false;

					Log (string.Format("Received {0} bytes",pendingBytes));

					for (int index = 0; index < pendingBytes; index++) {
						byte incomingByte = (byte)this.port.ReadByte();
						this.incomingBytes.Enqueue(incomingByte);
						Console.WriteLine (incomingByte.ToString ("x"));
					}

					Message incomingMessage = Message.Parse (pendingBytes, this.incomingBytes);

					if (incomingMessage is Acknowledge && pendingAcknowledgement != null) 
					{
						pendingAcknowledgement = null;
					}
					else if (incomingMessage != null) 
					{
						this.OnMessage (incomingMessage);
					}
				}
					
				if (outboundMessages.Count > 0 && pendingAcknowledgement == null) {

					if (outboundMessages.TryDequeue (out pendingAcknowledgement)) {
						slowLoop = false;
						byte[] messageBytes = pendingAcknowledgement.GetChecksumBytes ().ToArray();
						port.Write (messageBytes, 0, messageBytes.Length);
					}
				}

				if (slowLoop) {
					Thread.Sleep(10);
				}
			}
		}

		public void Log(Object str)
		{
			Console.WriteLine("{0:yyyy-MM-dd:hh:mm:ss} {1}", DateTime.Now, str);
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
			this.outboundMessages.Enqueue (message);
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
